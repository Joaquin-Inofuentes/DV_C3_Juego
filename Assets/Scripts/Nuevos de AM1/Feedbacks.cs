using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Feedbacks : MonoBehaviour
{
    [Header("Referencias")]
    public AccionesJugador S_AccionesJugador;
    public RawImage BarraDeVida;
    public RawImage feedbackImage;
    public UI UIFadeComboScript;

    [Header("Sonidos")]
    public AudioSource S_MomentoEpico;

    public static Feedbacks Componente;

    private Animator animator;
    private float Vida_TamanoMaximo;
    private Coroutine currentRoutine;

    void Awake()
    {
        if (Componente == null) Componente = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        if (BarraDeVida != null)
        {
            Vida_TamanoMaximo = BarraDeVida.rectTransform.rect.width;
        }
        else
        {
            Debug.LogError("Referencia a 'BarraDeVida' no asignada en el Inspector de Feedbacks.", this);
        }
    }

    void Update()
    {
        // El canvas (barra de vida) se actualiza aquí cada frame para reflejar
        // los cambios hechos en la propiedad 'vidaActual' del jugador.
        ActualizarBarraDeVida();
    }

    /// <summary>
    /// Ajusta el tamaño visual de la barra de vida.
    /// Lee los valores directamente de las propiedades del jugador.
    /// </summary>
    private void ActualizarBarraDeVida()
    {
        if (S_AccionesJugador == null || BarraDeVida == null) return;

        float nuevoAncho = ((float)S_AccionesJugador.vidaActual / S_AccionesJugador.vidaMaxima) * Vida_TamanoMaximo;
        BarraDeVida.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nuevoAncho);
    }

    // El resto de los métodos se mantienen igual, ya que son llamados
    // desde el script del jugador cuando es necesario.

    #region Feedback Visual Radial
    public void FeedbackRadialVisual(Color color, float duration)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(DoFeedback(color, duration));
    }

    private IEnumerator DoFeedback(Color color, float duration)
    {
        feedbackImage.color = color;
        feedbackImage.gameObject.SetActive(true);
        float elapsed = 0f;
        float startAlpha = color.a;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            Color c = color;
            c.a = alpha;
            feedbackImage.color = c;
            yield return null;
        }
        feedbackImage.gameObject.SetActive(false);
        currentRoutine = null;
    }
    #endregion
}