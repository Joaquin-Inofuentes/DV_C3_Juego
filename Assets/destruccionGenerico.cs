using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destruccionGenerico : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Hielo"))
        {
            Destroy(other.gameObject);
        }
    }
}
