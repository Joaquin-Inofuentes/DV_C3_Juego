using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorEnemigos : MonoBehaviour
{
    public GameObject enemigoPrefab;    
    public GameObject jefePrefab;       
    public Transform[] puntosSpawn;     

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
        while (!jefeGenerado)
        {
            if (enemigosActuales.Count < limiteEnemigos)
            {
                CrearEnemigo();
            }
            else if (!jefeGenerado)
            {
                CrearJefe();
                jefeGenerado = true;
            }

            yield return new WaitForSeconds(tiempoEntreSpawns);
        }
    }

    void CrearEnemigo()
    {
        Transform punto = puntosSpawn[Random.Range(0, puntosSpawn.Length)];
        GameObject enemigo = Instantiate(enemigoPrefab, punto.position, Quaternion.identity);
        enemigosActuales.Add(enemigo);

        enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>().Morir += () => enemigosActuales.Remove(enemigo);

        enemigosCreados++;
    }

    void CrearJefe()
    {
        Transform punto = puntosSpawn[Random.Range(0, puntosSpawn.Length)];
        Instantiate(jefePrefab, punto.position, Quaternion.identity);
    }
}