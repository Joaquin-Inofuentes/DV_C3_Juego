using UnityEngine;
<<<<<<< HEAD
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
=======

public class TimerManager : MonoBehaviour
{
    // Timer general actual
    public float generalTimer;

    // Tiempo máximo del timer general
    public float generalTimerMax = 10f;

    // Lista de 7 timers individuales
    public float[] timers = new float[7];

    // Valores máximos para cada timer individual
    public float[] maxTimers = new float[7];

    // Booleans que indican si cada timer llegó a 0
    public bool[] isTimerZero = new bool[7];

    // Porcentaje (0 a 1) de cada timer respecto a su valor máximo
    public float[] timersPercent = new float[7];

    void Update()
    {
        // ⏱ Actualiza el timer general si es mayor a 0
        if (generalTimer > 0)
        {
            generalTimer -= Time.deltaTime; // Resta tiempo
            if (generalTimer < 0.2f) generalTimer = 0; // Si es muy bajo, lo pone en 0
        }

        // 🔁 Recorre todos los timers individuales
        for (int i = 0; i < timers.Length; i++)
        {
            // Si el timer está corriendo
            if (timers[i] > 0)
            {
                timers[i] -= Time.deltaTime; // Resta tiempo
                if (timers[i] < 0.2f) timers[i] = 0; // Si es muy bajo, lo pone en 0
            }

            // ✅ Marca si el timer está en 0
            isTimerZero[i] = timers[i] == 0;

            // 📊 Calcula el porcentaje (entre 0 y 1)
            timersPercent[i] = maxTimers[i] > 0 ? Mathf.Clamp01(timers[i] / maxTimers[i]) : 0;
        }
    }

    // 🔁 Reinicia un timer individual al valor máximo
    public void SetTimerToMax(int index)
    {
        if (index < 0 || index >= timers.Length) return; // Validación del índice
        timers[index] = maxTimers[index]; // Reinicia el timer con su valor máximo
    }

    // 🔁 Reinicia el timer general al valor máximo
>>>>>>> 1-joaco
    public void ResetGeneralTimer()
    {
        generalTimer = generalTimerMax;
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 1-joaco
