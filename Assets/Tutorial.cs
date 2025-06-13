using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public List<string> Mensajes = new List<string>
    {
        "Bienvenido al tutorial. Aquí aprenderás los conceptos básicos del juego.",  // 0
        "Usa los clicks del mouse para moverte",  //1 
        "Ataca al enemigo con 1234",  // 2
        "Aprieta 4 y cambia a modo fisico",  // 3
        "¡Felicidades! Has completado el tutorial. Ahora estás listo para jugar." // 4
    };
    public TextMeshProUGUI _textoTutorial;
    public TextMeshProUGUI textoTutorial
    {
        get
        {
            if (_textoTutorial != null)
            {
                // Feedback visual: escalar el padre rápidamente
                Transform parent = _textoTutorial.transform.parent;
                if (parent != null)
                {
                    parent.localScale = Vector3.one * 1.2f;
                    // Asegúrate de que LeanTween y LeanTweenType estén disponibles en tu proyecto
                    if (parent != null)
                    {
                        StopAllCoroutines();
                        StartCoroutine(EscalarPadre(parent));
                    }
                }
            }
            return _textoTutorial;
        }
        set
        {
            _textoTutorial = value;
        }
    }
    public int faseActual = -1;

    public static Tutorial Instance;

    public void Awake()
    {
        SiguienteFase();
    }

    public void Start()
    {
        StartCoroutine(SiguienteFaseTrasEspera());
    }

    private IEnumerator SiguienteFaseTrasEspera()
    {
        yield return new WaitForSeconds(3f);
        if (faseActual == 1)
        {
            Debug.Log("Se cambio al nuevo mensaje del tutorial: " + Mensajes[faseActual]);
            SiguienteFase();
        }
    }

    public void Update()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SiguienteFase()
    {
        if (faseActual == Mensajes.Count) return; // Si ya está en la última fase, no hacer nada
        if (faseActual < Mensajes.Count)
        {
            TextMeshProUGUI Texto = textoTutorial;
            Texto.text = Mensajes[faseActual];
            faseActual++;
        }
    }
    float elapsed = 0f;
    float duration = 0.5f;
    private IEnumerator EscalarPadre(Transform parent)
    {
        parent.localScale = Vector3.one * 1.2f;
        Vector3 startScale = parent.localScale;
        Vector3 endScale = Vector3.one;
        while (elapsed < duration)
        {
            parent.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        parent.localScale = endScale;
    }
}
