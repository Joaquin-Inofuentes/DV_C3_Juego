using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiezaHielo : MonoBehaviour
{
    public PuenteDeHielo puente;
    public GameObject piezaQueActiva;
    public GameObject Puente;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("1");
        if (other.gameObject.name.Contains("Hielo"))
        {
            Debug.Log("2");
            puente.ActivarPlataformaCercana(piezaQueActiva.transform.position);
            Puente.SetActive(true);
        }
    }

}