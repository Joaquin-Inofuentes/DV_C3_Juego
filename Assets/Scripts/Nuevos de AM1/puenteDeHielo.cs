using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class PuenteDeHielo : MonoBehaviour
{
    [Tooltip("Referencia al GameObject del puente de hielo. Este debe iniciar INACTIVO.")]
    public GameObject puenteDeHielo;

    [Tooltip("Referencia al NavMeshSurface global del Terrain. Arrastra el componente NavMeshSurface de tu Terrain aquí.")]
    public NavMeshSurface globalNavMeshSurface;

    [Tooltip("El material que se aplicará al objeto del puente de hielo cuando se congele.")]
    public Material materialHielo;

    [Tooltip("El GameObject que contiene todos los NavMesh Obstacles (los 'huecos') que deben desactivarse cuando el río se congele.")]
    public GameObject huecos; // <--- ¡VARIABLE RENOMBRADA A 'huecos'!

    private bool isFrozen = false;

    void Awake()
    {
        if (globalNavMeshSurface == null)
        {
            Terrain terrain = FindObjectOfType<Terrain>();
            if (terrain != null)
            {
                globalNavMeshSurface = terrain.GetComponent<NavMeshSurface>();
            }

            if (globalNavMeshSurface == null)
            {
                Debug.LogError("PuenteDeHielo: NavMeshSurface GLOBAL del Terrain no asignado o no encontrado. ¡No se podrá actualizar el NavMesh!", this);
            }
        }

        if (puenteDeHielo == null)
        {
            Debug.LogError("PuenteDeHielo: El objeto 'puenteDeHielo' no ha sido asignado. ¡Por favor, arrastra el GameObject del puente de hielo al slot en el Inspector!", this);
        }
        else
        {
            puenteDeHielo.SetActive(false);
            Debug.Log("PuenteDeHielo: Objeto 'puenteDeHielo' establecido a inactivo en Awake.");
        }

        // Validación para la variable 'huecos'
        if (huecos == null) // <--- USANDO LA NUEVA VARIABLE
        {
            Debug.LogWarning("PuenteDeHielo: El GameObject 'huecos' no asignado. Asegúrate de que los NavMesh Obstacles correctos estén desactivados manualmente si es necesario.", this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"PuenteDeHielo: Colisión detectada con: {collision.gameObject.name}.");

        if (!isFrozen)
        {
            Debug.Log("PuenteDeHielo: Colisión recibida. Activando Puente.");
            ActivarPuente();

            // Opcional: destruir la bola de hielo.
            // Destroy(collision.gameObject);
        }
        else
        {
            Debug.Log("PuenteDeHielo: Ya congelado, ignorando colisión.");
        }
    }

    private void ActivarPuente()
    {
        if (isFrozen) return;

        
        if (huecos != null)
        {
            huecos.SetActive(false); 
            Debug.Log("PuenteDeHielo: GameObject 'huecos' (NavMesh Obstacles) desactivado.");
        }

        
        if (puenteDeHielo != null)
        {
            puenteDeHielo.SetActive(true);
            Debug.Log("PuenteDeHielo: Objeto 'puenteDeHielo' activado.");

          
            Renderer hieloRenderer = puenteDeHielo.GetComponent<Renderer>();
            if (hieloRenderer != null && materialHielo != null)
            {
                hieloRenderer.material = materialHielo;
                Debug.Log("PuenteDeHielo: Material de hielo aplicado.");
            }
            else
            {
                Debug.LogWarning("PuenteDeHielo: No se pudo aplicar el material de hielo. Asegúrate de que el objeto del puente de hielo tiene un Renderer y que el material está asignado.");
            }
        }
        else
        {
            Debug.LogError("PuenteDeHielo: El objeto 'puenteDeHielo' es nulo. No se pudo activar el puente.");
            return;
        }

       
        if (globalNavMeshSurface != null)
        {
            Debug.Log("PuenteDeHielo: Reconstruyendo NavMesh GLOBAL para incluir el puente de hielo...");
            globalNavMeshSurface.BuildNavMesh();
            Debug.Log("PuenteDeHielo: NavMesh GLOBAL actualizado.");
        }
        else
        {
            Debug.LogWarning("PuenteDeHielo: NavMeshSurface GLOBAL del Terrain no asignado. No se pudo reconstruir el NavMesh.");
        }

        isFrozen = true;
        Debug.Log("Puente de Hielo activado y NavMesh funcional.");

        
    }
}