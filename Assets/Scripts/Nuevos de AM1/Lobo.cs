using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Lobo : A1_A1_H1_MoustroDelAverno
{
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

    // Huida
    bool estaHuyendo = false;
    public float multiplicadorVelocidadHuida = 1.4f;
    public float fuerzaFlee = 8f;
    public float distanciaHuida = 10f; // cuanto se intenta alejar

    void Update()
    {
        if (agent != null && anim != null)
            anim.SetFloat("velocidad", agent.velocity.magnitude);

        if (enAnimacionEspecial) return;

        // Si no hay alfa y la manada es menor a 3 => huir siempre
        if (AlfaActual == null && Manada.Count < 3)
        {
            if (!estaHuyendo) IniciarHuida();
            // ya está huyendo o iniciando huida; comportamiento de huida en su bloque
        }

        if (estaHuyendo)
        {
            if (jugador != null && agent != null)
            {
                // Calcula un destino de huida en NavMesh
                Vector3 destino = CalcularDestinoHuidaDesde(jugador.position);
                agent.SetDestination(destino);
            }
            return; // no ejecutar la IA normal mientras huye
        }

        // IA normal
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
        // lo sacamos de la lista antes de reaccionar
        Manada.Remove(this);

        if (esAlfa)
        {
            // llamamos al handler de alfa muerto
            Lobo.AlfaMurio();
        }
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
        AlfaActual = this;

        vidaActual = Mathf.RoundToInt(vidaActual * multiplicadorVida);
        vidaMaxima = Mathf.RoundToInt(vidaMaxima * multiplicadorVida);
        Velocidad = Mathf.RoundToInt(Velocidad * multiplicadorVelocidad);

        if (agent != null)
            agent.speed = Velocidad;

        transform.localScale = escalaAlfa;

        if (auraPrefab != null)
            auraInstanciada = Instantiate(auraPrefab, transform);

        foreach (var l in Manada)
        {
            if (!l.esAlfa)
                l.DañoDeAtaque = Mathf.RoundToInt(l.DañoDeAtaque * 1.1f); // +10%
        }

        Debug.Log($"{name} es LOBO ALFA");
        foreach (var l in Manada) l.Aullar();
    }

    IEnumerator ProcesoTrasMuerteAlfa()
    {
        // Este método ya no se usa; mantengo por compatibilidad si lo necesitas
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

    // Animation Event al final de la animación Aullar
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
            puntoSalidaLlamarada // parent constructor
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

    // -------------------- HUIDA / MANADA --------------------

    public void IniciarHuida()
    {
        if (estaHuyendo) return;

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
            agent.speed = Velocidad; // normal
    }

    // Calcula un punto alejado del 'target' en el NavMesh (flee)
    private Vector3 CalcularDestinoHuidaDesde(Vector3 target)
    {
        Vector3 direccion = (transform.position - target).normalized;
        Vector3 objetivo = transform.position + direccion * distanciaHuida;

        // sample para asegurar que está sobre NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(objetivo, out hit, 6f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // fallback simple: intenta hacia atrás unos metros
        return transform.position + direccion * (distanciaHuida * 0.5f);
    }

    // Método llamado cuando muere el alfa (estático)
    public static void AlfaMurio()
    {
        Debug.Log("ALFA MURIÓ — MANADA REACCIONA");

        // INMEDIATAMENTE marcar que no hay alfa
        AlfaActual = null;

        // 1) Todos inician huida ahora mismo
        foreach (var l in Manada)
        {
            if (l != null) l.IniciarHuida();
        }

        // 2) Si quedan 3 o más -> después de un tiempo elegimos nuevo alfa
        if (Manada.Count >= 3)
        {
            // arrancamos coroutine desde cualquiera que exista en Manada para esperar y elegir
            Lobo starter = Manada.Find(x => x != null);
            if (starter != null)
                starter.StartCoroutine(ProcesoNuevoAlfa());
        }
        else
        {
            // Menos de 3: se queda sin alfa, terminar huida en X segundos
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

        // Volvemos a chequear que haya 3 o más (pueden haber muerto mientras tanto)
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

    void OnDrawGizmosSelected()
    {
        if (puntoSalidaLlamarada != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(puntoSalidaLlamarada.position, radioDanioLlamarada);
        }
    }
}
