using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    public GameManager C_GameManager;

    // Propiedades privadas
    public KeyCode TeclaAtaque1 = KeyCode.Alpha1;
    public KeyCode TeclaAtaque2 = KeyCode.Alpha2;
    public KeyCode TeclaAtaque3 = KeyCode.Alpha3;
    public KeyCode TeclaCambioModo = KeyCode.M;
    public KeyCode botonPausa = KeyCode.Escape;

    public Vector3 movimientoMouse;

    public AccionesJugador C_AccionesJugador;
    public TimerManager C_TimerManager;  // <-- Referencia al TimerManager

    public GameObject Menu;

    private bool enModoMagico = true;

    void Update()
    {
        Movimiento();
        Ataque();
        Pausa();
        Menu.SetActive(Time.timeScale == 0); // Alternar entre pausa y reanudación
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
        // Ataque 1: Bola de Fuego, índice 0
        if (Input.GetKeyDown(TeclaAtaque1) &&
            !Input.GetKey(TeclaAtaque2) &&
            !Input.GetKey(TeclaAtaque3) &&
            !Input.GetKey(TeclaCambioModo))
        {
            if (C_TimerManager.IsTimerCharging(0))
            {
                C_TimerManager.MostrarFeedbackNoDisponible(0);
            }
            else
            {
                C_AccionesJugador.Atacar(GameManager.PosicionDelMouseEnElEspacio, "BolaDeFuego");
                C_TimerManager.SetTimerToMax(0);
                Debug.Log("Ataque 1 ejecutado");
            }
        }
        // Ataque 2: Bola de Hielo, índice 1
        else if (Input.GetKeyDown(TeclaAtaque2) &&
                 !Input.GetKey(TeclaAtaque1) &&
                 !Input.GetKey(TeclaAtaque3) &&
                 !Input.GetKey(TeclaCambioModo))
        {
            if (C_TimerManager.IsTimerCharging(1))
            {
                C_TimerManager.MostrarFeedbackNoDisponible(1);
            }
            else
            {
                C_AccionesJugador.Atacar(GameManager.PosicionDelMouseEnElEspacio, "BolaDeHielo");
                C_TimerManager.SetTimerToMax(1);
            }
        }
        // Ataque 3: Rayo, índice 2
        else if (Input.GetKeyDown(TeclaAtaque3) &&
                 !Input.GetKey(TeclaAtaque1) &&
                 !Input.GetKey(TeclaAtaque2) &&
                 !Input.GetKey(TeclaCambioModo))
        {
            if (C_TimerManager.IsTimerCharging(2))
            {
                C_TimerManager.MostrarFeedbackNoDisponible(2);
            }
            else
            {
                C_AccionesJugador.Atacar(GameManager.PosicionDelMouseEnElEspacio, "Rayo");
                C_TimerManager.SetTimerToMax(2);
                Debug.Log("Ataque 3 ejecutado");
            }
        }
        // Cambio de modo
        else if (Input.GetKeyDown(TeclaCambioModo) &&
                 !Input.GetKey(TeclaAtaque1) &&
                 !Input.GetKey(TeclaAtaque2) &&
                 !Input.GetKey(TeclaAtaque3))
        {
            Debug.Log("Modo cambiado");
        }
    }

    public void Pausa()
    {
        if (Input.GetKeyDown(botonPausa))
        {
            Time.timeScale = Time.timeScale == 1 ? 0 : 1;
        }
    }

    void Start()
    {
        if (C_TimerManager == null)
            Debug.LogWarning("[Inputs] No asignaste TimerManager en el Inspector.");
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
