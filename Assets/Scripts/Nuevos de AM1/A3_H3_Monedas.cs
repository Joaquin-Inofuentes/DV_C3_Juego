using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A3_H3_Monedas : A3_Interactuable
{
    public int CantidadDeMonedas = 5;
    public override void Interactuar()
    {
        GameManager.SumarMonedas(CantidadDeMonedas);
        //SonidoDeInteraccion.Play();
    }

    public void OnTriggerEnter(Collider collider)
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
        Debug.Log(collider.gameObject.name, gameObject);
        if (collider.gameObject.GetComponent<AccionesJugador>() != null)
        {
            Debug.Log(1);
            Interactuar();
            collider.gameObject.GetComponent<AccionesJugador>().Feedbacks.FeedbackRadialVisual(
                collider.gameObject.GetComponent<AccionesJugador>().Color_ObtieneMonedas
                , 1f
                );
            Destroy(gameObject);
        }
    }
}
