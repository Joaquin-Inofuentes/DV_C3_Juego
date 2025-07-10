using System;
using System.Collections.Generic;
using UnityEngine;

public class ATK_Reportador : MonoBehaviour
{
    void Start()
    {
        // ▶️ Disparar trigger en el mundo
        CrearDisparador(
            new Vector3(0, 1, 0),   // Posición
            new Vector3(2, 2, 2)    // Tamaño del área de detección
        );
    }

    public void CrearDisparador(Vector3 posicion, Vector3 tamaño)
    {
        // 🎮 Crea GameObject dinámico
        GameObject obj = new GameObject("ATK_Disparador");
        obj.transform.position = posicion;

        // ➕ BoxCollider tipo trigger
        BoxCollider col = obj.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = tamaño;

        // ➕ Lógica del disparador
        ATK_Disparador disparador = obj.AddComponent<ATK_Disparador>();
        disparador.tiempoEspera = 0.2f;

        // 📡 Escucha el evento
        disparador.AlReportar += ProcesarResultado;
    }

    // 🧠 Qué hacer con la lista de colliders detectados
    public void ProcesarResultado(List<Collider> detectados)
    {
        Debug.Log("✅ ATK_Reportador recibió " + detectados.Count + " objetos:");
        foreach (var c in detectados)
        {
            Debug.Log("👤 Detectado: " + c.name);
        }
    }
}