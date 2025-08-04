using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public GameObject Creador;
    public GameObject EfectoEspecial;
    public bool AutoDestruir = true;
    public AudioClip AudioAlAparecer;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2);
        AudioManager.CrearEfectoSonoro(transform.position, AudioAlAparecer);
    }

    public float velocidad = 10f;
    private Vector3 direccion;

    public void Inicializar(Vector3 dir)
    {
        direccion = dir;
        Destroy(gameObject, 5f); // autodestruir
    }

    void Update()
    {
        transform.position += direccion * velocidad * Time.deltaTime;
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
        //Debug.Log(other.name + " " + other.tag, gameObject);
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



    public GameObject Destino = null; // Para proyectiles que siguen un destino
    public int danio = 10;
    public Vector3 PuntoDeColision;
    public AudioClip AudioAlColisionar;
    private void ColisionoCon(GameObject collision, string TipoDeColision)
    {
        if (collision.gameObject == Creador) return;
        if (collision == Creador) return;
        if (Destino != null && Destino != collision)
        {
            return; // No colisionar con el destino
        }
        //Debug.Log("Colisiono con " + collision.name + " tag " + collision.tag + " Tipo: " + TipoDeColision, gameObject);
        // Colisiono con Moustro Mas cercano tag Untagged Tipo: CollisionEnter

        if (collision.tag == "Ambiente" || collision.name == "Terrain")
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
        A1_Entidad enemigo = collision.gameObject.GetComponent<A1_Entidad>(); //interface
        if (enemigo != null)
        {
            if (enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>())
            {
                enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>().ultimoProyectilRecibido = gameObject.name;
            }

            if (EfectoEspecial != null)
            {
                GameObject efecto = Instantiate(EfectoEspecial, collision.transform.position, Quaternion.identity);
                if (EfectoEspecial.GetComponent<ATK_Congelar>() && gameObject.name.Contains("Hielo"))
                {
                    A1_A1_H1_MoustroDelAverno EnemigoV2Real = enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>();
                    EnemigoV2Real.Congelado = true;
                    EnemigoV2Real.PrimerAtaqueAAnular = true;
                    //EnemigoV2Real.S_RupturaDeHielo
                    ATK_Congelar Componente = EfectoEspecial.GetComponent<ATK_Congelar>();
                    Componente.padre = collision.transform;
                    Componente.agent = enemigo.agent;
                    Componente.anim = enemigo.anim;
                    Destroy(efecto, 1);
                    //return;
                }

                else if (EfectoEspecial.GetComponent<ATK_Congelar>() && gameObject.name.Contains("Electrico"))
                {
                    A1_A1_H1_MoustroDelAverno EnemigoV2Real = enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>();
                    if (EnemigoV2Real != null)
                    {
                        EnemigoV2Real.Congelado = true;
                        EnemigoV2Real.PrimerAtaqueAAnular = true;
                        EnemigoV2Real.PendienteDeCargaElectrica = true;
                        ATK_Congelar Componente = EfectoEspecial.GetComponent<ATK_Congelar>();
                        Componente.padre = collision.transform;
                        Componente.agent = enemigo.agent;
                        Componente.anim = enemigo.anim;
                        //return;
                    }
                    Destroy(efecto, 1);
                }
            }
            enemigo.GetComponent<IDaniable>().RecibirDanio(danio);
            Debug.Log(collision.name, collision.gameObject);
            float DistanciaParaAtacar =
                enemigo.ModoAtaqueMelee ?
                enemigo.DistanciaParaAtaqueMelee : enemigo.DistanciaParaAtaqueLargo;
            if (Creador != null && enemigo != null &&

                Vector3.Distance(
                enemigo.transform.position
                , Creador.transform.position) > DistanciaParaAtacar
                )
            {
                if (!enemigo.name.Contains("Jugador"))
                {
                    if (enemigo.gameObject.name != "Jugador 1")
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

        if (gameObject.name.Contains("BolaDeHielo") && EfectoEspecial)
        {
            GameObject Efecto = Instantiate(EfectoEspecial, collision.transform.position, Quaternion.identity);
            ATK_Congelar Componente = Efecto.GetComponent<ATK_Congelar>();
            Componente.anim = enemigo.anim;
            Componente.timer = 5;
            Componente.timerActual = 5;
            Componente.padre = enemigo.transform;
            Debug.Log("Congelando a " + enemigo.name, gameObject);
            enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>().RecibiraDobleDanoLaProximaVez = true;
            enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>().EfectoDeCongelado = Componente;
        }

        else if (gameObject.name.Contains("Electrico") && EfectoEspecial)
        {
            GameObject Efecto = Instantiate(EfectoEspecial, collision.transform.position, Quaternion.identity);
            ATK_Congelar Componente = Efecto.GetComponent<ATK_Congelar>();
            Componente.anim = enemigo.anim;
            Componente.timer = 5;
            Componente.timerActual = 5;
            Componente.padre = enemigo.transform;
            Debug.Log("Congelando a " + enemigo.name, gameObject);
            if (enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>())
            {

                enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>().RecibiraDobleDanoLaProximaVez = true;
                enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>().EfectoDeCongelado = Componente;
                enemigo.GetComponent<A1_A1_H1_MoustroDelAverno>().DestinoAsignado = Creador.transform.position;
            }
            Debug.Log(Creador.transform.position, Creador);
            Componente.destinoGuardado = Creador.transform.position;
        }

        if (AutoDestruir)
        {
            // 5. Destruir objeto
            Destroy(gameObject, 0.1f);
        }
    }

    public float Volumen = 1f;
    public float RadioDeImpactoSonoro = 50f;
    public void OnDestroy()
    {
        AudioManager.CrearEfectoSonoro(transform.position, AudioAlColisionar, Volumen, RadioDeImpactoSonoro);
    }
}
