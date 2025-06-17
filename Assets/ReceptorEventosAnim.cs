using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReceptorEventosAnim : MonoBehaviour
{
    [System.Serializable]
    public class EventoPorNombre
    {
        public string NombreDelClipDeLaAnimacion;
        public UnityEvent AlIniciar;
        public UnityEvent MientrasSeReproduce;
        public UnityEvent AlFinalizar;
    }

    public List<EventoPorNombre> EventosAnimaciones = new();

    public void LlamarEvento(string nombre, string tipo)
    {
        Debug.Log($"Llamando evento '{tipo}' para la animación '{nombre}' en {gameObject.name}");
        foreach (var evento in EventosAnimaciones)
        {
            if (evento.NombreDelClipDeLaAnimacion == nombre)
            {
                switch (tipo)
                {
                    case "inicio": evento.AlIniciar?.Invoke(); break;
                    case "update": evento.MientrasSeReproduce?.Invoke(); break;
                    case "fin": evento.AlFinalizar?.Invoke(); break;
                }
                break;
            }
        }
    }
}
