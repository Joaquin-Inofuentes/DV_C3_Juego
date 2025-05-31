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
        if (this != null) return;
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
    }



    void Update()
    {
        if (padre == null) return;

        // Mantener posición y rotación
        padre.position = posOriginal;
        padre.rotation = rotOriginal;

        // Contador
        timerActual -= Time.deltaTime;
        if (timerActual <= 0)
        {
            Destroy(gameObject); // eliminar congelador
        }
    }
    public void OnDestroy()
    {
        ReanudarAgente();
    }
}
