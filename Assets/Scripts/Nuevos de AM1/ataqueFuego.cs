using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ataqueFuego : Iataque
{
    public void Ejecutar(AtaqueJugador jugador, Vector3 destino)
    {
        if (jugador.modoActual == AtaqueJugador.ModoPelea.Rango)
        {
            if (jugador._TimerManager.IsTimerCharging(0)) return;

            GameObject prefab = (jugador.TimerDeCombos > 0 &&
                                 jugador.AtaquesQImpactaron.Count == 2 &&
                                 jugador.AtaquesQImpactaron[0] == "Melee1" &&
                                 jugador.AtaquesQImpactaron[1] == "Melee2")
                ? jugador.PrefabDeComboFuego
                : jugador.BolaDeFuego;

            jugador.anim.SetTrigger("magic1");
            jugador._TimerManager.SetTimerToMax(0);
            jugador.InstanciarProyectil(prefab, "BolaDeFuego", destino);
        }
        else
        {
            if (jugador._TimerManager.IsTimerCharging(3)) return;
            jugador.anim.SetTrigger("melee1");
            jugador._TimerManager.SetTimerToMax(3);
        }
    }
}
