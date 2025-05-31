using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int danio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Jugador")) return;
        A1_Entidad enemigo = other.GetComponent<A1_Entidad>();
        if (enemigo != null)
        {
            enemigo.RecibirDanio(danio);
            Destroy(gameObject); // Solo golpea una vez
        }
    }


    public void ConfigurarDanio(int cantidad)
    {
        danio = cantidad;
    }
}
