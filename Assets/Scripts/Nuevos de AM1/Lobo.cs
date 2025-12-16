using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Lobo : A1_A1_H1_MoustroDelAverno
{
    public float tiempoDesaparicionLobo = 2f;

    public static List<Lobo> Manada = new List<Lobo>();
    public static Lobo AlfaActual = null;

    public bool esAlfa = false;
    public Transform jugador;
    public float distanciaParaHacerDanio = 3f;
    public bool estaAtacando = false;
    private bool puedeAtacar = true;

    [Header("Stats del Alfa")]
    public float multiplicadorVida = 1.2f;
    public float multiplicadorVelocidad = 1.2f;

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
    public int danioLlamarada = 8;
    public LayerMask capaJugador;
    public float escalaRelativaLlamarada = 0.5f;
    private bool puedeUsarLlamarada = true;

    // Estados internos
    private enum EstadoAnimacionEspecial { Ninguno, Aullido, Llamarada }
    private EstadoAnimacionEspecial estadoEspecialActual = EstadoAnimacionEspecial.Ninguno;
    private bool enAnimacionEspecial = false;
    private GameObject fxLlamaradaInstanciada;

    bool estaHuyendo = false;
    public float multiplicadorVelocidadHuida = 1.4f;
    public float fuerzaFlee = 8f;
    public float distanciaHuida = 10f; 

    void Update()
    {
        if (agent != null && anim != null)
            anim.SetFloat("velocidad", agent.velocity.magnitude);

        if (enAnimacionEspecial) return;

        bool manadaValida = (Manada.Count >= 3 && AlfaActual != null);

        if (!manadaValida && !esAlfa)
        {
            if (!estaHuyendo)
                IniciarHuida();
        }
        else
        {
            if (estaHuyendo)
                TerminarHuida();
        }


        if (estaHuyendo)
        {
            if (jugador != null && agent != null)
            {
                Vector3 destino = CalcularDestinoHuidaDesde(jugador.position);
                agent.SetDestination(destino);
            }
            return; 
        }

        base.Update();

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

        if (esAlfa && AlfaActual == this)
        {
            AlfaActual = null;
            AlfaMurio();
        }
    }


    void IntentarConvertirseEnAlfa()
    {
        if (AlfaActual != null) return;
        if (Manada.Count < 3) return;

        var candidatos = Manada.FindAll(l => l != null && !l.esAlfa);
        if (candidatos.Count == 0) return;

        candidatos[Random.Range(0, candidatos.Count)].ConvertirseEnAlfa();
    }


    void ConvertirseEnAlfa()
    {
        if (AlfaActual != null && AlfaActual != this)
            return;

        if (esAlfa) return;

        estaHuyendo = false;
        if (agent != null)
            agent.speed = Velocidad;

        esAlfa = true;
        AlfaActual = this;

        vidaActual = Mathf.RoundToInt(vidaActual * multiplicadorVida);
        vidaMaxima = Mathf.RoundToInt(vidaMaxima * multiplicadorVida);
        Velocidad = Mathf.RoundToInt(Velocidad * multiplicadorVelocidad);

        if (agent != null)
            agent.speed = Velocidad;

        transform.localScale = escalaAlfa;

        if (auraPrefab != null && auraInstanciada == null)
            auraInstanciada = Instantiate(auraPrefab, transform);

        foreach (var l in Manada)
        {
            if (l != null && !l.esAlfa)
                l.DañoDeAtaque = Mathf.RoundToInt(l.DañoDeAtaque * 1.1f);
        }

        Debug.Log($"[LOBO] {name} ES ALFA (único)");
        foreach (var l in Manada)
            if (l != null) l.Aullar();
    }


    IEnumerator ProcesoTrasMuerteAlfa()
    {
        yield break;
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

    public void EventoDeAnimacionEspecial()
    {
        if (estadoEspecialActual == EstadoAnimacionEspecial.Llamarada)
            ActivarLlamarada();
        // reset
        estadoEspecialActual = EstadoAnimacionEspecial.Ninguno;
        enAnimacionEspecial = false;
        if (agent != null) agent.isStopped = false;
    }

    public void ActivarLlamarada()
    {
        if (fxLlamaradaHielo == null || puntoSalidaLlamarada == null) return;

        GameObject fx = Instantiate(
            fxLlamaradaHielo,
            puntoSalidaLlamarada 
        );
        fx.transform.localPosition = Vector3.zero;
        fx.transform.localRotation = Quaternion.identity;
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


    public void IniciarHuida()
    {
        if (estaHuyendo || esAlfa) return;

        Debug.Log(name + " ENTRA EN HUIDA");
        estaHuyendo = true;

        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = Velocidad * multiplicadorVelocidadHuida;
            // objetivo inicial de huida:
            if (jugador != null)
            {
                Vector3 destino = CalcularDestinoHuidaDesde(jugador.position);
                agent.SetDestination(destino);
            }
        }
    }

    public void TerminarHuida()
    {
        if (!estaHuyendo) return;

        Debug.Log(name + " TERMINA HUIDA");
        estaHuyendo = false;

        if (agent != null)
            agent.speed = Velocidad; 
    }

    private Vector3 CalcularDestinoHuidaDesde(Vector3 target)
    {
        Vector3 direccion = (transform.position - target).normalized;
        Vector3 objetivo = transform.position + direccion * distanciaHuida;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(objetivo, out hit, 6f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position + direccion * (distanciaHuida * 0.5f);
    }

    public static void AlfaMurio()
    {
        Debug.Log("ALFA MURIÓ — MANADA REACCIONA");

        AlfaActual = null;

        foreach (var l in Manada)
        {
            if (l != null) l.IniciarHuida();
        }

        if (Manada.Count >= 3)
        {
            Lobo starter = Manada.Find(x => x != null);
            if (starter != null)
                starter.StartCoroutine(ProcesoNuevoAlfa());
        }
        else
        {
            Lobo starter = Manada.Find(x => x != null);
            if (starter != null)
                starter.StartCoroutine(FinalsHuidaSinAlfa());
        }
    }

    static IEnumerator FinalsHuidaSinAlfa()
    {
        yield return new WaitForSeconds(3f);

        foreach (var l in Manada)
        {
            if (l != null) l.TerminarHuida();
        }

        AlfaActual = null;
    }

    static IEnumerator ProcesoNuevoAlfa()
    {
        yield return new WaitForSeconds(3f);

        if (Manada.Count >= 3)
        {
            var candidatos = Manada.FindAll(x => x != null && !x.esAlfa);
            if (candidatos.Count > 0)
            {
                var nuevo = candidatos[Random.Range(0, candidatos.Count)];
                if (nuevo != null) nuevo.ConvertirseEnAlfa();
            }
        }
        else
        {
            Debug.Log("Ya no hay suficientes lobos, NO se elige nuevo alfa.");
        }

        foreach (var l in Manada)
        {
            if (l != null) l.TerminarHuida();
        }
    }
    public override void Morir()
    {
        if (estaMuerto) return;

        try
        {
            if (SonidoAlMorir)
                AudioManager.CrearEfectoSonoro(transform.position, SonidoAlMorir);

            if (agent != null) agent.enabled = false;
            if (anim != null) anim.SetBool("life", false);

            StartCoroutine(DesaparecerDespuesDeSegundos(tiempoDesaparicionLobo));

            var col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            var rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        finally
        {
            estaMuerto = true;
        }

        if (generador != null)
            generador.EliminarEnemigo(gameObject);
    }
    public override void EfectoDeRopturaDeCongelamiento()
    {
        EfectoDeCongelado.ReanudarAgente();

        float altura = 1f;

        if (agent != null)
            altura = agent.height * 0.5f;

        Vector3 pos = transform.position + Vector3.up * altura;

        GameObject efecto = Instantiate(VFXDeRopturaDeHielo, pos, Quaternion.identity);
        Destroy(efecto, 2f);

        if (S_RupturaDeHielo != null)
        {
            S_RupturaDeHielo.loop = false;
            S_RupturaDeHielo.Play();
        }

        IrAlDestino(DestinoAsignado);

        if (agent != null)
            agent.isStopped = false;

        RecibiraDobleDanoLaProximaVez = false;
        RalentizarTiempo();

        if (EfectoDeCongelado)
            Destroy(EfectoDeCongelado.gameObject);
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
