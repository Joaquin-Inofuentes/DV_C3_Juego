using UnityEngine;

public class HitboxDeLobo : MonoBehaviour
{
    private Lobo lobo;

    private void Start()
    {
        lobo = GetComponentInParent<Lobo>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!lobo.estaAtacando) return;

        A1_Entidad entidad = other.GetComponent<A1_Entidad>();
        if (entidad != null && entidad != lobo)
        {
            entidad.GetComponent<IDaniable>().RecibirDanio(lobo.DañoDeAtaque);
            Debug.Log($"[Lobo Hitbox] Dañó a {entidad.name}");
        }
    }
}
