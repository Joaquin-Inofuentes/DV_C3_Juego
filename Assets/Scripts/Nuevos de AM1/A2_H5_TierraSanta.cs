using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A2_H5_TierraSanta : A2_Trampa
{
    public GameObject Indicadores1;
    public GameObject Indicadores2;
    public GameObject Indicadores3;

    private void OnTriggerEnter(Collider other)
    {
        AccionesJugador jugador = other.GetComponent<AccionesJugador>();

        if (jugador != null && jugador._TimerManager != null)
        {
            // Bloquear magia dentro de la zona
            jugador._TimerManager.magiaBloqueadaPorZona = true;
            jugador._TimerManager.enModoMagico = false;

            // Forzar modo melee
            jugador.modoActual = ModoPelea.Melee;

            // Actualizar visual (anim layers, espada y HUD)
            jugador.ForzarModoMeleeVisual();

            // Indicadores visuales de la zona
            Indicadores1.SetActive(true);
            Indicadores2.SetActive(true);
            Indicadores3.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AccionesJugador jugador = other.GetComponent<AccionesJugador>();

        if (jugador != null && jugador._TimerManager != null)
        {
            // Desbloquear magia al salir
            jugador._TimerManager.magiaBloqueadaPorZona = false;

            // Dejar que el jugador controle el HUD otra vez (opcional: forzar refresco)
            jugador.ForzarRefrescoHUDAlSalirDeZona();

            Indicadores1.SetActive(false);
            Indicadores2.SetActive(false);
            Indicadores3.SetActive(false);
        }
    }



    public override void Activate()
    {
        base.Activate();
    }

    public override void Desactivar()
    {
        base.Desactivar();
        // Aquí podes agregar lógica adicional de desactivación
    }

    public override void OnCollisionEnter(Collision collider)
    {
        throw new System.NotImplementedException();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}



