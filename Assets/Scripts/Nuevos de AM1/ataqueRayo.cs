using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ataqueRayo : Iataque
{
    public void Ejecutar(AtaqueJugador jugador, Vector3 destino)
    {
        if (jugador.modoActual == AtaqueJugador.ModoPelea.Rango)
        {
            if (jugador._TimerManager.IsTimerCharging(2)) return;

            GameObject prefab = (jugador.TimerDeCombos > 0 &&
                                 jugador.AtaquesQImpactaron.Count == 2 &&
                                 jugador.AtaquesQImpactaron[0] == "Melee3" &&
                                 jugador.AtaquesQImpactaron[1] == "Melee2")
                ? jugador.PrefabDeComboElectrico
                : jugador.Rayo;

            jugador.anim.SetTrigger("magic3");
            jugador._TimerManager.SetTimerToMax(2);
            jugador.InstanciarProyectil(prefab, "RayoElectrico", destino);
        }
        else
        {
            if (jugador._TimerManager.IsTimerCharging(5)) return;
            jugador.anim.SetTrigger("melee3");
            jugador._TimerManager.SetTimerToMax(5);
        }
    }
}