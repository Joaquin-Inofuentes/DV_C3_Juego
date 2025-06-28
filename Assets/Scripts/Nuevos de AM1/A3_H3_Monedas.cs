using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A3_H3_Monedas : A3_Interactuable
{
    public int CantidadDeMonedas;
    public override void Interactuar()
    {
       
    }
    public AudioClip SonidoDeInteraccionClip; // Sonido que se reproducirá al destruir la moneda
    public override void OnDestroy()
    {
        AudioManager.CrearEfectoSonoro(transform.position, SonidoDeInteraccionClip);
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
        if(other.gameObject.GetComponent<AccionesJugador>() == null) return; // Asegurarse de que el objeto que colisiona tenga el componente AccionesJugador
        Debug.Log("Moneda recogida por: " + other.gameObject.name);
        AccionesJugador jugador = other.gameObject.GetComponent<AccionesJugador>();
        jugador.SumarMonedas(CantidadDeMonedas);
        jugador.Feedbacks.FeedbackRadialVisual(jugador.Color_ObtieneMonedas, 1f);
        Destroy(gameObject);
    }
    
}
