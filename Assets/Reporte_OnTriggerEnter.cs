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

        Debug.Log($"[Trigger Enter] con {other.name}");
        OnTriggerEnterEvent?.Invoke(other);
    }
}
