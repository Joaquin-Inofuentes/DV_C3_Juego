using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using CustomInspector;

/// <summary>
/// TimerManager gestiona un temporizador general y varios temporizadores individuales.
/// También actualiza la interfaz visual de los cooldowns (enfriamientos) en la UI.
/// Permite reiniciar temporizadores y consultar su estado.
/// Además, permite ejecutar eventos cuando un temporizador individual llega a 0.
/// </summary>
public class TimerManager : MonoBehaviour
{
    /// <summary>
    /// Índice del temporizador individual que se va a reiniciar manualmente.
    /// Se puede usar desde el inspector con un botón.
    /// </summary>
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

    [Header("👤 Referencia a AccionesJugador")]
    public AccionesJugador _AccionesDeJugador;

    [Header("⚡ Eventos cuando el timer llega a 0")]
    public UnityEvent[] onTimerFinished = new UnityEvent[7];

    void Update()
    {
        EjecutarTimers();
    }

    public void EjecutarTimers()
    {
        // 1. ⏱ Temporizador general
        if (generalTimer > 0)
        {
            generalTimer -= Time.deltaTime;
            if (generalTimer < 0.2f) generalTimer = 0;
        }

        // 2. 🔁 Temporizadores individuales
        for (int i = 0; i < timers.Length; i++)
        {
            // 2.1 Resta tiempo si está corriendo
            if (timers[i] > 0)
            {
                timers[i] -= Time.deltaTime;
                if (timers[i] < 0.2f) timers[i] = 0;
            }

            // 2.2 Detectar cambio a cero y disparar evento si aplica
            bool yaEstabaEnCero = isTimerZero[i];
            isTimerZero[i] = timers[i] == 0;

            if (!yaEstabaEnCero && isTimerZero[i])
            {
                if (onTimerFinished[i] != null)
                {
                    onTimerFinished[i].Invoke();
                }
            }

            // 2.3 Porcentaje restante del temporizador
            timersPercent[i] = maxTimers[i] > 0 ? Mathf.Clamp01(timers[i] / maxTimers[i]) : 0;

            // 2.4 Actualizar UI si corresponde
            if (cooldownOverlays[i] != null)
            {
                if (_AccionesDeJugador.modoMelee && i <= 2) continue;
                if (!_AccionesDeJugador.modoMelee && i >= 3) continue;

                cooldownOverlays[i].fillAmount = timersPercent[i];
            }
        }
    }

    /// <summary>
    /// Reinicia un temporizador individual al valor máximo.
    /// </summary>
    public void SetTimerToMax(int index)
    {
        if (index < 0 || index >= timers.Length) return;
        timers[index] = maxTimers[index];
    }

    /// <summary>
    /// Reinicia el temporizador general a su valor máximo.
    /// </summary>
    public void ResetGeneralTimer()
    {
        generalTimer = generalTimerMax;
    }

    /// <summary>
    /// Indica si un temporizador individual está "cargando".
    /// </summary>
    public bool IsTimerCharging(int index)
    {
        if (index < 0 || index >= timers.Length) return false;
        return timers[index] > 0.2f;
    }
}
