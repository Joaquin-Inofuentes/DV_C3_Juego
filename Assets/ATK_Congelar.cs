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

    void Start()
    {
        if (anim != null) anim.speed = 0;
        if (this == null) return;
        if (padre == null) return; // Asegurarse de que padre no sea nulo
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

    void ReanudarAgente()
    {
        if (anim != null) anim.speed = 1;
        if (agent == null) return;
        agent.SetDestination(destinoGuardado);
        agent.isStopped = false;
        padre.GetComponent<A1_A1_H1_MoustroDelAverno>().RecibiraDobleDanoLaProximaVez = false; // Reanudar el estado del monstruo
    }



    void Update()
    {
        if (padre == null) return;

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
