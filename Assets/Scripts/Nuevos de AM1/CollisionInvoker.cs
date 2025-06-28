using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionInvoker : MonoBehaviour
{
    [Header("Evento a invocar al colisionar con 'Jugador'")]
    public UnityEvent onJugadorTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Jugador 1"))
        {
            Debug.Log("Se registro entrada");
            onJugadorTrigger?.Invoke();
            Destroy(gameObject);
        }
    }
}
