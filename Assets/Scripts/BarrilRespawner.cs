using UnityEngine;
using System.Collections;

public class BarrilRespawner : MonoBehaviour
{
    public GameObject prefabBarril;
    public float tiempoRespawn = 10f;

    private GameObject barrilActual;

    void Start()
    {
        Spawn();
    }

    void Spawn()    
    {
        barrilActual = Instantiate(prefabBarril, transform.position, transform.rotation);
        StartCoroutine(EsperarYRevisar());
    }

    IEnumerator EsperarYRevisar()
    {
        while (barrilActual != null)
            yield return null;

        yield return new WaitForSeconds(tiempoRespawn);
        Spawn();
    }
}
