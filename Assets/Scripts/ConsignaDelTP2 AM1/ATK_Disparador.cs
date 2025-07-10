using System;
using System.Collections.Generic;
using UnityEngine;

public class ATK_Disparador : MonoBehaviour
{
    // ⏱ Tiempo a esperar para reportar (aunque no haya detectados)
    public float tiempoEspera = 0.2f;

    // 📣 Evento que se lanza con la lista de colliders detectados (puede estar vacía)
    public event Action<List<Collider>> AlReportar;

    // 📦 Lista de objetos detectados que entraron al trigger
    private List<Collider> detectados = new List<Collider>();

    // 🕒 Última vez que hubo actividad (entrada o creación)
    private float ultimoIngreso;

    void Start()
    {
        // Inicializamos el reloj desde el momento que se crea el objeto
        ultimoIngreso = Time.time;
    }

    void Update()
    {
        // Si pasó el tiempo de espera sin nueva actividad, reportamos y destruimos
        if (Time.time - ultimoIngreso > tiempoEspera)
        {
            AlReportar?.Invoke(new List<Collider>(detectados)); // Puede ser lista vacía
            Destroy(gameObject); // Destruye el trigger
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Si no estaba en la lista, lo agregamos y reiniciamos el reloj
        if (!detectados.Contains(other))
        {
            detectados.Add(other);
            ultimoIngreso = Time.time; // Reinicia el temporizador
            Debug.Log("🟢 ATK_Disparador detectó: " + other.name);
        }
    }
}