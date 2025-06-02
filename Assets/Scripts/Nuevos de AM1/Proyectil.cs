using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public GameObject Creador;
    public GameObject EfectoEspecial;
    public bool AutoDestruir = true;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2);
    }

    // Update is called once per frame
    void Update()
    {

    }



    private void OnCollisionEnter(Collision c)
    {
        if (c.contactCount > 0)
            PuntoDeColision = c.GetContact(0).point;
        ColisionoCon(c.gameObject, "CollisionEnter");
    }
    private void OnCollisionStay(Collision c)
    {
        if (c.contactCount > 0)
            PuntoDeColision = c.GetContact(0).point;
        ColisionoCon(c.gameObject, "CollisionStay");
    }
    private void OnCollisionExit(Collision c)
    {
        if (c.contactCount > 0)
            PuntoDeColision = c.GetContact(0).point;
        ColisionoCon(c.gameObject, "CollisionExit");
    }

    private void OnTriggerEnter(Collider other)
    {
        PuntoDeColision = other.ClosestPoint(transform.position);
        ColisionoCon(other.gameObject, "TriggerEnter");
    }
    private void OnTriggerStay(Collider other)
    {
        PuntoDeColision = other.ClosestPoint(transform.position);
        ColisionoCon(other.gameObject, "TriggerStay");
    }
    private void OnTriggerExit(Collider other)
    {
        PuntoDeColision = other.ClosestPoint(transform.position);
        ColisionoCon(other.gameObject, "TriggerExit");
    }




    public int danio = 10;
    public Vector3 PuntoDeColision;
    private void ColisionoCon(GameObject collision, string TipoDeColision)
    {
        if (collision == Creador) return;
        if(collision.tag == "Ambiente")
        {
            //Debug.Log(collision.ToString() + TipoDeColision);
            GameObject efecto = Instantiate(EfectoEspecial, PuntoDeColision, Quaternion.identity);
            Destroy(efecto, 0.4f);
            Destroy(gameObject);
            return;
        }

        if (collision.tag == "Monedas")
        {
            //Debug.Log(collision.ToString() + TipoDeColision);
            GameObject efecto = Instantiate(EfectoEspecial, PuntoDeColision, Quaternion.identity);
            Destroy(efecto, 0.3f);
            Destroy(gameObject);
            return;
        }
        //Debug.Log("___" + collision.ToString() + " _ " + TipoDeColision);
        // 1. Verifica si es enemigo
        A1_Entidad enemigo = collision.gameObject.GetComponent<A1_Entidad>();
        if (enemigo != null)
        {
            if (EfectoEspecial != null)
            {
                GameObject efecto = Instantiate(EfectoEspecial, collision.transform.position, Quaternion.identity);
                Destroy(efecto, 1);
                if (EfectoEspecial.GetComponent<ATK_Congelar>() && gameObject.name.Contains("Hielo"))
                {
                    ATK_Congelar Componente = EfectoEspecial.GetComponent<ATK_Congelar>();
                    Componente.padre = collision.transform;
                    Componente.agent = enemigo.agent;
                    Componente.anim = enemigo.anim;
                }
            }
            enemigo.RecibirDanio(danio);
            float DistanciaParaAtacar =
                enemigo.ModoAtaqueMelee ?
                enemigo.DistanciaParaAtaqueMelee : enemigo.DistanciaParaAtaqueLargo;
            if (Vector3.Distance(
                enemigo.transform.position
                , Creador.transform.position) > DistanciaParaAtacar)
            {
                if (!enemigo.name.Contains("Jugador"))
                {
                    enemigo.IrAlDestino(Creador.transform.position);
                }
            }
        }
        if (enemigo == null)
        {
            return;
        }

        // 2. Desactiva collider y arranca animación
        GetComponent<BoxCollider>().enabled = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb);
        }

        if (gameObject.name == "BolaDeHielo" && EfectoEspecial)
        {
            GameObject Efecto = Instantiate(EfectoEspecial, collision.transform.position, Quaternion.identity);
            ATK_Congelar Componente = Efecto.GetComponent<ATK_Congelar>();
            Componente.anim = enemigo.anim;
            Componente.timer = 4;
            Componente.padre = enemigo.transform;
        }

        if (AutoDestruir)
        {
            // 5. Destruir objeto
            Destroy(gameObject, 0.1f);
        }
    }

}
