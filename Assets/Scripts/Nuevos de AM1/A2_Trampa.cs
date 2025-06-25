using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TP2_Scrip hecho por Javier Espiñeira
public abstract class A2_Trampa : MonoBehaviour
{
    
    /*
     #activada: bool 
     <abstract> OnCollisionEnter(Coll Collider): void 
     * 
     */
    protected bool Activado;
    public virtual void Activate()
    {
        Activado = true;
    }
    public virtual void Desactivar()
    {
        Activado = false;
    }

    public abstract void OnCollisionEnter(Collision collider);


    protected virtual void Start()
    {
        // Código base de la trampa
    }

    protected virtual void Update()
    {
        // Código base de la trampa
    }

}
