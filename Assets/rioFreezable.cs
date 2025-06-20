using UnityEngine;

public class RioFreezable : MonoBehaviour
{
    public PuenteDeHielo sistemaPuenteHielo;
    [Tooltip("Arrastra el GameObject 'PuenteDeHielo' (padre) aquí")]

    private void OnCollisionEnter(Collision collision)
    {
        GameObject objetoImpactante = collision.gameObject;

       
        if (objetoImpactante.name.Contains("Hielo"))
        {
            
            if (sistemaPuenteHielo != null)
            {

               
            }
            else
            {
                Debug.LogError("RioFreezable: No se asignó el sistema de puente de hielo en el Inspector");
            }

            Destroy(collision.gameObject);
        }
    }
}