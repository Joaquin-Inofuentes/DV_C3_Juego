using System.Collections;
using UnityEngine;

public class BarrilExplosivo : A3_Interactuable
{
    public float tiempoParaExplotar = 2.5f;
    public float radioDeExplosion = 5f;
    public int danioExplosion = 40;
    public GameObject efectoEncendido;
    public GameObject efectoExplosion;
    public Transform puntoDeEfecto;

    private bool estaPorExplotar = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (estaPorExplotar)
        {
            rb.velocity = Vector3.zero;  
            rb.isKinematic = true;       
        }
    }

    public override void Interactuar()
    {
       
        if (!estaPorExplotar)
            StartCoroutine(ExplosiónEnInstantes());
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (estaPorExplotar) return;

        if (collision.gameObject.CompareTag("HechizoDeFuego"))
        {
            Destroy(collision.gameObject);
            StartCoroutine(ExplosiónEnInstantes());
            
        }
    }

    IEnumerator ExplosiónEnInstantes()
    {
        estaPorExplotar = true;

        if (efectoEncendido != null)
        {
            GameObject fx = Instantiate(efectoEncendido, puntoDeEfecto.position, Quaternion.identity);
            ParticleSystem ps = fx.GetComponentInChildren<ParticleSystem>();
            if (ps != null) ps.Play();
            Destroy(fx, 1.9f); // destruir después de unos segundos
        }



        yield return new WaitForSeconds(tiempoParaExplotar);
       
        if (efectoEncendido != null)
            efectoEncendido.SetActive(false);
       
        if (efectoExplosion != null)
            Instantiate(efectoExplosion, transform.position, Quaternion.identity);

        if (SonidoDeInteraccion != null)
            SonidoDeInteraccion.Play();

        
        Collider[] hits = Physics.OverlapSphere(transform.position, radioDeExplosion);
        foreach (Collider hit in hits)
        {
            EntidadBase entidad = hit.GetComponentInParent<EntidadBase>();
            if (entidad != null)
            {
                entidad.SendMessage("RecibirDanio", danioExplosion, SendMessageOptions.DontRequireReceiver);
            }
        }

        Destroy(gameObject, 0.2f);
    }


    public override void OnDestroy()
    {
       
    }
}

