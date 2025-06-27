using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int danio;
    public AudioClip AudioTrasAtacarAAlguien;
    public AudioClip AudioAtaqueAlAire;
    public void Start()
    {

    }
    public bool AtacoAAlguien;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Jugador")) return;
        A1_Entidad enemigo = other.GetComponent<A1_Entidad>();
        if (enemigo != null)
        {
            enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>().ultimoProyectilRecibido = gameObject.name;
            enemigo.RecibirDanio(danio);
            AtacoAAlguien = true;
            Destroy(gameObject); // Solo golpea una vez
        }
    }
    public void OnDestroy()
    {
        if (AtacoAAlguien)
        {
            AudioManager.CrearEfectoSonoro(transform.position, AudioTrasAtacarAAlguien);
        }
        else
        {
            AudioManager.CrearEfectoSonoro(transform.position, AudioAtaqueAlAire);
        }
    }

    public void ConfigurarDanio(int cantidad)
    {
        danio = cantidad;
    }
}
