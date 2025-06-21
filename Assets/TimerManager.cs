using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using CustomInspector;
using System.Collections;
using System.Xml.Serialization;

/// <summary>
/// TimerManager gestiona 6 cooldowns (3 mágicos + 3 melee) y feedback visual.
/// Incluye flash rojo, shake, y animación visual cuando una habilidad está lista.
/// </summary>
public class TimerManager : MonoBehaviour
{
    [Button(nameof(SetTimerToMax), true)]
    public int IndiceAReiniciar = 0;

    [Header("⚔️ Modo actual")]
    public bool enModoMagico = true;

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
    public float effectDuration = 0.5f;
    public float pulseSpeed = 4f;
    public float pulseAmount = 0.1f;

    [Header("⚡ Flash cuando se habilita la habilidad")]
    public Color flashColor = Color.white;
    public float flashDuration = 0.15f;

    [Header("👤 Referencia a AccionesJugador")]
    public AccionesJugador _AccionesDeJugador;

    [Header("⚡ Eventos cuando el timer llega a 0")]
    public UnityEvent[] onTimerFinished = new UnityEvent[6];

    private Coroutine[] flashCoroutines = new Coroutine[6];
    private Color[] originalColors;
    private Vector2[] originalIconPositions;
    private Vector2[] originalOverlayPositions;
    private float[] readyEffectTimers = new float[6];
    private bool[] yaHizoEfecto = new bool[6];

    public static TimerManager Controler;

    void Start()
    {
        originalColors = new Color[6];
        originalIconPositions = new Vector2[6];
        originalOverlayPositions = new Vector2[6];
        yaHizoEfecto = new bool[6];

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
        }
    }

    void Update()
    {

        EjecutarTimers();
        if(Controler == null)
        {
            Controler = this;
        }
    }

    public void EjecutarTimers()
    {
        for (int i = 0; i < 6; i++)
        {

            if (timers[i] > 0f)
            {
                timers[i] -= Time.deltaTime;
                if (timers[i] < 0f) timers[i] = 0f;
            }

            timersPercent[i] = maxTimers[i] > 0f ? timers[i] / maxTimers[i] : 0f;
            isTimerZero[i] = timers[i] <= 0f;

            if (cooldownOverlays[i] != null)
                cooldownOverlays[i].fillAmount = timersPercent[i];

            if (isTimerZero[i] && Mathf.Approximately(timers[i], 0f))
            {
                if (!yaHizoEfecto[i])
                {
                    readyEffectTimers[i] = effectDuration;
                    StartCoroutine(DoFlashEffect(i));
                    onTimerFinished[i]?.Invoke();
                    yaHizoEfecto[i] = true;
                }
            }
            else if (!isTimerZero[i])
            {
                yaHizoEfecto[i] = false; // Se reinicia cuando vuelva a cargar
            }

            bool esMagia = i <= 2;
            if ((enModoMagico && !esMagia) || (!enModoMagico && esMagia))
                continue;
            if (readyEffectTimers[i] > 0f)
            {
                readyEffectTimers[i] -= Time.deltaTime;
                if (iconosCompletos[i] != null)
                {
                    float scaleFactor = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
                    iconosCompletos[i].localScale = new Vector3(scaleFactor, scaleFactor, 1f);
                }

                if (readyEffectTimers[i] <= 0f && iconosCompletos[i] != null)
                {
                    iconosCompletos[i].localScale = Vector3.one;
                }
            }
            else
            {
                if (iconosCompletos[i] != null)
                    iconosCompletos[i].localScale = Vector3.one;
            }
        }
    }

    private IEnumerator DoFlashEffect(int index)
    {
        var iconRT = iconosCompletos[index];
        if (iconRT == null) yield break;

        var image = iconRT.GetComponent<Image>();
        if (image == null) yield break;

        Color original = image.color;

        Color intenseFlash = new Color(flashColor.r * 4f, flashColor.g * 4f, flashColor.b * 4f, 1f);

        float halfDuration = flashDuration / 2f;
        float t = 0f;

        // Fade in
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            float lerp = t / halfDuration;
            image.color = Color.Lerp(original, intenseFlash, lerp);
            yield return null;
        }

        t = 0f;

        // Fade out
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            float lerp = t / halfDuration;
            image.color = Color.Lerp(intenseFlash, original, lerp);
            yield return null;
        }

        image.color = original;
    }


    public void SetTimerToMax(int index)
    {
        bool esMagia = index <= 2;
        if ((enModoMagico && !esMagia) || (!enModoMagico && esMagia))
            return;

        if (index < 0 || index >= 6) return;
        timers[index] = maxTimers[index];
    }

    public bool IsTimerCharging(int index)
    {
        //Debug.Log("Se pregunto el valor de carga para : " + index, gameObject);
        if (index < 0 || index >= 6) return false;
        return timers[index] > 0f;
    }

    public void MostrarFeedbackNoDisponible(int index)
    {
        bool esMagia = index <= 2;
        if ((enModoMagico && !esMagia) || (!enModoMagico && esMagia))
            return;

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






