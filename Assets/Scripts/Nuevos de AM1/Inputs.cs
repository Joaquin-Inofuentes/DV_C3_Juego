using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    public GameManager C_GameManager;

    public KeyCode TeclaAtaque1 = KeyCode.Alpha1;
    public KeyCode TeclaAtaque2 = KeyCode.Alpha2;
    public KeyCode TeclaAtaque3 = KeyCode.Alpha3;
    public KeyCode TeclaCambioModo = KeyCode.M;
    public KeyCode botonPausa = KeyCode.Escape;

    public Vector3 movimientoMouse;

    public AccionesJugador C_AccionesJugador;
    public TimerManager C_TimerManager;

    public GameObject Menu;

    private bool enModoMagico = true;

    void Start()
    {
        if (C_TimerManager == null)
            Debug.LogWarning("[Inputs] No asignaste TimerManager en el Inspector.");
    }
    private Mensajes _tutorial;
    void Update()
    {
        if (Mensajes.Instance != null)
        {
            if (_tutorial == null)
                _tutorial = Mensajes.Instance;
            if (_tutorial.faseActual > 0)
            {
                if (_tutorial.faseActual == 2 && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
                {
                    Mensajes.Instance.SiguienteFase(); // Avanzar al primer mensaje del tutorial
                }
                Movimiento();
            }
            if (_tutorial.faseActual > 2)
            {
                Ataque();

                /*        
        "Bienvenido al tutorial. Aquí aprenderás los conceptos básicos del juego.",  // 0
        "Usa los clicks del mouse para moverte",  //1 
        "Ataca al enemigo con 1234",  // 2
        "Aprieta 4 y cambia a modo fisico",  // 3
        "¡Felicidades! Has completado el tutorial. Ahora estás listo para jugar." // 4
                 */


                // Reemplaza la línea seleccionada para permitir avanzar con 1, 2 o 3
                if (_tutorial.faseActual == 3 && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3)))
                {
                    Mensajes.Instance.SiguienteFase(); // Avanzar al primer mensaje del tutorial
                    return;
                }

                // Reemplaza la línea seleccionada para permitir avanzar con 1, 2 o 3
                if (_tutorial.faseActual == 4 && Input.GetKeyDown(KeyCode.Alpha4))
                {
                    Mensajes.Instance.SiguienteFase(); // Avanzar al primer mensaje del tutorial
                    return;
                }
                if (_tutorial.faseActual != 4)
                {
                    // Reemplaza la línea seleccionada para permitir avanzar con 1, 2 o 3
                    if (_tutorial.faseActual == 5 && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3)))
                    {
                        Mensajes.Instance.SiguienteFase(); // Avanzar al primer mensaje del tutorial
                        _tutorial.textoTutorial.gameObject.transform.parent.gameObject.SetActive(false);
                        return;
                    }
                }
            }
            Pausa();
            Menu.SetActive(Time.timeScale == 0); // Mostrar menú si está en pausa
        }
    }

    public void Movimiento()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            C_AccionesJugador.IrAlDestino(GameManager.PosicionDelMouseEnElEspacio);
        }
    }

    public void Ataque()
    {
        // ATAQUE 1
        if (Input.GetKeyDown(TeclaAtaque1) &&
            !Input.GetKey(TeclaAtaque2) &&
            !Input.GetKey(TeclaAtaque3) &&
            !Input.GetKey(TeclaCambioModo))
        {
            EjecutarAtaque(enModoMagico ? 0 : 3, enModoMagico ? "BolaDeFuego" : "Melee1");
        }

        // ATAQUE 2
        else if (Input.GetKeyDown(TeclaAtaque2) &&
                 !Input.GetKey(TeclaAtaque1) &&
                 !Input.GetKey(TeclaAtaque3) &&
                 !Input.GetKey(TeclaCambioModo))
        {
            EjecutarAtaque(enModoMagico ? 1 : 4, enModoMagico ? "BolaDeHielo" : "Melee2");
        }

        // ATAQUE 3
        else if (Input.GetKeyDown(TeclaAtaque3) &&
                 !Input.GetKey(TeclaAtaque1) &&
                 !Input.GetKey(TeclaAtaque2) &&
                 !Input.GetKey(TeclaCambioModo))
        {
            EjecutarAtaque(enModoMagico ? 2 : 5, enModoMagico ? "RayoElectrico" : "Melee3");
        }

        // CAMBIO DE MODO
        else if (Input.GetKeyDown(TeclaCambioModo) &&
                 !Input.GetKey(TeclaAtaque1) &&
                 !Input.GetKey(TeclaAtaque2) &&
                 !Input.GetKey(TeclaAtaque3))
        {
            enModoMagico = !enModoMagico;
            Debug.Log("Modo cambiado a " + (enModoMagico ? "mágico" : "melee"));
        }
    }
    private void EjecutarAtaque(int index, string nombreAtaque)
    {
        if (C_TimerManager.IsTimerCharging(index))
        {
            C_TimerManager.MostrarFeedbackNoDisponible(index);
        }
        else
        {
            Debug.Log("Ejecutando ataque: " + nombreAtaque + " (índice: " + index + ")");
            C_AccionesJugador.Atacar(GameManager.PosicionDelMouseEnElEspacio, nombreAtaque);
            C_TimerManager.SetTimerToMax(index);
        }
    }


    public void Pausa()
    {
        if (Input.GetKeyDown(botonPausa))
        {
            Time.timeScale = Time.timeScale == 1 ? 0 : 1;
        }
    }

    public void ReanudarJuego()
    {
        Time.timeScale = 1;
    }

    public void SalirDelJuego()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("Saliendo del juego");
    }
}
