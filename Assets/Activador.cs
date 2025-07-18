using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activador : MonoBehaviour
{
    public GameObject hielo;
    public GameObject agua;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Hielo"))
        {
            hielo.SetActive(true);
            agua.SetActive(false);

        }

    }
}
