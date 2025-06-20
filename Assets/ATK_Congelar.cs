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
        if (anim != null) anim.speed = 0;
        if (this == null) return;
        if (padre == null) return; // Asegurarse de que padre no sea nulo
        if (S_Congelar != null) S_Congelar.Play(); // Reproducir sonido de congelar
        agent = padre.GetComponent<A1_A1_H1_MoustroDelAverno>().agent; // Obtener el agente del monstruo
        posOriginal = padre.position;
        rotOriginal = padre.rotation;
        timerActual = timer;
        Invoke("DetenerAgente", 0.1f);
    }

    public NavMeshAgent agent;
    public Vector3 destinoGuardado;

    void DetenerAgente()
    {
        if (agent == null) return;

        destinoGuardado = agent.destination;
        agent.isStopped = true;
    }

    public void ReanudarAgente()
    {
        Debug.Log("Se desactivo " + gameObject.name, gameObject);
        padre.GetComponent<A1_A1_H1_MoustroDelAverno>().RecibiraDobleDanoLaProximaVez = false; // Reanudar el estado del monstruo
        padre.GetComponent<A1_A1_H1_MoustroDelAverno>().Congelado = false; // Reanudar el estado del monstruo
        padre.GetComponent<A1_A1_H1_MoustroDelAverno>().EfectoDeCongelado = null; // Reanudar el estado del monstruo
        if (anim != null) anim.speed = 1;
        if (agent == null) return;
        agent.SetDestination(destinoGuardado);
        agent.isStopped = false;
    }



    void Update()
    {
        if (padre == null) return;
        if( padre.GetComponent<A1_A1_H1_MoustroDelAverno>() == null) return; // Asegurarse de que el padre tenga el componente necesario
        if (padre.GetComponent<A1_A1_H1_MoustroDelAverno>().agent == null) return; // Asegurarse de que el padre tenga el componente necesario
        if (padre.GetComponent<A1_A1_H1_MoustroDelAverno>().agent.isStopped)
        {
            ReanudarAgente(); // Reanudar el agente si est�� detenido
            return; // Asegurarse de que el padre tenga el componente necesario
        }
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
