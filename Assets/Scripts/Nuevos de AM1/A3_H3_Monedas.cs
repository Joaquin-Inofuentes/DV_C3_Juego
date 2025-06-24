using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A3_H3_Monedas : A3_Interactuable
{
    public int CantidadDeMonedas;
    public override void Interactuar()
    {
     
        //SonidoDeInteraccion.Play();
    }

    public override void OnDestroy()
    {
        Debug.LogWarning("Falta efecto de conseguir monedas de las monedas");
    }

    // +CantidadDeMonedas : int
    protected override void Start()
    {
        base.Start(); // Llama al Start del padre
        // Código propio de ArquerasElfas
    }
    protected override void Update()
    {
        base.Update(); // Llama al Start del padre
        // Código propio de ArquerasElfas
    }
    public override void OnCollisionEnter(Collision collider)
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {

        AccionesJugador jugador = other.gameObject.GetComponent<AccionesJugador>();
        jugador.Feedbacks.FeedbackRadialVisual(jugador.Color_ObtieneMonedas, 1f);
        Interactuar();
        Destroy(gameObject);
    }
  
    /*public override void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.GetComponent<AccionesJugador>() != null)
        {
            Interactuar();
            collider.gameObject.GetComponent<AccionesJugador>().Feedbacks.FeedbackRadialVisual(
                collider.gameObject.GetComponent<AccionesJugador>().Color_ObtieneMonedas
                , 1f
                );
            Destroy(gameObject);
        }
    }*/
}
