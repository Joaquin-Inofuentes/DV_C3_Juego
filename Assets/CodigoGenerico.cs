using UnityEngine;

public class CodigoGenerico : MonoBehaviour
{
    public AudioClip Sonido;
    public Dialogos dialogosManager;  

    void Start()
    {
        if (dialogosManager == null)
            dialogosManager = FindObjectOfType<Dialogos>();  
    }

    public void EjecutarSonido()
    {
        Destroy(gameObject,Sonido.length);
        GetComponent<BoxCollider>().enabled = false; 
        AudioManager.CrearEfectoSonoro(transform.position, Sonido, false, 0.3f);

       
        if (dialogosManager != null)
        {
            dialogosManager.ReproducirDialogo(Sonido.name);
        }
        else
        {
            Debug.LogWarning("Dialogos manager no asignado");
        }

    }
}

