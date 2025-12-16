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
    public int minimoLobos = 3;
    public int maximoLobos = 3;

    private List<GameObject> lobosActuales = new List<GameObject>();

    private List<GameObject> enemigosActuales = new List<GameObject>();

    void Start()
    {
        StartCoroutine(GenerarEnemigos());
    }

    IEnumerator GenerarEnemigos()
    {

        while (true)
        {
            int totalEnEscena = enemigosActuales.Count + lobosActuales.Count;

            // Asegurar mínimo de lobos (sin bloquear el resto)
            if (lobosActuales.Count < minimoLobos && totalEnEscena < limiteEnemigos)
            {
                CrearLobo();
                totalEnEscena++;
            }

            // Spawnear enemigos normales en paralelo
            if (totalEnEscena < limiteEnemigos)
            {
                CrearEnemigo();
            }

            yield return new WaitForSeconds(tiempoEntreSpawns);
        }

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
            enemigosActuales.Remove(enemigo);

        if (lobosActuales.Contains(enemigo))
            lobosActuales.Remove(enemigo);
    }

    public void InvocarEfecto(Vector3 posicion)
    {
        if (efectoPrefab != null)
        {
            GameObject efecto = Instantiate(efectoPrefab, posicion, Quaternion.identity);
            Destroy(efecto, 5f);
        }
    }

    void CrearLobo()
    {
        if (lobosActuales.Count >= maximoLobos) return;

        Transform punto = puntosSpawn[Random.Range(0, puntosSpawn.Length)];
        GameObject lobo = Instantiate(jefePrefab, punto.position, Quaternion.identity);

        lobosActuales.Add(lobo);

        var script = lobo.GetComponent<A1_A1_H1_MoustroDelAverno>();
        if (script != null)
            script.generador = this;
    }

}