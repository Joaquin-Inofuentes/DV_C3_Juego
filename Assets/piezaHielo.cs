using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiezaHielo : MonoBehaviour
{
    public PuenteDeHielo puente; // arrastra aquí el componente PuenteDeHielo

    private void OnCollisionEnter(Collision other)
    {
        // Por ejemplo, solo reaccionar si colisiona una bola o el jugador:
        if (other.gameObject.name.Contains("Hielo"))
        {
            Debug.Log($"{name} ha sido activada por {other.gameObject.name}");
            puente.ActivarPieza(this.gameObject); // método que haremos en PuenteDeHielo
        }
    }
}