using UnityEngine;
using UnityEngine.UI;
using CustomInspector;

public class TimerManager : MonoBehaviour
{
    [Button(nameof(SetTimerToMax), true)]
    public int IndiceAReiniciar = 0;

    [Header("⏱ Timer General")]
    public float generalTimer;
    public float generalTimerMax = 10f;

    [Header("⏱ Timers Individuales")]
    public float[] timers = new float[7];
    public float[] maxTimers = new float[7];
    public bool[] isTimerZero = new bool[7];
    public float[] timersPercent = new float[7];

    [Header("🖼️ UI Cooldown Overlays (Image con Fill)")]
    public Image[] cooldownOverlays = new Image[7];

    public AccionesJugador _AccionesDeJugador;

    void Update()
    {
        // 1. ⏱ Timer general
        if (generalTimer > 0)
        {
            generalTimer -= Time.deltaTime;
            if (generalTimer < 0.2f) generalTimer = 0;
        }

        // 2. 🔁 Timers individuales
        for (int i = 0; i < timers.Length; i++)
        {
            // 2.1 Resta tiempo si corre
            if (timers[i] > 0)
            {
                timers[i] -= Time.deltaTime;
                if (timers[i] < 0.2f) timers[i] = 0;
            }

            // 2.2 Estado de finalización
            isTimerZero[i] = timers[i] == 0;

            // 2.3 Porcentaje de tiempo restante
            timersPercent[i] = maxTimers[i] > 0 ? Mathf.Clamp01(timers[i] / maxTimers[i]) : 0;

            // 2.4 Actualiza UI del cooldown con imagen radial
            if (cooldownOverlays[i] != null)
            {
                // Omitir según modo melee
                if (_AccionesDeJugador.modoMelee && i <= 2) continue;
                if (!_AccionesDeJugador.modoMelee && i >= 3) continue;

                cooldownOverlays[i].fillAmount = timersPercent[i];
            }
        }
    }

    public void SetTimerToMax(int index)
    {
        if (index < 0 || index >= timers.Length) return;
        timers[index] = maxTimers[index];
    }

    public void ResetGeneralTimer()
    {
        generalTimer = generalTimerMax;
    }

    public bool IsTimerCharging(int index)
    {
        if (index < 0 || index >= timers.Length) return false;
        return timers[index] > 0.2f;
    }
}
