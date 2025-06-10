using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using CustomInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// TimerManager gestiona 6 cooldowns (3 mágicos + 3 melee) y feedback visual.
/// Incluye flash rojo, shake, y animación visual cuando una habilidad está lista.
/// </summary>
public class TimerManager : MonoBehaviour
{
    [Button(nameof(SetTimerToMax), true)]
    public int IndiceAReiniciar = 0;

    [Header("⏱ Timers Individuales (6 habilidades)")]
    public float[] timers = new float[6];
    public float[] maxTimers = new float[6];
    public bool[] isTimerZero = new bool[6];
    public float[] timersPercent = new float[6];

    [Header("🖼️ Overlays UI (6 Image con Fill)")]
    public Image[] cooldownOverlays = new Image[6];

    [Header("🎯 Iconos completos (padres de los cooldowns)")]
    public RectTransform[] iconosCompletos = new RectTransform[6];

    [Header("✨ Efecto visual cuando la habilidad está lista")]
    public Animator[] readyAnimators = new Animator[6]; // NUEVO

    [Header("👤 Referencia a AccionesJugador")]
    public AccionesJugador _AccionesDeJugador;

    [Header("⚡ Eventos cuando el timer llega a 0")]
    public UnityEvent[] onTimerFinished = new UnityEvent[6];

    private Coroutine[] flashCoroutines = new Coroutine[6];
    private Color[] originalColors;
    private Vector2[] originalIconPositions;
    private Vector2[] originalOverlayPositions;

    void Start()
    {
        originalColors = new Color[6];
        originalIconPositions = new Vector2[6];
        originalOverlayPositions = new Vector2[6];

        for (int i = 0; i < 6; i++)
        {
            var overlay = cooldownOverlays[i];
            if (overlay != null)
            {
                originalColors[i] = overlay.color;
                originalOverlayPositions[i] = overlay.rectTransform.anchoredPosition;
            }

            if (iconosCompletos[i] != null)
                originalIconPositions[i] = iconosCompletos[i].anchoredPosition;

            if (readyAnimators[i] != null)
                readyAnimators[i].gameObject.SetActive(false); // Ocultar animadores al inicio
        }
    }

    void Update() => EjecutarTimers();

    public void EjecutarTimers()
    {
        for (int i = 0; i < 6; i++)
        {
            if (timers[i] > 0f)
            {
                timers[i] -= Time.deltaTime;
                if (timers[i] < 0f) timers[i] = 0f;
            }

            bool wasZero = isTimerZero[i];
            isTimerZero[i] = timers[i] == 0f;

            if (!wasZero && isTimerZero[i] && onTimerFinished[i] != null)
                onTimerFinished[i].Invoke();

            timersPercent[i] = maxTimers[i] > 0f ? Mathf.Clamp01(timers[i] / maxTimers[i]) : 0f;

            var overlay = cooldownOverlays[i];
            if (overlay != null)
                overlay.fillAmount = timersPercent[i];

            // Mostrar animación cuando está lista
            if (readyAnimators[i] != null)
            {
                if (isTimerZero[i])
                {
                    if (!readyAnimators[i].gameObject.activeSelf)
                    {
                        readyAnimators[i].gameObject.SetActive(true);
                        readyAnimators[i].Play("ReadyAnim", -1, 0f);
                    }
                }
                else
                {
                    if (readyAnimators[i].gameObject.activeSelf)
                        readyAnimators[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetTimerToMax(int index)
    {
        if (index < 0 || index >= 6) return;
        timers[index] = maxTimers[index];

        if (readyAnimators[index] != null)
            readyAnimators[index].gameObject.SetActive(false); // Ocultar el brillo al activar cooldown
    }

    public bool IsTimerCharging(int index)
    {
        if (index < 0 || index >= 6) return false;
        return timers[index] > 0f;
    }

    /// <summary>
    /// Flash rojo + shake suave sobre el ícono (0–5).
    /// </summary>
    public void MostrarFeedbackNoDisponible(int index)
    {
        if (index < 0 || index >= 6) return;
        var overlay = cooldownOverlays[index];
        if (overlay == null) return;

        if (flashCoroutines[index] != null)
        {
            StopCoroutine(flashCoroutines[index]);
            overlay.color = originalColors[index];
            overlay.rectTransform.anchoredPosition = originalOverlayPositions[index];
            if (iconosCompletos[index] != null)
                iconosCompletos[index].anchoredPosition = originalIconPositions[index];
        }

        flashCoroutines[index] = StartCoroutine(FlashAndShake(index));
    }

    private IEnumerator FlashAndShake(int index)
    {
        var overlay = cooldownOverlays[index];
        var overlayRT = overlay.rectTransform;
        var iconRT = iconosCompletos[index];

        var origColor = originalColors[index];
        var origOverlayPos = originalOverlayPositions[index];
        var origIconPos = originalIconPositions[index];

        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector2 shake = Random.insideUnitCircle * 5f;

            overlay.color = Color.red;
            if (overlayRT != null) overlayRT.anchoredPosition = origOverlayPos + shake;
            if (iconRT != null) iconRT.anchoredPosition = origIconPos + shake;

            yield return null;
        }

        if (overlay != null) overlay.color = origColor;
        if (overlayRT != null) overlayRT.anchoredPosition = origOverlayPos;
        if (iconRT != null) iconRT.anchoredPosition = origIconPos;

        flashCoroutines[index] = null;
    }
}





