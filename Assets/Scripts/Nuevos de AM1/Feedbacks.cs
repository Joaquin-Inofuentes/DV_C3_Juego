using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Feedbacks : MonoBehaviour
{
    // Propiedades
    public AccionesJugador S_AccionesJugador;
    private Animator animator;
    public string TipoDeAtaque; // Ejemplo: "Físico" o "Mágico"
    public RawImage BarraDeVida;
    public float Vida_TamanoMaximo;
    public TextMeshProUGUI Text_CantidadDeMonedas;

    public static Feedbacks Componente; // Singleton para acceder fácilmente desde otros scripts
    public GameObject PantallaDeCombo;
    void Start()
    {
        animator = GetComponent<Animator>(); // Obtiene el Animator del GameObject
        Vida_TamanoMaximo = BarraDeVida.rectTransform.rect.width;
    }

    void Update()
    {
        if(Componente == null)
        {
            Componente = this; // Asigna la instancia si aún no está asignada
        }
        // Aquí podrías manejar lógica de actualización, como el movimiento
        ActualizarBarra();
        if (GameManager.Componente)
        {
            Text_CantidadDeMonedas.text = "$ " + GameManager.Componente.ContadorDeMonedas.ToString();
        }
    }


    private void ActualizarBarra()
    {

        float nuevoValor = ((float)S_AccionesJugador.Vida / S_AccionesJugador.VidaMax) * Vida_TamanoMaximo;
        BarraDeVida.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nuevoValor);

    }

    public void AnimacionAtaque()
    {
        // Llama a la animación de ataque
        animator.SetTrigger("Ataque"); // Suponiendo que tienes un trigger llamado "Ataque" en el Animator
    }

    public void AnimacionMovimiento()
    {
        // Llama a la animación de movimiento
        animator.SetFloat("Velocidad", 1.0f); // Suponiendo que tienes un parámetro de velocidad
    }

    public void AnimacionMuerte()
    {
        // Llama a la animación de muerte
        animator.SetTrigger("Muerte"); // Suponiendo que tienes un trigger llamado "Muerte" en el Animator
    }

    public void CreacionDeProyectiles()
    {
        // Lógica para crear proyectiles
        // Por ejemplo, instanciar un proyectil en una posición dada:
        GameObject proyectil = Instantiate(Resources.Load("ProyectilPrefab") as GameObject, transform.position, Quaternion.identity);
        // Añadir lógica de dirección, velocidad, etc., al proyectil
    }


    public RawImage feedbackImage;
    private Coroutine currentRoutine;


    public void FeedbackRadialVisual(Color color, float duration)
    {
        // 1. Cancela la rutina previa si existe
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        // 2. Inicia la nueva y guarda su referencia
        currentRoutine = StartCoroutine(DoFeedback(color, duration));
    }
    private IEnumerator DoFeedback(Color color, float duration)
    {
        feedbackImage.color = color;
        feedbackImage.gameObject.SetActive(true);

        float elapsed = 0f;
        float startAlpha = color.a; // Guardamos el alpha original

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // El alpha va de startAlpha a 0
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);

            Color c = color;
            c.a = alpha;
            feedbackImage.color = c;

            yield return null;
        }

        feedbackImage.gameObject.SetActive(false);
        currentRoutine = null;
    }


}
