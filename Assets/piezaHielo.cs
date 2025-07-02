using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using Unity.VisualScripting;

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

      
        if (obstacle == null)
        {
            
            obstacle = gameObject.AddComponent<NavMeshObstacle>();
            obstacle.carving = true; 
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        
        if (other.gameObject.name.Contains("Hielo"))
        {
            
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