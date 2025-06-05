using UnityEngine;
using UnityEngine.UI;
using CustomInspector;

/// <summary>
/// TimerManager gestiona un temporizador general y varios temporizadores individuales.
/// También actualiza la interfaz visual de los cooldowns (enfriamientos) en la UI.
/// Permite reiniciar temporizadores y consultar su estado.
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
    /// <summary>
    /// Temporizador general que cuenta hacia atrás.
    /// </summary>
    public float generalTimer;
    /// <summary>
    /// Valor máximo que puede tener el temporizador general.
    /// </summary>
    public float generalTimerMax = 10f;

    [Header("⏱ Timers Individuales")]
    /// <summary>
    /// Arreglo de temporizadores individuales, uno para cada habilidad o acción.
    /// </summary>
    public float[] timers = new float[7];
    /// <summary>
    /// Valores máximos para cada temporizador individual.
    /// </summary>
    public float[] maxTimers = new float[7];
    /// <summary>
    /// Indica si cada temporizador individual ha llegado a cero.
    /// </summary>
    public bool[] isTimerZero = new bool[7];
    /// <summary>
    /// Porcentaje restante de cada temporizador individual (de 0 a 1).
    /// </summary>
    public float[] timersPercent = new float[7];

    [Header("🖼️ UI Cooldown Overlays (Image con Fill)")]
    /// <summary>
    /// Referencias a las imágenes de la UI que muestran el enfriamiento de cada habilidad.
    /// </summary>
    public Image[] cooldownOverlays = new Image[7];

    /// <summary>
    /// Referencia al componente AccionesJugador, para saber el modo de ataque actual.
    /// </summary>
    public AccionesJugador _AccionesDeJugador;

    /// <summary>
    /// Se llama automáticamente cada frame.
    /// Ejecuta la lógica de los temporizadores.
    /// </summary>
    void Update()
    {
        EjecutarTimers();
    }

    /// <summary>
    /// Actualiza el temporizador general y los temporizadores individuales.
    /// También actualiza la interfaz visual de los cooldowns.
    /// </summary>
    public void EjecutarTimers()
    {
        // 1. ⏱ Temporizador general
        if (generalTimer > 0)
        {
            // Resta el tiempo transcurrido desde el último frame
            generalTimer -= Time.deltaTime;
            // Si queda menos de 0.2 segundos, lo pone en cero para evitar valores negativos
            if (generalTimer < 0.2f) generalTimer = 0;
        }

        // 2. 🔁 Temporizadores individuales
        for (int i = 0; i < timers.Length; i++)
        {
            // 2.1 Resta tiempo si el temporizador está corriendo
            if (timers[i] > 0)
            {
                timers[i] -= Time.deltaTime;
                // Si queda menos de 0.2 segundos, lo pone en cero
                if (timers[i] < 0.2f) timers[i] = 0;
            }

            // 2.2 Marca si el temporizador llegó a cero
            isTimerZero[i] = timers[i] == 0;

            // 2.3 Calcula el porcentaje restante del temporizador (0 = terminado, 1 = completo)
            timersPercent[i] = maxTimers[i] > 0 ? Mathf.Clamp01(timers[i] / maxTimers[i]) : 0;

            // 2.4 Actualiza la imagen de la UI si existe
            if (cooldownOverlays[i] != null)
            {
                // Si el modo es melee, solo actualiza los primeros 3 overlays
                if (_AccionesDeJugador.modoMelee && i <= 2) continue;
                // Si el modo NO es melee, solo actualiza los overlays del 3 en adelante
                if (!_AccionesDeJugador.modoMelee && i >= 3) continue;

                // Cambia el tamaño horizontal de la imagen según el porcentaje restante
                Vector3 scale = cooldownOverlays[i].rectTransform.localScale;
                scale.x = 1 - timersPercent[i];
                cooldownOverlays[i].rectTransform.localScale = scale;
            }
        }
    }

    /// <summary>
    /// Reinicia un temporizador individual al valor máximo.
    /// </summary>
    /// <param name="index">Índice del temporizador a reiniciar (de 0 a 6).</param>
    public void SetTimerToMax(int index)
    {
        // Si el índice es inválido, no hace nada
        if (index < 0 || index >= timers.Length) return;
        // Pone el temporizador en su valor máximo
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
    /// Indica si un temporizador individual está "cargando" (es decir, si aún no ha terminado).
    /// </summary>
    /// <param name="index">Índice del temporizador a consultar.</param>
    /// <returns>True si el temporizador está activo, false si ya terminó.</returns>
    public bool IsTimerCharging(int index)
    {
        // Si el índice es inválido, devuelve false
        if (index < 0 || index >= timers.Length) return false;
        // Devuelve true si el temporizador tiene más de 0.2 segundos restantes
        return timers[index] > 0.2f;
    }
}