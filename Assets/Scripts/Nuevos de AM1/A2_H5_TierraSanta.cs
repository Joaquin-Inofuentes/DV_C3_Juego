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

        Debug.Log("1");
        AccionesJugador jugador = other.GetComponent<AccionesJugador>();
        if (jugador != null && jugador._TimerManager != null)
        {
            if (!jugador.modoMelee)
            {
                Debug.Log(2);
                jugador._TimerManager.magiaBloqueadaPorZona = true;
                jugador.ModoMelee();

                Indicadores1.SetActive(true);
                Indicadores2.SetActive(true);
                Indicadores3.SetActive(true);
            }

        }
    }


    private void OnTriggerExit(Collider other)
    {
        AccionesJugador jugador = other.GetComponent<AccionesJugador>();
        if (jugador != null && jugador._TimerManager != null)
        {
            jugador._TimerManager.magiaBloqueadaPorZona = false;
            Indicadores1.SetActive(false);
        }
    }


    public override void Activate()
    {
        base.Activate();
    }

    public override void Desactivar()
    {
        base.Desactivar();
        // Agregar Logica de desactivar Sonido, Apariencia, Creacion etc
    }

    public override void OnCollisionEnter(Collision collider)
    {
        throw new System.NotImplementedException();
    }

    protected override void Start()
    {
        base.Start(); // Llama al Start del padre
                      // Código propio de ArquerasElfas
    }

    protected override void Update()
    {
        base.Update(); // Llama al Update del padre
                       // Código propio de ArquerasElfas
    }
}


