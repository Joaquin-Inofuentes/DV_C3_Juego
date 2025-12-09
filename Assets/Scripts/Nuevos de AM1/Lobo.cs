using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    void Update()
    {
        // actualizar parámetro de correr
        if (agent != null && anim != null)
            anim.SetFloat("velocidad", agent.velocity.magnitude);

        // si está en aullido/llamarada, no persigue
        if (enAnimacionEspecial) return;

        if (estaHuyendo)
        {
            if (jugador != null)
            {
                Vector3 fleeDestino = Flee(jugador.position);
                agent.SetDestination(fleeDestino);
            }
            return;
        }


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

        if (esAlfa)
        {
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
        Debug.Log("El ALFA murió. Los betas huyen.");

        // 1) betas huyen
        foreach (var l in Manada)
        {
            l.IniciarHuida();
        }

        // 2) esperar unos segundos
        yield return new WaitForSeconds(3f);

        // 3) si quedan 2 o más, elegir nuevo alfa
        if (Manada.Count >= 2)
        {
            var nuevo = Manada[Random.Range(0, Manada.Count)];
            nuevo.ConvertirseEnAlfa();
        }

        // 4) quitar estado de huida
        foreach (var l in Manada)
        {
            l.TerminarHuida();
        }
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

    bool estaHuyendo = false;

    // Velocidad extra durante huida
    public float multiplicadorVelocidadHuida = 1.4f;

    // Factor de suavizado
    public float fuerzaFlee = 8f;


    public void IniciarHuida()
    {
        Debug.Log(name + " ENTRA EN HUIDA");
        estaHuyendo = true;

        if (agent != null)
            agent.speed = Velocidad * multiplicadorVelocidadHuida;
    }


    public void TerminarHuida()
    {
        estaHuyendo = false;

        if (agent != null)
            agent.speed = Velocidad; // normal
    }

    private Vector3 Flee(Vector3 target)
    {
        // Dirección opuesta al jugador
        Vector3 desired = (transform.position - target).normalized * fuerzaFlee;

        // Convertirlo a un punto de destino en el NavMesh
        return transform.position + desired;
    }
    public static void AlfaMurio()
    {
        Debug.Log("ALFA MURIÓ — MANADA REACCIONA");

        // 1) Modo huida
        foreach (var l in Manada)
            l.IniciarHuida();

        // 2) Esperar para elegir nuevo alfa
        if (Manada.Count > 1)
            Manada[0].StartCoroutine(ProcesoNuevoAlfa());
    }

    static IEnumerator ProcesoNuevoAlfa()
    {
        yield return new WaitForSeconds(3f);

        if (Manada.Count >= 2)
        {
            var nuevo = Manada[Random.Range(0, Manada.Count)];
            nuevo.ConvertirseEnAlfa();
        }

        foreach (var l in Manada)
            l.TerminarHuida();
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

