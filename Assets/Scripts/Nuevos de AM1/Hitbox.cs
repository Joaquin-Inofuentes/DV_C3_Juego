using UnityEngine;
using System.Collections.Generic;

public class Hitbox : MonoBehaviour
{
    [Header("Configuración de Daño")]
    public int danio;

    [Header("Sonidos")]
    public AudioClip AudioTrasAtacarAAlguien;
    public AudioClip AudioAtaqueAlAire;

    [Header("Comportamiento del Hitbox")]
    [Tooltip("Si es true, el hitbox se destruye al impactar con el primer enemigo.")]
    public bool destruirAlImpactar = true;

    [Tooltip("Si es true, el hitbox tiene un tiempo de vida y se puede desactivar con el mouse. Ideal para auras o ataques persistentes.")]
    public bool AutoDesactivarse = false;

    [Tooltip("Tiempo en segundos que el hitbox permanecerá activo si AutoDesactivarse es true.")]
    public float TiempoDeVida = 3f;

    // --- Variables Internas ---
    private bool atacoAAlguien;
    private List<A1_Entidad> enemigosYaGolpeados = new List<A1_Entidad>();
    private AccionesJugador accionesJugadorAsociadas;

    void Awake()
    {
        // Optimizacion: buscar la referencia una sola vez.
        accionesJugadorAsociadas = FindObjectOfType<AccionesJugador>();
    }

    void OnEnable()
    {
        // Reiniciamos el estado cada vez que el hitbox se activa.
        atacoAAlguien = false;
        enemigosYaGolpeados.Clear();

        // ¡IMPORTANTE! La creación de efectos visuales fue eliminada de aquí.
        // El efecto visual debe ser parte del mismo prefab que contiene este script.
        // Instanciar desde OnEnable creaba un bucle infinito y causaba la fuga de memoria.
    }

    void Update()
    {
        // Esta lógica es para auras o ataques que duran un tiempo.
        if (AutoDesactivarse)
        {
            if (TiempoDeVida > 0f)
            {
                TiempoDeVida -= Time.deltaTime;
            }
            else
            {
                gameObject.SetActive(false); // Desactivar si el tiempo se agota.
            }

            // Esta lógica parece muy específica, considera si realmente la necesitas.
            if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
            {
                gameObject.SetActive(false);
            }
        }
    }
    public GameObject Creador;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Creador) return; 
        // Usar CompareTag es mucho más eficiente que comparar nombres.
        if (other.CompareTag("Jugador"))
        {
            return;
        }
        // Aplicar daño
        if (other.TryGetComponent<IDaniable>(out var daniable))
        {
            if(other.GetComponent<A1_A1_H1_MoustroDelAverno>() != null)
                other.GetComponent<A1_A1_H1_MoustroDelAverno>().ultimoProyectilRecibido = gameObject.name;
            daniable.RecibirDanio(danio); 
        }

        // Usamos TryGetComponent para eficiencia y seguridad.
        if (other.TryGetComponent<A1_Entidad>(out var enemigo))
        {
            // Si ya golpeamos a este enemigo con este hitbox, no hacemos nada.
            if (enemigosYaGolpeados.Contains(enemigo))
            {
                return;
            }


            // Asignar el nombre del proyectil para combos
            if (enemigo is A1_A1_H1_MoustroDelAverno moustro)
            {
                moustro.ultimoProyectilRecibido = gameObject.name;
            }

            atacoAAlguien = true;
            enemigosYaGolpeados.Add(enemigo); // Añadir a la lista para no volver a golpearlo.

            // Notificar a AccionesJugador para el sistema de combos.
            if (accionesJugadorAsociadas != null)
            {
                accionesJugadorAsociadas.AtaquesQImpactaron.Add(gameObject.name);
                accionesJugadorAsociadas.TimerDeCombos = 3f; // Reinicia el tiempo para el siguiente combo.
                Debug.Log($"[Hitbox] Ataque exitoso a {enemigo.name} con {gameObject.name}");
            }

            // Si el hitbox es de un solo uso, se destruye.
            if (destruirAlImpactar)
            {
                // Usamos Destroy(gameObject) para limpiar el objeto completo.
                Destroy(gameObject);
            }
        }
    }

    void OnDestroy()
    {
        // Se llama cuando el objeto se destruye, ya sea por impacto o por tiempo de vida.
        // Esto asegura que los sonidos se reproduzcan correctamente.
        if (gameObject.scene.isLoaded) // Previene errores al cerrar la escena
        {
            if (atacoAAlguien)
            {
                AudioManager.CrearEfectoSonoro(transform.position, AudioTrasAtacarAAlguien);
            }
            else
            {
                AudioManager.CrearEfectoSonoro(transform.position, AudioAtaqueAlAire);
            }
        }
    }

    public void ConfigurarDanio(int cantidad)
    {
        danio = cantidad;
    }
}