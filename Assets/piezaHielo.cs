using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class PiezaHielo : MonoBehaviour
{
    public PuenteDeHielo puente;
    public GameObject piezaQueActiva;
    public GameObject Puente;
    public GameObject riocolision;
    public NavMeshObstacle obstacle;
    public float duracionPuente = 15f;
    private Coroutine coroutineDesactivar;

    void Start()
    {
        obstacle = GetComponent<NavMeshObstacle>();

        // Si no existe, lo agregamos
        if (obstacle == null)
        {
            Debug.LogWarning("NavMeshObstacle no encontrado en " + gameObject.name + ". Agregando uno.");
            obstacle = gameObject.AddComponent<NavMeshObstacle>();
            obstacle.carving = true; // opcional   
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("1");
        if (other.gameObject.name.Contains("Hielo"))
        {
            Debug.Log("2");
            puente.ActivarPlataformaCercana(piezaQueActiva.transform.position);
            riocolision.gameObject.SetActive(false);
            obstacle.enabled = false;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("Hielo"))
        {
            // Iniciamos la cuenta regresiva para desactivar el puente
            coroutineDesactivar = StartCoroutine(DesactivarPuenteDespuesDeTiempo());
            //other.gameObject.SetActive(true);
        }
    }

    private IEnumerator DesactivarPuenteDespuesDeTiempo()
    {
        yield return new WaitForSeconds(duracionPuente);

        Puente.SetActive(false);
        obstacle.enabled = true;
        riocolision.SetActive(true);
        coroutineDesactivar = null;
    }

}