using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UI : MonoBehaviour
{
    [Header("Movimiento")]
    public Transform objetivo;              
    public Camera camara;
    public Vector3 offset = new Vector3(0, 1.8f, 0); 
    public float suavizado = 12f;

    [Header("Animación y Visual")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI textoUI;
    public float fadeTime = 0.3f;
    public float duracionVisible = 3f;

    [Header("Textos de diálogo puzzle")]
    public List<string> textosDePuzzle = new List<string>
    {
    "No puedo pasar por ahí",
    "No puedo",
    "¿Vos querés que me muera?"
    };

    private RectTransform rectTransform;
    private Vector3 destinoAnterior;
    public TextMeshProUGUI textoCombo;
    public float fadeDuration = 2f;

    public void MostrarTexto(string mensaje, Color color)
    {
        textoCombo.text = mensaje;
        textoCombo.color = new Color(color.r, color.g, color.b, 1f); 

        StopAllCoroutines();
        gameObject.SetActive(true);
        StartCoroutine(FadeOut());
    }


    private System.Collections.IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color originalColor = textoCombo.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            textoCombo.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        textoCombo.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        gameObject.SetActive(false);
    }
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
    void LateUpdate()
    {
        if (objetivo == null || camara == null) return;

        Vector3 screenPos = camara.WorldToScreenPoint(objetivo.position + offset);

        if (screenPos.z < 0)
        {
            canvasGroup.alpha = 0;
            return;
        }

        // Suaviza el movimiento de la burbuja
        if (destinoAnterior == Vector3.zero)
            destinoAnterior = screenPos;

        Vector3 posSuavizada = Vector3.Lerp(destinoAnterior, screenPos, Time.deltaTime * suavizado);
        rectTransform.position = posSuavizada;
        destinoAnterior = posSuavizada;
    }

    public void Mostrar(string mensaje)
    {
        StopAllCoroutines();
        StartCoroutine(MostrarRutina(mensaje));
    }

    private IEnumerator MostrarRutina(string mensaje)
    {
        textoUI.text = mensaje;
        canvasGroup.alpha = 0;
        gameObject.SetActive(true);

        // Fade in
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeTime);
            yield return null;
        }

        yield return new WaitForSeconds(duracionVisible);

        // Fade out
        t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeTime);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}


