using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiezaHielo : MonoBehaviour
{
    public PuenteDeHielo puente;
    public GameObject piezaQueActiva;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("1");
        if (other.gameObject.name.Contains("Hielo"))
        {
            Debug.Log("2");
            puente.ActivarPieza(piezaQueActiva);
        }
    }

}