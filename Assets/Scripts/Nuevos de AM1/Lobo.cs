using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lobo : A1_A1_H1_MoustroDelAverno
{
    public static List<Lobo> Manada = new List<Lobo>();

    public bool esAlfa = false;
    public Transform jugador;
    public float distanciaParaHacerDanio = 3f;
    public bool estaAtacando = false;
    private bool puedeAtacar = true;

    [Header("Stats del Alfa")]
    public float multiplicadorVida = 2f;
    public float multiplicadorDanio = 2f;
    public float multiplicadorVelocidad = 1.5f;
    public Vector3 escalaAlfa = new Vector3(1.5f, 1.5f, 1.5f);

    [Header("Aura del Alfa")]
    public GameObject auraPrefab;
    private GameObject auraInstanciada;

    [Header("Aullido")]
    public AudioClip sonidoAullido;
    public string triggerAnimacionAullido = "Aullar";

    [Header("Ataque especial del Alfa")]
    public GameObject fxLlamaradaHielo;
    public Transform puntoSalidaLlamarada;
    public float duracionLlamarada = 2f;
    public float cooldownLlamarada = 10f;
    public float radioDanioLlamarada = 4f;
    public int danioLlamarada = 30;
    public LayerMask capaJugador;
    public float escalaRelativaLlamarada = 0.5f;
    private bool puedeUsarLlamarada = true;

    // Estados internos
    private enum EstadoAnimacionEspecial { Ninguno, Aullido, Llamarada }
    private EstadoAnimacionEspecial estadoEspecialActual = EstadoAnimacionEspecial.Ninguno;
    private bool enAnimacionEspecial = false;
    private GameObject fxLlamaradaInstanciada;

    void Update()
    {
        // actualizar parámetro de correr
        if (agent != null && anim != null)
            anim.SetFloat("velocidad", agent.velocity.magnitude);

        // si está en aullido/llamarada, no persigue
        if (enAnimacionEspecial) return;

        // IA normal (llama a EjecutarCadaMedioSegundo a través de base.Update())
        base.Update();

        // uso automático de llamarada si es alfa y cerca
        if (esAlfa && puedeUsarLlamarada && jugador != null &&
            Vector3.Distance(transform.position, jugador.position) < 8f)
        {
            UsarLlamaradaDeHielo();
        }
    }

    protected override void Start()
    {
        base.Start();
        if (agent != null)
        {
            agent.updatePosition = true;
            agent.updateRotation = true;
        }
        Manada.Add(this);
        Invoke(nameof(IntentarConvertirseEnAlfa), 1f);
        jugador = GameObject.FindGameObjectWithTag("Jugador")?.transform;

        
    }

    public override void Atacar(Vector3 destino, string nombre = "")
    {
        if (estaMuerto || estaAtacando || !puedeAtacar) return;
        transform.LookAt(destino);
        anim.SetTrigger("ataque1");
        estaAtacando = true;
        puedeAtacar = false;
        StartCoroutine(FinalizarAtaque(1f));
    }

    IEnumerator FinalizarAtaque(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        estaAtacando = false;
        puedeAtacar = true;
    }

    public void AplicarDanio()
    {
        if (jugador == null || estaMuerto) return;
        if (Vector3.Distance(transform.position, jugador.position) <= distanciaParaHacerDanio)
        {
            var entidad = jugador.GetComponent<A1_Entidad>();
            if (entidad != null)
            {
                entidad.GetComponent<IDaniable>().RecibirDanio(DañoDeAtaque);
                Debug.Log($"[Lobo] Atacó e hizo {DañoDeAtaque} de daño.");
            }
        }
    }

    void OnDestroy()
    {
        Manada.Remove(this);
    }

    void IntentarConvertirseEnAlfa()
    {
        if (Manada.Count >= 3 && !Manada.Exists(l => l.esAlfa))
        {
            var elegido = Manada[Random.Range(0, Manada.Count)];
            elegido.ConvertirseEnAlfa();
        }
    }

    void ConvertirseEnAlfa()
    {
        esAlfa = true;
        Vida = Mathf.RoundToInt(Vida * multiplicadorVida);
        VidaMax = Mathf.RoundToInt(VidaMax * multiplicadorVida);
        DañoDeAtaque = Mathf.RoundToInt(DañoDeAtaque * multiplicadorDanio);
        Velocidad = Mathf.RoundToInt(Velocidad * multiplicadorVelocidad);
        if (agent != null) agent.speed *= multiplicadorVelocidad;
        transform.localScale = escalaAlfa;
        if (auraPrefab != null)
            auraInstanciada = Instantiate(auraPrefab, transform);
        foreach (var l in Manada) l.Aullar();
        Debug.Log($"{name} es LOBO ALFA");
    }

    public void Aullar()
    {
        if (anim == null || agent == null) return;
        enAnimacionEspecial = true;
        agent.isStopped = true;
        estadoEspecialActual = EstadoAnimacionEspecial.Aullido;
        anim.SetTrigger(triggerAnimacionAullido);
        if (S_Caminar != null && sonidoAullido != null)
        {
            S_Caminar.clip = sonidoAullido;
            S_Caminar.Play();
        }
    }

    public void UsarLlamaradaDeHielo()
    {
        if (!esAlfa || !puedeUsarLlamarada || estaMuerto || anim == null || agent == null)
            return;
        enAnimacionEspecial = true;
        agent.isStopped = true;
        transform.LookAt(jugador);
        estadoEspecialActual = EstadoAnimacionEspecial.Llamarada;
        anim.SetTrigger(triggerAnimacionAullido);
        puedeUsarLlamarada = false;
        StartCoroutine(CooldownLlamarada());
    }

    IEnumerator CooldownLlamarada()
    {
        yield return new WaitForSeconds(cooldownLlamarada);
        puedeUsarLlamarada = true;
    }

    // Animation Event al final de la animación Aullar
    public void EventoDeAnimacionEspecial()
    {
        if (estadoEspecialActual == EstadoAnimacionEspecial.Llamarada)
            ActivarLlamarada();
        // reset
        estadoEspecialActual = EstadoAnimacionEspecial.Ninguno;
        enAnimacionEspecial = false;
        agent.isStopped = false;
    }

    public void ActivarLlamarada()
    {
        if (fxLlamaradaHielo == null || puntoSalidaLlamarada == null) return;

        // 1) Instanciar directamente como hijo del hueso:
        GameObject fx = Instantiate(
            fxLlamaradaHielo,
            puntoSalidaLlamarada       // parent
        );
        // 2) Resetar transform local para que quede justo en la boca:
        fx.transform.localPosition = Vector3.zero;
        fx.transform.localRotation = Quaternion.identity;
        // 3) Escala relativa (ajústalo aquí al valor que quieras):
        fx.transform.localScale = Vector3.one * escalaRelativaLlamarada;

        fxLlamaradaInstanciada = fx;
        Destroy(fx, duracionLlamarada);

        StartCoroutine(DanioPorLlamarada());
    }



    IEnumerator DanioPorLlamarada()
    {
        float t = duracionLlamarada;
        while (t > 0f)
        {
            t -= 0.3f;
            if (fxLlamaradaInstanciada != null)
            {
                // usar posición real del VFX
                Vector3 centro = fxLlamaradaInstanciada.transform.position;
                var cols = Physics.OverlapSphere(centro, radioDanioLlamarada, capaJugador);
                foreach (var col in cols)
                {
                    var entidad = col.GetComponent<A1_Entidad>();
                    if (entidad != null)
                    {
                        entidad.GetComponent<IDaniable>().RecibirDanio(danioLlamarada);
                        Debug.Log("[Lobo Alfa] Daño por llamarada");
                    }
                }
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (puntoSalidaLlamarada != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(puntoSalidaLlamarada.position, radioDanioLlamarada);
        }
    }
}

