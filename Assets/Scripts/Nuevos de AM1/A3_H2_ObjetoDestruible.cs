using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInspector;
public class A3_H2_ObjetoDestruible : A3_Interactuable
{
    [Button(nameof(Interactuar))]
    public GameObject Monedas;
    public override void Interactuar()
    {
        SonidoDeInteraccion.Play();
        Monedas.SetActive(true);
        gameObject.SetActive(false);
    }

    public override void OnCollisionEnter(Collision collider)
    {
        Debug.Log(collider);
        if (collider.gameObject.layer == 9)
            Interactuar();
    }

    public override void OnDestroy()
    {
        Debug.LogWarning("Falta accion tras conseguir esto " + gameObject.name);
    }

    /*
+MonedasASoltar : int

+Vida : int

SoltarMonedas() : void
RecibirDano() : void
*/
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
}
