using UnityEngine;
using UnityEngine.UI;
using CustomInspector;

public class TimerManager : MonoBehaviour
{
    [Button(nameof(SetTimerToMax),true)]
    public int IndiceAReiniciar = 0;


    [Header("⏱ Timer General")]
    public float generalTimer;
    public float generalTimerMax = 10f;

    [Header("⏱ Timers Individuales")]
    public float[] timers = new float[7];
    public float[] maxTimers = new float[7];
    public bool[] isTimerZero = new bool[7];
    public float[] timersPercent = new float[7];

    [Header("🖼️ UI Bars (RectTransform)")]
    public RectTransform[] cooldownBars = new RectTransform[7];

    [Header("📐 Configuración de UI")]
    public float barMaxWidth = 100f; // Ancho máximo de la barra

    public AccionesJugador _AccionesDeJugador;

    void Update()
    {
        // ⏱ Timer general
        if (generalTimer > 0)
        {
            generalTimer -= Time.deltaTime;
            if (generalTimer < 0.2f) generalTimer = 0;
        }

        // 🔁 Timers individuales
        for (int i = 0; i < timers.Length; i++)
        {
            // ⏬ Resta tiempo
            if (timers[i] > 0)
            {
                timers[i] -= Time.deltaTime;
                if (timers[i] < 0.2f) timers[i] = 0;
            }

            // ✅ Estado de finalización
            isTimerZero[i] = timers[i] == 0;

            // 📊 Porcentaje de tiempo restante
            timersPercent[i] = maxTimers[i] > 0 ? Mathf.Clamp01(timers[i] / maxTimers[i]) : 0;

            // 📏 Actualiza UI del cooldown
            if (cooldownBars[i] != null)
            {
                if (_AccionesDeJugador.modoMelee && i <= 2) continue;
                else if (!_AccionesDeJugador.modoMelee && i >= 3) continue;
                //Debug.Log("Se esta actualizando el timer de este indice " + i);
                float width = isTimerZero[i] ? barMaxWidth : timersPercent[i] * barMaxWidth;
                Vector2 size = cooldownBars[i].sizeDelta;
                cooldownBars[i].sizeDelta = new Vector2(width, size.y);
            }
        }
    }

    // 🔁 Reinicia timer individual
    public void SetTimerToMax(int index)
    {
        if (index < 0 || index >= timers.Length) return;
        timers[index] = maxTimers[index];
    }

    /// <summary>
    /// Retorna true si el timer en el índice dado está "cargándose" (es mayor a 0.2).
    /// </summary>
    public bool IsTimerCharging(int index)
    {
        if (index < 0 || index >= timers.Length)
            return false;

        return timers[index] > 0.2f;
    }


    // 🔁 Reinicia timer general
    public void ResetGeneralTimer()
    {
        generalTimer = generalTimerMax;
    }
}
