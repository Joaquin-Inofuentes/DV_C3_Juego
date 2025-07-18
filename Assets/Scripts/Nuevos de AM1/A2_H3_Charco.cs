using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A2_H3_Charco : A2_Trampa
{
    public GameObject EfectoEspecial;
    //public ATK_Congelar EfectoDeCongelado;
    public override void OnTriggerEnter(Collider other)
    {
        if (!gameObject.activeSelf) return;
        if (!other.gameObject.name.Contains("Jugador"))
        {
            A1_Entidad enemigo = other.gameObject.GetComponent<A1_Entidad>();
            if (enemigo == null) return;

            A1_A1_H1_MoustroDelAverno enemigo2 = enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>();
            if (enemigo2 != null && !enemigo2.Congelado)
            {
                GameObject efecto = Instantiate(EfectoEspecial, other.transform.position, Quaternion.identity);
                enemigo2.Congelado = true;
                enemigo2.PrimerAtaqueAAnular = true;

                ATK_Congelar Componente = efecto.GetComponent<ATK_Congelar>();  // Ojo: cambiar EfectoEspecial por efecto
                Componente.padre = other.transform;
                Componente.agent = enemigo.agent;
                Componente.anim = enemigo.anim;

                Destroy(efecto, 1f);
            }
        }
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
