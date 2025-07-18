using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A2_H3_Charco : A2_Trampa
{
    public GameObject agua;
    public GameObject hielo;
    public override void Activate()
    {
        base.Activate();
        // Agregar Logica de desactivar Sonido, Apariencia, Creacion etc

    }

    public override void Desactivar()
    {
        base.Desactivar();
        // Agregar Logica de desactivar Sonido, Apariencia, Creacion etc
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Hielo"))
        {
            hielo.SetActive(true);
            agua.SetActive(false);
            
        }
    }

    public override void OnCollisionEnter(Collision colision)
    {
       
        
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
