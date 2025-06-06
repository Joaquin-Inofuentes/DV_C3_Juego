using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addColliders : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // ObtenerAudioPorNombre todos los objetos hijos en el GameObject
        foreach (Transform child in transform)
        {
            // Si no tiene un Collider, agregar uno
            if (child.GetComponent<Collider>() == null)
            {
                child.gameObject.AddComponent<BoxCollider>();
            }

            // Si no tiene un Rigidbody, agregar uno (opcional)
            if (child.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
                // Si solo quieres que el objeto no se mueva, pero aún tenga colisiones:
                rb.isKinematic = true;
            }
        }
    }
}
