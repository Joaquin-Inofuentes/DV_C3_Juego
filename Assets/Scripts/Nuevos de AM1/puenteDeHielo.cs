using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuenteDeHielo : MonoBehaviour
{
    public float duracionPlataforma = 15f;
    public float duracionTransicion = 0.3f;
    public float retardoEntreBloques = 0.1f;

    void Start()
    {
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
            StartCoroutine(AparecerBloquesUnoPorUno(plataformaCercana));
        }
    }

    private IEnumerator AparecerBloquesUnoPorUno(Transform plataforma)
    {
        // Guardamos escalas originales por bloque
        Dictionary<Transform, Vector3> escalasOriginales = new Dictionary<Transform, Vector3>();

        foreach (Transform bloque in plataforma)
        {
            escalasOriginales[bloque] = bloque.localScale;
            bloque.gameObject.SetActive(false);
            bloque.localScale = Vector3.zero;
        }

        yield return null; // espera un frame

        foreach (Transform bloque in plataforma)
        {
            bloque.gameObject.SetActive(true);
            Vector3 escalaFinal = escalasOriginales[bloque];

            float t = 0f;
            while (t < duracionTransicion)
            {
                t += Time.deltaTime;
                float factor = t / duracionTransicion;
                bloque.localScale = Vector3.Lerp(Vector3.zero, escalaFinal, factor);
                yield return null;
            }

            bloque.localScale = escalaFinal;

            yield return new WaitForSeconds(retardoEntreBloques);
        }

        // Esperar antes de ocultar todo
        yield return new WaitForSeconds(duracionPlataforma);
        plataforma.gameObject.SetActive(false);
    }

    private void DesactivarTodasLasPlataformas()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
