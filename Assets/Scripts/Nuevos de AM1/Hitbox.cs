using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int danio;
    public AudioClip AudioTrasAtacarAAlguien;
    public AudioClip AudioAtaqueAlAire;

    public bool AutoDesactivarse = false; // Si se destruye al golpear a alguien o no
    public float TiempoDeVida = 3f; // Tiempo de vida del proyectil
    public void Update()
    {
        if (TiempoDeVida > 0f)
        {
            TiempoDeVida -= Time.deltaTime;
        }
        if (TiempoDeVida <= 0f && AutoDesactivarse)
        {
            gameObject.SetActive(false);
        }
        if (AutoDesactivarse)
        {
            if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
            {
                gameObject.SetActive(false); // Desactiva el hitbox si se mantiene presionado el botón del mouse
            }
        }
    }
    public bool AtacoAAlguien;

    public List<A1_Entidad> enemigosAsociados = new List<A1_Entidad>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Jugador")) return;


        if (AutoDesactivarse == true)
        {
            Debug.Log(other.name + " " + other.tag, gameObject);
            A1_A1_H1_MoustroDelAverno enemigo2 = other.GetComponent<A1_A1_H1_MoustroDelAverno>();
            if (enemigo2)
            {

                enemigo2.GetComponent<A1_A1_H1_MoustroDelAverno>().ultimoProyectilRecibido = gameObject.name;
                if (enemigosAsociados.Contains(enemigo2)) return; // Evita que el mismo enemigo reciba daño varias veces
                enemigo2.GetComponent<A1_A1_H1_MoustroDelAverno>().ultimoProyectilRecibido = gameObject.name;
                enemigo2.RecibirDanio(danio);
                AudioManager.CrearEfectoSonoro(transform.position, AudioTrasAtacarAAlguien);
            }
            return;
        }

        A1_Entidad enemigo = other.GetComponent<A1_Entidad>();
        if (enemigo != null)
        {



            enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>().ultimoProyectilRecibido = gameObject.name;
            enemigo.RecibirDanio(danio);
            AtacoAAlguien = true;
            AccionesJugador accionesJugador = FindObjectOfType<AccionesJugador>()?.gameObject.GetComponent<AccionesJugador>();
            if (accionesJugador != null)
            {
                accionesJugador.AtaquesQImpactaron.Add(gameObject.name);
                accionesJugador.TimerDeCombos = 3;
                Debug.Log($"[Hitbox] Ataque exitoso a {enemigo.name} con {gameObject.name}");
            }
            else
            {
                Debug.Log($"[Hitbox] Ataque exitoso a {enemigo.name} sin AccionesJugadorAsociadas con {gameObject.name}");
            }
            Destroy(gameObject); // Solo golpea una vez
        }
    }


    public void OnDisable()
    {
        if (AutoDesactivarse)
        {
            enemigosAsociados.Clear(); // Limpia la lista de enemigos asociados al desactivar el hitbox
        }
    }

    public void OnDestroy()
    {
        Debug.Log($"[Hitbox] Destruyendo hitbox {gameObject.name}");
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
