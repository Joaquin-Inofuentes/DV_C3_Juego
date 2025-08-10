using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ataqueHielo : Iataque
{
    public void Ejecutar(AtaqueJugador jugador, Vector3 destino)
    {
        if (jugador.modoActual == AtaqueJugador.ModoPelea.Rango)
        {
            if (jugador._TimerManager.IsTimerCharging(1)) return;
            jugador.anim.SetTrigger("magic2");
            jugador._TimerManager.SetTimerToMax(1);
            jugador.InstanciarProyectil(jugador.BolaDeHielo, "BolaDeHielo", destino);
        }
        else
        {
            if (jugador._TimerManager.IsTimerCharging(4)) return;
            jugador.anim.SetTrigger("melee2");
            jugador._TimerManager.SetTimerToMax(4);
        }
    }
}
