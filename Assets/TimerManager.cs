using UnityEngine;
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

    public void ResetGeneralTimer()
    {
        generalTimer = generalTimerMax;
    }
}
