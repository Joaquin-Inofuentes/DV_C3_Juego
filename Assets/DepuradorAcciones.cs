using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepuradorAcciones : MonoBehaviour
{
    // CICLO DE VIDA
    void Awake() { }
    void OnEnable() { }
    void OnDisable() { }
    void OnDestroy() { }
    // COLISIONES FÍSICAS
    void OnCollisionEnter(Collision col)
    {
        Debug.Log($"{name} [COLLISION ENTER] con {col.gameObject.name}", gameObject);
        Debug.DrawLine(transform.position, col.transform.position, Color.red, 1f);
    }

    void OnCollisionStay(Collision col)
    {
        Debug.Log($"{name} [COLLISION STAY] con {col.gameObject.name}", gameObject);
        Debug.DrawLine(transform.position, col.transform.position, new Color(1f, 0.5f, 0f), 1f); // Orange
    }

    void OnCollisionExit(Collision col)
    {
        Debug.Log($"{name} [COLLISION EXIT] con {col.gameObject.name}", gameObject);
        Debug.DrawLine(transform.position, col.transform.position, Color.magenta, 1f);
    }

    // TRIGGERS
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{name} [TRIGGER ENTER] con {other.name}", gameObject);
        Debug.DrawLine(transform.position, other.transform.position, Color.green, 1f);
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log($"{name} [TRIGGER STAY] con {other.name}", gameObject);
        Debug.DrawLine(transform.position, other.transform.position, Color.yellow, 1f);
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"{name} [TRIGGER EXIT] con {other.name}", gameObject);
        Debug.DrawLine(transform.position, other.transform.position, Color.blue, 1f);
    }


}
