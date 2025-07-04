using UnityEngine;

public class CodigoGenerico : MonoBehaviour
{
    public AudioClip Sonido;
    public Dialogos dialogosManager;  // <-- esta es la referencia al script Dialogos

    void Start()
    {
        if (dialogosManager == null)
            dialogosManager = FindObjectOfType<Dialogos>();  // busca el Dialogos en la escena
    }

    public void EjecutarSonido()
    {
        AudioManager.CrearEfectoSonoro(transform.position, Sonido, false, 0.3f);

        // Le decís al Dialogos que muestre el texto del clip que acabás de reproducir
        if (dialogosManager != null)
        {
            dialogosManager.ReproducirDialogo(Sonido.name);
        }
        else
        {
            Debug.LogWarning("Dialogos manager no asignado");
        }

        Destroy(gameObject);
    }
}

