using UnityEngine;

public class Reporte_OnTriggerEnter : MonoBehaviour
{
    public bool activarEvento = true;
    private AccionesJugador jugador;

    private void Awake()
    {
        jugador = FindObjectOfType<AccionesJugador>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!activarEvento) return;

        if (other.CompareTag("Jugador"))
        {
            jugador?.CrearEfectoDeCuracion();
            AutoDestruirse();
        }
    }

    public void AutoDestruirse()
    {
        Destroy(gameObject);
    }

    public void AutoDestruirseConTimer(float timer)
    {
        Destroy(gameObject, timer);
    }
}

