using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private Lobo lobo;

    void Start()
    {
        lobo = GetComponentInParent<Lobo>();
    }

    public void AplicarDanio()
    {
        if (lobo != null)
        {
            lobo.AplicarDanio();
        }
    }
    public void EventoDeAnimacionEspecial()
    {
        if (lobo != null)
        {
            lobo.EventoDeAnimacionEspecial();
        }
    }
}
