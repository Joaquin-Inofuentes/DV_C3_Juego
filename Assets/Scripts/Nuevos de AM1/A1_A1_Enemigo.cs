using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class A1_A1_Enemigo : A1_Entidad
{


    /*
     AudioManager 
     
     * + rangoDeteccion : int
+ rangoAtaque : int
+ agente : NavMeshAgent
+ animator : Animator
- objetivoActual : Transform
+ {static} ListaDeEnemigosActivos : List<GameObject>




    -Start() : Void
-Update() : Void
+OnTriggerEnter(Collider) : Void
+MoverseAlDestino() : Void
+OnTriggerExit(Collider) : Void
<abstract> MoverseAlDestino() : void
<abstract>  OnEnabled() : void
<abstract>  OnDisabled() : void
     * 
     */
    // Público
    public int rangoDeteccion;
    public int rangoAtaque;
    public static List<GameObject> ListaDeEnemigosActivos = new List<GameObject>();

    // Privado
    public Transform objetivoActual;

    // Unity Methods
    protected virtual void Start()
    {
        ListaDeEnemigosActivos.Add(gameObject);
    }
    public Enemigo EstadoActual;
    protected virtual void Update()
    {
        if (EstadoActual == null)
        {
            EstadoActual = gameObject.GetComponent<Enemigo>();
        }
        // Comportamiento general a definir en clases hijas
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detectar entrada a trigger (enemigo, jugador, zona)
    }

    private void OnTriggerExit(Collider other)
    {
        // Salida de zona (ej: dejar de seguir al jugador)
    }

    // Métodos públicos

    public abstract void OnEnabled();
    public abstract void OnDisabled();
}
