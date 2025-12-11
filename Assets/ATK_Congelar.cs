using UnityEngine;
using UnityEngine.AI;

public class ATK_Congelar : MonoBehaviour
{
    public float timer = 3f;

    public Animator anim;
    public Transform padre;
    public Vector3 posOriginal;
    public Quaternion rotOriginal;
    public float timerActual;
    public AudioSource S_Congelar;

    void Start()
    {
        // Si ya existe un hielo en el enemigo, destruir este duplicado
        var enemy = padre.GetComponent<A1_A1_H1_MoustroDelAverno>();
        if (enemy.EfectoDeCongelado != null && enemy.EfectoDeCongelado != this)
        {
            Destroy(gameObject);
            return;
        }

        // Registrar este hielo como el único efecto activo
        enemy.EfectoDeCongelado = this;

        Destroy(gameObject, timer);
        if (anim != null) anim.speed = 0;
        if (this == null) return;
        if (padre == null) return;
        if (S_Congelar != null) S_Congelar.Play();

        // Ajuste de altura del hielo SOLO para enemigos que lo necesiten
        float offset = padre.GetComponent<A1_A1_H1_MoustroDelAverno>().offsetCongelamiento;
        transform.position += Vector3.up * offset;

        agent = padre.GetComponent<A1_A1_H1_MoustroDelAverno>().agent;

        posOriginal = padre.position;
        rotOriginal = padre.rotation;
        timerActual = timer;
        Invoke("DetenerAgente", 0.1f);
    }


    public NavMeshAgent agent;
    public Vector3 destinoGuardado ;

    void DetenerAgente()
    {
        if (agent == null) return;
        //Debug.Log(agent.destination + " " + padre.transform.position);
        if (agent.destination == padre.transform.position) return;
        //Debug.Log("Se detuvo");
        destinoGuardado = agent.destination;
        agent.isStopped = true;
    }

    public void ReanudarAgente()
    {
        if (padre == null) return;
        if(padre.GetComponent<A1_A1_H1_MoustroDelAverno>() == null) return; // Asegurarse de que el padre tenga el componente necesario
        //Debug.Log("Se desactivo " + gameObject.name, gameObject);
        //Debug.Log(destinoGuardado +" " + agent);
        padre.GetComponent<A1_A1_H1_MoustroDelAverno>().RecibiraDobleDanoLaProximaVez = false; // Reanudar el estado del monstruo
        padre.GetComponent<A1_A1_H1_MoustroDelAverno>().Congelado = false; // Reanudar el estado del monstruo
        padre.GetComponent<A1_A1_H1_MoustroDelAverno>().EfectoDeCongelado = null; // Reanudar el estado del monstruo
        if (anim != null) anim.speed = 1;
        if (agent == null) return;
        agent.SetDestination(destinoGuardado);
        agent.isStopped = false;
        padre.GetComponent<A1_A1_H1_MoustroDelAverno>().PendienteDeCargaElectrica = false;
        padre.GetComponent<A1_A1_H1_MoustroDelAverno>().PrimerAtaqueAAnular = false;
        //Debug.Log("Persiguiendo a " + destinoGuardado, gameObject);
    }



    void Update()
    {
        if (padre == null) return;
        if (padre.GetComponent<A1_A1_H1_MoustroDelAverno>() == null) return; // Asegurarse de que el padre tenga el componente necesario
        if (padre.GetComponent<A1_A1_H1_MoustroDelAverno>().agent == null) return; // Asegurarse de que el padre tenga el componente necesario
        // Mantener posici�n y rotaci�n
        padre.position = posOriginal;
        padre.rotation = rotOriginal;

        // Contador
        timerActual -= Time.deltaTime;
        //Debug.Log(timerActual);
        if (timerActual <= 0)
        {
            Destroy(gameObject); // eliminar congelador
        }
        else
        {
            DetenerAgente(); // detener agente mientras se congela
        }
    }
    public void OnDestroy()
    {
        ReanudarAgente();
    }
}
