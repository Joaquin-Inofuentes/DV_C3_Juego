using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorEnemigos : MonoBehaviour
{
    public GameObject enemigoPrefab;
    public GameObject jefePrefab;
    public Transform[] puntosSpawn;
    public GameObject efectoPrefab;

    public float tiempoEntreSpawns = 5f;
    public int limiteEnemigos = 10;

    private int enemigosCreados = 0;
    private bool jefeGenerado = false;
    private List<GameObject> enemigosActuales = new List<GameObject>();

    void Start()
    {
        StartCoroutine(GenerarEnemigos());
    }

    IEnumerator GenerarEnemigos()
    {

        while (true)
        {
            if (enemigosActuales.Count < limiteEnemigos)
            {
                CrearEnemigo();
            }

            CrearJefe();
            yield return new WaitForSeconds(tiempoEntreSpawns);
        }
        /*for(int i = 0; i < 3; i++)
        {
            CrearJefe();
        }*/
    }

    void CrearEnemigo()
    {
        Transform punto = puntosSpawn[Random.Range(0, puntosSpawn.Length)];
        GameObject enemigo = Instantiate(enemigoPrefab, punto.position, Quaternion.identity);
        InvocarEfecto(punto.position);
        A1_A1_H1_MoustroDelAverno scriptEnemigo = enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>();
        scriptEnemigo.generador = this;

        enemigosActuales.Add(enemigo);
    }
    public void EliminarEnemigo(GameObject enemigo)
    {
        if (enemigosActuales.Contains(enemigo))
        {
            enemigosActuales.Remove(enemigo);
        }
    }
    public void InvocarEfecto(Vector3 posicion)
    {
        if (efectoPrefab != null)
        {
            GameObject efecto = Instantiate(efectoPrefab, posicion, Quaternion.identity);
            Destroy(efecto, 5f);
        }
    }

    void CrearJefe()
    {


        Transform punto = puntosSpawn[Random.Range(0, puntosSpawn.Length)];
        Instantiate(jefePrefab, punto.position, Quaternion.identity);


    }
}