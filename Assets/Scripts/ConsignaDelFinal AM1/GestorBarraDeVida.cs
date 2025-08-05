using UnityEngine;
using CustomInspector;

/// <summary>
/// Gestiona la visualización de una barra de vida para cualquier objeto
/// que implemente la interfaz IDaniable.
/// Se encarga de rotar hacia la cámara y escalar la barra según la vida del objetivo.
/// </summary>
public class GestorBarraDeVida : MonoBehaviour
{
    [Button(nameof(Start))]
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el GameObject que tiene el script del enemigo (o cualquier objeto con IDaniable).")]
    public GameObject objetivoDuenio;

    [Tooltip("El objeto padre de la barra de vida, que rotará para encarar a la cámara.")]
    public GameObject PadreDeLaBarraDeVida;

    [Tooltip("La barra de vida visual (la imagen roja/verde) que se escalará horizontalmente.")]
    public GameObject BarraDeVidaVisual;

    // La referencia a la interfaz del objetivo, para leer su vida.
    private IDaniable daniable;

    // El ancho inicial de la BarraDeVida para calcular el escalado correctamente.
    public float anchoOriginal;

    void Start()
    {
        // 1. Validar y obtener el componente IDaniable del objetivo.
        if (objetivoDuenio == null)
        {
            Debug.LogError("Error: No se ha asignado un 'objetivoDuenio' en el GestorBarraDeVida. El script se desactivará.", this);
            this.enabled = false;
            return;
        }

        daniable = objetivoDuenio.GetComponent<IDaniable>();

        if (daniable == null)
        {
            Debug.LogError($"Error: El 'objetivoDuenio' ({objetivoDuenio.name}) no tiene un componente que implemente la interfaz IDaniable. El script se desactivará.", this);
            this.enabled = false;
            return;
        }

        // 2. Validar las referencias de la UI.
        if (PadreDeLaBarraDeVida == null || BarraDeVidaVisual == null)
        {
            Debug.LogError("Error: No se han asignado los GameObjects de la barra de vida en el inspector. El script se desactivará.", this);
            this.enabled = false;
            return;
        }

        // 3. Guardar la escala inicial de la barra de vida.
        anchoOriginal = BarraDeVidaVisual.transform.localScale.x;
    }

    void Update()
    {
        // Si el dueño de la barra de vida fue destruido, también destruimos la barra.
        if (objetivoDuenio == null)
        {
            Destroy(this.gameObject); // Destruye el GameObject que contiene este script y la barra.
            return;
        }

        // En cada frame, se actualiza la rotación y el tamaño.
        ActualizarVisualmenteLaBarraDeVida();
    }

    /// <summary>
    /// Contiene toda la lógica para actualizar la rotación y la escala de la barra de vida.
    /// Lee los datos directamente desde la interfaz IDaniable.
    /// </summary>
    public void ActualizarVisualmenteLaBarraDeVida()
    {
        // #6. Si todos los enemigos actualizan su barra de vida de la misma manera lo deberían programar en la clase padre.
        // 1. Rotar el contenedor de la barra para que siempre mire a la cámara (solo en el eje Y).
        if (Camera.main != null)
        {
            Vector3 dirHaciaCamara = Camera.main.transform.position - PadreDeLaBarraDeVida.transform.position;
            dirHaciaCamara.y = 0; // Anula la rotación vertical para que la barra no se incline.

            if (dirHaciaCamara != Vector3.zero)
                PadreDeLaBarraDeVida.transform.rotation = Quaternion.LookRotation(dirHaciaCamara);
        }

        if (daniable == null)
        {
            if(objetivoDuenio != null)
            {
                if(objetivoDuenio.GetComponent<IDaniable>() != null)
                {
                    daniable = objetivoDuenio.GetComponent<IDaniable>();
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        } 
        // 2. Calcular el porcentaje de vida usando los datos de la interfaz.
        if (daniable.vidaMaxima <= 0) return; // Evitar división por cero.

        float porcentaje = Mathf.Clamp01((float)daniable.vidaActual / daniable.vidaMaxima);

        // 3. Escalar el ancho de la barra de vida visual.
        Vector3 escalaActual = BarraDeVidaVisual.transform.localScale;
        BarraDeVidaVisual.transform.localScale = new Vector3(anchoOriginal * porcentaje, escalaActual.y, escalaActual.z);

        // 4. Mover la barra localmente para que parezca que se vacía desde la derecha.
        float offset = (anchoOriginal - (anchoOriginal * porcentaje)) / 2f;
        BarraDeVidaVisual.transform.localPosition = new Vector3(-offset, BarraDeVidaVisual.transform.localPosition.y, BarraDeVidaVisual.transform.localPosition.z);
    }
}