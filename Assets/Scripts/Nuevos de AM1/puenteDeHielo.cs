using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections.Generic;
using System.Collections;


public class PuenteDeHielo : MonoBehaviour
{
    [Header("Configuración")]
    public float duracionHielo = 15f;
    public NavMeshSurface navMeshSurface; // Arrastra el componente del padre aquí

    private void Start()
    {
        // Asegurarse de que todas las piezas empiezan inactivas
        foreach (Transform hijo in transform)
        {
            hijo.gameObject.SetActive(false);
        }
    }

    public void ActivarPieza(GameObject pieza)
    {
        if (!pieza.activeSelf)
        {
            pieza.SetActive(true);
            if (navMeshSurface != null) navMeshSurface.BuildNavMesh();
            StartCoroutine(DesactivarPieza(pieza));
        }
    }


    private IEnumerator DesactivarPieza(GameObject pieza)
    {
        Debug.Log("5");
        yield return new WaitForSeconds(duracionHielo);
        pieza.SetActive(false);

        // Reconstruye NavMesh al desactivarse
        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();
    }
}