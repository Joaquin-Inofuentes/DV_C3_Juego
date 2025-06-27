using System.Linq;
using UnityEngine;

public class NotificadorAnimacion : StateMachineBehaviour
{
    private string ObtenerNombre(int hash, Animator animator)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (Animator.StringToHash(clip.name) == hash)
                return clip.name;
        }
        return "";
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var nombre = ObtenerNombre(stateInfo.shortNameHash, animator);
        //Debug.Log($"Iniciando animación: {nombre}");
        animator.GetComponent<ReceptorEventosAnim>()?.LlamarEvento(nombre, "inicio");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var nombre = ObtenerNombre(stateInfo.shortNameHash, animator);
        animator.GetComponent<ReceptorEventosAnim>()?.LlamarEvento(nombre, "update");
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log(layerIndex + animator.name);
        var nombre = ObtenerNombre(stateInfo.shortNameHash, animator);
        //Debug.Log($"Finalizando animación: {nombre}");
        if(animator.name != "GEO_Arquera")
            animator.GetComponent<ReceptorEventosAnim>()?.LlamarEvento(nombre, "fin");
        else
        {
            nombre = animator.GetCurrentAnimatorClipInfo(layerIndex)[0].clip.name;
            animator.GetComponent<ReceptorEventosAnim>()?.LlamarEvento(nombre, "fin");
            //animator.GetComponent<ReceptorEventosAnim>()?.LlamarEvento("boss_ataque1", "fin");
        }
    }
}
