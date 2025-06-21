using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;

public class PuenteDeHielo : MonoBehaviour
{
    public float duracionPlataforma = 15f;
    private NavMeshSurface navMeshSurface;

    void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        DesactivarTodasLasPlataformas();
    }

    public void ActivarPlataformaCercana(Vector3 posicionImpacto)
    {
        Transform plataformaCercana = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (Transform child in transform)
        {
            float distancia = Vector3.Distance(child.position, posicionImpacto);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                plataformaCercana = child;
            }
        }

        if (plataformaCercana != null)
        {
            plataformaCercana.gameObject.SetActive(true);
            if (navMeshSurface != null)
                navMeshSurface.BuildNavMesh(); // Reconstruye el NavMesh

            StartCoroutine(DesactivarPlataforma(plataformaCercana.gameObject));
        }
    }

    private IEnumerator DesactivarPlataforma(GameObject plataforma)
    {
        yield return new WaitForSeconds(duracionPlataforma);
        plataforma.SetActive(false);
        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();
    }

    private void DesactivarTodasLasPlataformas()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}