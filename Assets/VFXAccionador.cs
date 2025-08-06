using System.Collections;
using UnityEngine;

public class VFXAccionador : MonoBehaviour
{
    [Header("🔧 Configuraciones")]
    public ParticleSystem particulas;
    public int cantidadDeDano = 10;

    void Start()
    {
        if (particulas != null)
        {
            float tiempoParaDestruir = particulas.main.duration + particulas.main.startLifetime.constant;
            Destroy(gameObject, tiempoParaDestruir);
        }
    }

    public void RecibioUnaColision(Collider other)
    {
        if (other.name == "Jugador 1") return; // Ignorar colisiones con el jugador
        Debug.Log($"[VFXAccionador] Colisionó con: {other.name}");

        IDaniable entidad = other.GetComponent<IDaniable>();
        if (entidad != null)
        {
            Debug.Log($"[VFXAccionador] Se aplicó {cantidadDeDano} de daño a {other.name}");
            entidad.RecibirDanio(cantidadDeDano);
        }
    }
}
