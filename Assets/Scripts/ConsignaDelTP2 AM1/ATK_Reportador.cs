using System;
using System.Collections.Generic;
using UnityEngine;

public class ATK_Reportador : MonoBehaviour
{
    void Start()
    {
        CrearDisparador(
            new Vector3(0, 1, 0),  // Posición
            new Vector3(2, 2, 2)   // Tamaño
        );
    }

    public void CrearDisparador(Vector3 posicion, Vector3 tamaño)
    {
        GameObject obj = new GameObject("ATK_Disparador");
        obj.transform.position = posicion;

        BoxCollider col = obj.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = tamaño;

        ATK_Disparador disparador = obj.AddComponent<ATK_Disparador>();
        disparador.tiempoEspera = 0.2f;

        disparador.AlReportar += ProcesarResultado;
    }

    public void ProcesarResultado(List<Collider> detectados)
    {
        Debug.Log("✅ ATK_Reportador recibió " + detectados.Count + " objetos:");
        if (detectados.Count == 0)
            Debug.Log("⚠️ No se detectó ningún objeto");
        else
            foreach (var c in detectados)
                Debug.Log("👤 Detectado: " + c.name);
    }
}