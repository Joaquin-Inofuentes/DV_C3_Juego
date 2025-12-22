using UnityEngine;
using UnityEngine.UI; // Necesario para la UI
using UnityEngine.Events;

public class TeleportSystem : MonoBehaviour
{
    [Header("Configuración Básica")]
    [Tooltip("Tiempo de espera en segundos.")]
    public float cooldownTime = 5.0f;
    [Tooltip("Altura fija Y al teletransportarse.")]
    public float alturaFijaY = 1.5f;

    [Header("Referencias UI")]
    [Tooltip("IMPORTANTE: En el Inspector, este objeto debe tener un componente 'Image' con Image Type: Filled.")]
    public Image circuloCarga; // Usamos Image para poder usar fillAmount
    [Tooltip("Imagen que se muestra cuando la habilidad está lista.")]
    public RawImage imagenListo;

    [Header("Eventos Unity")]
    // Evento que envía la posición DESDE donde sales
    public UnityEvent<Vector3> OnTeleportStart;
    // Evento que envía la posición HACIA donde llegas
    public UnityEvent<Vector3> OnTeleportEnd;

    // Variables internas
    private float _timerActual = 0f;
    private bool _estaHabilitado = true;

    void Start()
    {
        // Inicializar UI
        ActualizarVisuales(true);
        if (circuloCarga != null) circuloCarga.fillAmount = 0;

        Debug.Log("Sistema de Teletransporte Iniciado.");
    }

    void Update()
    {
        // 1. Lógica de Cooldown
        if (!_estaHabilitado)
        {
            _timerActual -= Time.deltaTime;

            // Actualizar gráfico del círculo (efecto reloj)
            if (circuloCarga != null)
            {
                // Invertimos el valor para que se llene en sentido horario
                // 1.0 es lleno, 0.0 es vacío
                float porcentaje = 1f - (_timerActual / cooldownTime);
                circuloCarga.fillAmount = porcentaje;
            }

            // Si el tiempo termina
            if (_timerActual <= 0)
            {
                _timerActual = 0;
                _estaHabilitado = true;
                ActualizarVisuales(true);
                Debug.Log("Cooldown finalizado. Habilidad Lista.");
            }
        }

        // 2. Lógica de Input (Solo si está habilitado)
        if (_estaHabilitado && Input.GetKeyDown(KeyCode.Q))
        {
            IntentarTeletransporte();
        }

        // 2. Lógica de Input (Solo si está habilitado)
        if (_estaHabilitado && Input.GetKeyDown(KeyCode.Space))
        {
            IntentarTeletransporte();
        }
    }

    void IntentarTeletransporte()
    {
        // Lanzar rayo desde la cámara al mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // A. Datos de posiciones
            Vector3 posOrigen = transform.position;
            Vector3 posDestino = hit.point;
            posDestino.y = alturaFijaY; // Aplicar altura fija

            // B. Iniciar Cooldown
            _estaHabilitado = false;
            _timerActual = cooldownTime;
            ActualizarVisuales(false);

            // C. Ejecutar lógica y eventos
            Debug.Log($"[TELEPORT START] De: {posOrigen} -> A: {posDestino}");

            // Evento Inicio
            if (OnTeleportStart != null) OnTeleportStart.Invoke(posOrigen);

            // Mover objeto
            transform.position = posDestino;

            // Evento Final
            if (OnTeleportEnd != null) OnTeleportEnd.Invoke(posDestino);

            Debug.Log($"[TELEPORT END] Nueva posición: {transform.position}");
        }
        else
        {
            Debug.LogWarning("No se encontró un lugar válido donde apunta el mouse.");
        }
    }

    // Alterna visibilidad entre el círculo de carga y la imagen de listo
    void ActualizarVisuales(bool estaListo)
    {
        if (imagenListo != null)
            imagenListo.gameObject.SetActive(estaListo);

        if (circuloCarga != null)
            circuloCarga.gameObject.SetActive(!estaListo);
    }
}