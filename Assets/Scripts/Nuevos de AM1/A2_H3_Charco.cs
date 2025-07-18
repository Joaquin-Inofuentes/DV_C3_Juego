using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A2_H3_Charco : A2_Trampa
{
    public GameObject EfectoEspecial;
    //public ATK_Congelar EfectoDeCongelado;
    private void OnTriggerEnter(Collider other)
    {
        
    }

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

    public override void OnCollisionEnter(Collision collision)
    {
        if (!gameObject.activeSelf) return;
        if(!collision.gameObject.name.Contains("Jugador"))
        {

        A1_Entidad enemigo = collision.gameObject.GetComponent<A1_Entidad>();
        A1_A1_H1_MoustroDelAverno enemigo2 = enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>();
        if (enemigo2 != null && !enemigo2.Congelado)
        {
        GameObject efecto = Instantiate(EfectoEspecial, collision.transform.position, Quaternion.identity);
            enemigo2.Congelado = true;
            enemigo2.PrimerAtaqueAAnular = true;
            ATK_Congelar Componente = EfectoEspecial.GetComponent<ATK_Congelar>();
            Componente.padre = collision.transform;
            Componente.agent = enemigo.agent;
            Componente.anim = enemigo.anim;
            Destroy(efecto, 1);

        }
        }
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
