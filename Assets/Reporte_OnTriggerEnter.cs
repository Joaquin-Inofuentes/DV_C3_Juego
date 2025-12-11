using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class ColliderEvent : UnityEvent<Collider> { }

public class Reporte_OnTriggerEnter : MonoBehaviour
{
    public bool activarEvento = true;
    public ColliderEvent OnTriggerEnterEvent;

    void OnTriggerEnter(Collider other)
    {
        if (!activarEvento) return;
        if (other.CompareTag("Jugador"))
        {
            VerificarPrimerSuscriptor();
            Debug.Log($"[Trigger Enter] con {other.name}", gameObject);
            OnTriggerEnterEvent?.Invoke(other);
        }
    }
    //public AccionesJugador accionesJugadorAsociadas;
    void VerificarPrimerSuscriptor()
    {
        if (OnTriggerEnterEvent != null)
        {
            if (OnTriggerEnterEvent.GetPersistentEventCount() > 0)
            {
                var target = OnTriggerEnterEvent.GetPersistentTarget(0);
                if (target == null)
                {
                    var encontrados = FindObjectsOfType<AccionesJugador>();
                    Debug.Log($"Encontrados {encontrados.Length} objetos con Reporte_OnTriggerEnter:");
                    foreach (var obj in encontrados)
                    {
                        obj.CrearEfectoDeCuracion();
                    }
                    Debug.Log("El primer delegado es null");
                }
                else
                {
                    Debug.Log("Primer delegado válido: " + OnTriggerEnterEvent.GetPersistentMethodName(0));
                }
            }
            else
            {
                Debug.Log("No hay suscriptores");
            }
        }
        else
        {
            Debug.Log("No hay suscriptores");
        }
    }


    public void AutoDestruirse()
    {
        Destroy(gameObject);
    }

    public void AutoDestruirseConTimer(float Timer)
    {
        Destroy(gameObject, Timer);
    }
}
