using System;
using System.Collections.Generic;
using UnityEngine;

public class ATK_Disparador : MonoBehaviour
{
    // ⏱ Tiempo sin nuevas entradas antes de reportar
    public float tiempoEspera = 0.2f;

    // 📣 Evento que se lanza con la lista de colliders detectados
    public event Action<List<Collider>> AlReportar;

    // 📦 Lista de lo que entró
    private List<Collider> detectados = new List<Collider>();

    // 🕒 Última vez que alguien entró
    private float ultimoIngreso;

    void Update()
    {
        // Si hay detectados y pasó el tiempo sin nuevos ingresos
        if (detectados.Count > 0 && Time.time - ultimoIngreso > tiempoEspera)
        {
            // 🔔 Reporta a los suscritos
            AlReportar?.Invoke(new List<Collider>(detectados));

            // 💣 Se destruye automáticamente
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!detectados.Contains(other))
        {
            detectados.Add(other);
            ultimoIngreso = Time.time;
            Debug.Log("🟢 ATK_Disparador detectó: " + other.name);
        }
    }
}