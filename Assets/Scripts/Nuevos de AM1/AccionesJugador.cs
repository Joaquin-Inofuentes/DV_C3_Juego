using Drakkar.GameUtils;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class AccionesJugador : A1_Entidad, IDaniable, IContadormonedas
{
    [Header("🏹 Proyectiles y Hitboxes")]
    public GameObject BolaDeFuego;
    public GameObject BolaDeHielo;
    public GameObject Rayo;
    public GameObject ataqueRapido;
    public GameObject Flechazo;
    public GameObject hitboxCuboPrefab;
    public float fuerzaDisparo = 500f;
    public GameObject espada;
    public Transform Origen;
    public GameObject trailObject;
    public TrailRenderer trail;
    public float trailTime = 0.5f;
    public float clearDelay = 0.1f;

    [Header("🗡️ Datos de Combate Cuerpo a Cuerpo")]
    public bool modoMelee = false;
    public Transform puntoGolpePatada;
    public Transform puntoGolpeEspada;

    [Header("❤️ Estado del Jugador")]
    private bool estaMuerto = false;
    private bool hitboxGenerada = false;
    public Vector3 Destino;

    [Header("🎨 Feedbacks y Partículas")]
    public ParticleSystem Particulas;
    public GameObject explosionPrefab;
    public Transform puntoDeImpacto;
    public Feedbacks Feedbacks;
    public Color Color_RecibeDano;
    public Color Color_ObtieneMonedas;
    public Color Color_FueAvistado;
    public Color Color_Muere;
    public Color Color_SeCura;

    [Header("Efectos sonido")]
    public AudioSource Coins;
    public int CantidadDeMonedas;
    [SerializeField] private TextMeshProUGUI textoMonedasUI;

    [Header("⏱️ Cooldown Interno (barra horizontal)")]
    public float maxCoolDown = 0f;
    private float _coolDown = 0f;
    public float CoolDown
    {
        get => _coolDown;
        set
        {
            _coolDown = value;
            if (_coolDown > maxCoolDown)
                maxCoolDown = _coolDown;
            ActualizarBarraCoolDown();
        }
    }
    public UnityEngine.UI.Image barraCoolDown;

    [Header("🎯 Referencia a TimerManager (cooldowns de habilidades)")]
    public TimerManager _TimerManager;

    [Header("🛠️ Indicadores Melee/Rango")]
    public GameObject IndicadoresMelee;
    public GameObject Flecha;

    [Header("Combos")]
    public List<string> AtaquesQImpactaron = new List<string>();
    public float TimerDeCombos = 0f;
    public Transform OrigenDelDisparo;
    public GameObject PrefabDeComboElectrico;
    public GameObject PrefabDeComboFuego;
    public GameObject VFX_ComboExplosion;
    public AudioClip SonidoComboExplosion;
    public GameObject VFX_EfectoDeCuracion;
    public AudioClip VFX_A_EfectoDeCuracion;

    [Header("Sonidos de Pasos")]
    public AudioClip SonidoDeCaminarEnHielo;
    public AudioClip SonidoDeCaminarEnPasto;

    // Variables de estado y audio
    public Vector3 DestinoGuardado;
    public AudioSource RecibirDanoAudio;
    public AudioSource SonidoDeMorir;
    public GameObject FondoOscuroSangriendo;
    private static float tiempoInicio;


    void Awake()
    {
        if (trail != null)
        {
            trail.time = trailTime;
            trail.Clear();
            trail.emitting = false;
        }
    }

    void Start()
    {
        if (_TimerManager == null)
            Debug.LogWarning("[AccionesJugador] No asignaste TimerManager en el Inspector.");
        if (espada != null)
        {
            espada.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) { AtaqueHardCodeado(); }
        if (Input.GetKeyDown(KeyCode.F2)) { AtaqueHardCodeado2(); }

        if (_TimerManager.magiaBloqueadaPorZona)
        {
            if (_TimerManager.enModoMagico)
            {
                _TimerManager.enModoMagico = false;
            }
        }
        modoMelee = !_TimerManager.enModoMagico;

        CargarBarraDeCoolDown();

        float velocidadActual = agent.velocity.magnitude;
        anim.SetFloat("velocidad", velocidadActual);
        if (Vector3.Distance(transform.position, Destino) < 2f)
        {
            Detenerse();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (!modoMelee)
            {
                modoMelee = true;
                if (!TimerManager.Controler.enTransicionVisual)
                {
                    TimerManager.Controler.TransicionarModoVisual();
                    TimerManager.Controler.enModoMagico = false;
                }
                Debug.Log("Modo cambiado a MELEE");
                anim.SetLayerWeight(0, 0f);
                anim.SetLayerWeight(1, 1f);
                if (espada != null) espada.SetActive(true);
                IndicadoresMelee.SetActive(true);
            }
            else if (modoMelee)
            {
                modoMelee = false;
                _TimerManager.TransicionarModoVisual();
                _TimerManager.enModoMagico = true;
                Debug.Log("Modo cambiado a RANGO");
                anim.SetLayerWeight(0, 1f);
                anim.SetLayerWeight(1, 0f);
                if (espada != null) espada.SetActive(false);
                IndicadoresMelee.SetActive(false);
            }
        }

        RotarFlechaHaciaElCursor();
        CorrerTimerDeCombos();
    }

    public override void Atacar(Vector3 Destino, string Nombre)
    {
        Debug.Log("Iniciando ataque...");
        if (estaMuerto || _TimerManager == null || _TimerManager.IsTimerCharging(6))
        {
            return;
        }

        _TimerManager.SetTimerToMax(6);
        GameObject prefabDelAtaque = null;

        // --- Lógica de selección de prefab ---
        if (Nombre == "BolaDeFuego" || Nombre == "Melee1")
        {
            if (!modoMelee)
            {
                if (_TimerManager.IsTimerCharging(0)) return;
                if (TimerDeCombos > 0 && AtaquesQImpactaron.Count == 2 && AtaquesQImpactaron[0] == "Melee1" && AtaquesQImpactaron[1] == "Melee2")
                {
                    Debug.Log("Usando combo de FUEGO");
                    prefabDelAtaque = PrefabDeComboFuego;
                    AtaquesQImpactaron.Clear();
                    TimerDeCombos = 0f;
                }
                else
                {
                    prefabDelAtaque = BolaDeFuego;
                }
                anim.SetTrigger("magic1");
                _TimerManager.SetTimerToMax(0);
                RegistrarAhora();
            }
            else // Lógica Melee
            {
                if (_TimerManager.IsTimerCharging(3)) return;
                anim.SetTrigger("melee1");
                _TimerManager.SetTimerToMax(3);
                if (PasoTiempo(2)) CrearEfectoDeExplosion();
            }
        }
        else if (Nombre == "BolaDeHielo" || Nombre == "Melee2")
        {
            if (!modoMelee)
            {
                if (_TimerManager.IsTimerCharging(1)) return;
                anim.SetTrigger("magic2");
                prefabDelAtaque = BolaDeHielo;
                _TimerManager.SetTimerToMax(1);
            }
            else
            {
                if (_TimerManager.IsTimerCharging(4)) return;
                anim.SetTrigger("melee2");
                _TimerManager.SetTimerToMax(4);
            }
        }
        else if (Nombre == "RayoElectrico" || Nombre == "Melee3")
        {
            if (!modoMelee)
            {
                if (_TimerManager.IsTimerCharging(2)) return;
                if (TimerDeCombos > 0 && AtaquesQImpactaron.Count == 2 && AtaquesQImpactaron[0] == "Melee3" && AtaquesQImpactaron[1] == "Melee2")
                {
                    Debug.Log("Usando combo ELÉCTRICO");
                    prefabDelAtaque = PrefabDeComboElectrico;
                    AtaquesQImpactaron.Clear();
                    TimerDeCombos = 0f;
                }
                else
                {
                    prefabDelAtaque = Rayo;
                }
                anim.SetTrigger("magic3");
                _TimerManager.SetTimerToMax(2);
            }
            else // Lógica Melee
            {
                if (_TimerManager.IsTimerCharging(5)) return;
                anim.SetTrigger("melee3");
                _TimerManager.SetTimerToMax(5);
            }
        }

        // Si es un ataque Melee, no se instancia un proyectil aquí.
        if (prefabDelAtaque == null)
        {
            Debug.Log("Ataque Melee o sin prefab. La generación del hitbox se maneja por evento de animación.");
            return;
        }

        // --- Instanciación del Proyectil ---
        transform.LookAt(Destino);
        Vector3 direccion = (Destino - Origen.position).normalized;
        Quaternion rotacion = Quaternion.LookRotation(direccion);

        GameObject ataqueInstanciado = Instantiate(prefabDelAtaque, Origen.position, rotacion);
        ataqueInstanciado.name = Nombre; // Muy importante para el sistema de combos
        Debug.Log($"{ataqueInstanciado.name} instanciado en {Origen.position}", ataqueInstanciado);

        if (ataqueInstanciado.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(direccion * fuerzaDisparo);
        }

        Destroy(ataqueInstanciado, 10f); // Destrucción de seguridad

        if (ataqueInstanciado.GetComponent<Proyectil>())
        {
            ataqueInstanciado.GetComponent<Proyectil>().Creador = gameObject;
        }
        if (ataqueInstanciado.GetComponent<Hitbox>())
        {
            ataqueInstanciado.GetComponent<Hitbox>().Creador = gameObject;
        }

        Invoke(nameof(RegistrarCoolDown), 0.1f);
    }

    #region Hitboxes y Lógica Melee
    public void GenerarHitboxAtaqueRapido() => GenerarHitbox(puntoGolpeEspada, 15, "Melee1");
    public void GenerarHitboxAtaquePesado() => GenerarHitbox(puntoGolpeEspada, 35, "Melee3");
    public void GenerarHitboxPie()
    {
        GenerarHitbox(puntoGolpePatada, 10, "Melee2");
        EmpujarEnemigosConPatada();
    }

    public void GenerarHitbox(Transform puntoDeGolpe, int danio, string hitboxName)
    {
        if (hitboxGenerada) return;
        if (hitboxCuboPrefab != null && puntoDeGolpe != null)
        {
            GameObject hitboxObj = Instantiate(hitboxCuboPrefab, puntoDeGolpe.position, gameObject.transform.rotation);
            if (hitboxObj.TryGetComponent<Hitbox>(out var componenteHitbox))
                componenteHitbox.ConfigurarDanio(danio);

            hitboxObj.name = hitboxName;
            hitboxGenerada = true;
            Destroy(hitboxObj, 0.5f);
            Invoke(nameof(ResetHitboxFlag), 0.1f);
        }
    }

    private void ResetHitboxFlag()
    {
        hitboxGenerada = false;
    }
    #endregion

    #region Salud y Daño
    public int vidaMaxima => VidaMax;
    public int vidaActual { get => Vida; set => Vida = value; }

    public void RecibirDanio(int cantidad)
    {
        Vida -= cantidad;
        Debug.Log($"Recibió {cantidad} de daño, su vida actual es {vidaActual}");
        Feedbacks.FeedbackRadialVisual(Color_RecibeDano, 1);
        EfectoDeRelentizarTiempo();
        if (Vida <= 0)
        {
            Morir();
            Invoke(nameof(CargaEscenaDerrota), 3f);
        }
        RecibirDanoAudio.Play();
    }

    public void Morir()
    {
        if (estaMuerto) return;
        Feedbacks.FeedbackRadialVisual(Color_Muere, 4);
        estaMuerto = true;
        anim.SetTrigger("life");
        SonidoDeMorir.Play();
        FondoOscuroSangriendo.SetActive(true);
    }

    void CargaEscenaDerrota()
    {
        SceneManager.LoadScene("Derrota");
    }

    public void EfectoDeRelentizarTiempo()
    {
        Time.timeScale = 0.6f;
        Invoke(nameof(RestablecerTiempo), 0.5f);
    }
    public void RestablecerTiempo()
    {
        Time.timeScale = 1f;
    }
    #endregion

    #region Movimiento y Navegación
    public override void Detenerse()
    {
        agent.isStopped = true;
        if (S_Caminar.isPlaying)
        {
            S_Caminar.Stop();
        }
    }

    public override void IrAlDestino(Vector3 destino)
    {
        if (Vector3.Distance(destino, DestinoGuardado) < 2) return;
        DestinoGuardado = destino;
        if (estaMuerto) return;

        agent.isStopped = false;
        transform.LookAt(destino);
        agent.SetDestination(destino);
        Destino = destino;

        if (Particulas)
        {
            Particulas.transform.position = destino;
            Particulas.Play();
        }

        if (!S_Caminar.isPlaying)
        {
            S_Caminar.loop = true;
            S_Caminar.Play();
        }
    }
    #endregion

    #region Utilidades y Helpers
    void CorrerTimerDeCombos()
    {
        if (TimerDeCombos > 0f)
            TimerDeCombos -= Time.deltaTime;
        if (TimerDeCombos <= 0f)
        {
            AtaquesQImpactaron.Clear();
        }
    }

    void CargarBarraDeCoolDown()
    {
        if (_coolDown > 0f)
            _coolDown -= Time.deltaTime;
        if (_coolDown < 0f)
            _coolDown = 0f;
        ActualizarBarraCoolDown();
    }

    private void ActualizarBarraCoolDown()
    {
        if (barraCoolDown == null || maxCoolDown == 0f) return;
        float porcentaje = 1f - Mathf.Clamp01(_coolDown / maxCoolDown);
        float anchoMaximo = 200f;
        barraCoolDown.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, anchoMaximo * porcentaje);
    }

    public void RotarFlechaHaciaElCursor()
    {
        if (Flecha == null) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plano = new Plane(Vector3.up, Flecha.transform.position);
        if (plano.Raycast(ray, out float distancia))
        {
            Vector3 puntoMundo = ray.GetPoint(distancia);
            Vector3 direccion = puntoMundo - Flecha.transform.position;
            direccion.y = 0f;
            if (direccion.sqrMagnitude > 0.001f)
            {
                Quaternion rot = Quaternion.LookRotation(direccion, Vector3.up);
                Vector3 euler = rot.eulerAngles;
                euler.x = 90f;
                euler.z -= 90f;
                Flecha.transform.rotation = Quaternion.Euler(euler);
            }
        }
    }
    #endregion

    #region Resto del Código

    // --- Métodos de Depuración y Hardcodeo ---

    public void AtaqueHardCodeado()
    {
        AtaquesQImpactaron = new List<string> { "Melee1", "Melee2" };
        TimerDeCombos = 2f;
        Vector3 posicion = OrigenDelDisparo.position;
        CrearCuboGenerico(posicion);
        Atacar(posicion, "BolaDeFuego");
    }

    public void AtaqueHardCodeado2()
    {
        AtaquesQImpactaron = new List<string> { "Melee3", "Melee2" };
        TimerDeCombos = 2f;
        Vector3 posicion = OrigenDelDisparo.position;
        CrearCuboGenerico(posicion);
        Atacar(posicion, "RayoElectrico");
    }

    public void CrearCuboGenerico(Vector3 posicion)
    {
        GameObject cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubo.transform.position = posicion;
        cubo.transform.localScale = Vector3.one;
        if (cubo.TryGetComponent<Renderer>(out var renderer))
        {
            renderer.material.color = Color.yellow;
        }
        if (cubo.TryGetComponent<BoxCollider>(out var collider))
        {
            collider.enabled = false;
        }
        cubo.transform.rotation = transform.rotation;
        Destroy(cubo, 2f);
    }

    // --- Lógica de Cooldown y Animación ---

    public void RegistrarCoolDown()
    {
        float speed = 1f;
        if (anim.GetCurrentAnimatorClipInfo(0).Length > 0)
        {
            string nombreAnimacion = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            if (nombreAnimacion.Contains("01")) speed = 8f;
            else if (nombreAnimacion.Contains("02")) speed = 4f;
            else if (nombreAnimacion.Contains("03")) speed = 4f;
            else if (nombreAnimacion.Contains("ataque_pesadoPersonaje")) speed = 2f;
            else if (nombreAnimacion.Contains("ataque_rapidoPersonaje")) speed = 3f;
            else if (nombreAnimacion.Contains("ataque_fuertePersonaje")) speed = 0.9f;

            float tiempoAnim = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length / speed;
            CoolDown = Mathf.Min(tiempoAnim, 2f);
        }
    }

    // --- Efectos de Habilidades ---

    void EmpujarEnemigosConPatada()
    {
        Collider[] enemigos = Physics.OverlapSphere(puntoGolpePatada.position, 2f);
        foreach (var col in enemigos)
        {
            if (col.TryGetComponent<A1_A1_H1_MoustroDelAverno>(out var enemigo))
            {
                if (col.TryGetComponent<Rigidbody>(out var rb) && !rb.isKinematic)
                {
                    Vector3 direccion = (col.transform.position - transform.position).normalized;
                    if (col.TryGetComponent<NavMeshAgent>(out var agent))
                    {
                        if (NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                        {
                            agent.Warp(hit.position);
                            agent.enabled = false;
                            float fuerzaEmpuje = 5f;
                            rb.AddForce(direccion * fuerzaEmpuje, ForceMode.Impulse);
                            StartCoroutine(ReactivarAgente(agent, 1f));
                        }
                    }
                    else
                    {
                        float fuerzaEmpuje = 5f;
                        rb.AddForce(direccion * fuerzaEmpuje, ForceMode.Impulse);
                    }
                }
            }
        }
    }

    private IEnumerator ReactivarAgente(NavMeshAgent agent, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (agent != null)
        {
            if (NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                agent.enabled = true;
                agent.isStopped = false;
            }
        }
    }

    public void ActivarEfectoPatada()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, puntoDeImpacto.position, Quaternion.identity);
        }
    }

    public void ActivarTrail()
    {
        if (trail == null) return;
        trail.Clear();
        trail.emitting = true;
    }

    public void DesactivarTrail()
    {
        if (trail == null) return;
        trail.emitting = false;
    }

    public override void OnCollision(Collision collider)
    {
        // Implementar si es necesario
    }

    public override void Colisiono(GameObject col, string TipoDeColision)
    {
        if (col.TryGetComponent<A3_Interactuable>(out var interactivo))
        {
            interactivo.Interactuar();
        }
    }

    // --- Sistema de Monedas ---

    public int ContadorDeMonedas => CantidadDeMonedas;

    public void SumarMonedas(int cantidad)
    {
        CantidadDeMonedas += cantidad;
        ActualizarTextoMonedas();
    }

    public void ActualizarTextoMonedas()
    {
        if (textoMonedasUI != null)
        {
            textoMonedasUI.text = CantidadDeMonedas.ToString();
        }
    }

    // --- Sonidos ---

    public void ReproducirSonidoDeCaminar()
    {
        float posicionZ = Mathf.Abs(transform.position.z);
        if (posicionZ < 195 && posicionZ > 175)
        {
            S_Caminar.clip = SonidoDeCaminarEnHielo;
        }
        else
        {
            S_Caminar.clip = SonidoDeCaminarEnPasto;
        }
    }

    // --- Medidor de Tiempo Estático ---

    public static void RegistrarAhora()
    {
        tiempoInicio = Time.time;
    }

    public static bool PasoTiempo(float segundos)
    {
        return Time.time - tiempoInicio <= segundos;
    }

    // --- Efectos de Combos ---

    public void CrearEfectoDeExplosion()
    {
        if (estaMuerto || VFX_ComboExplosion == null || SonidoComboExplosion == null) return;

        Feedbacks.Componente.UIFadeComboScript.MostrarTexto("¡ONDA FLAMEANTE!", new Color(1f, 0.352f, 0f));
        Vector3 origenDeEfecto = transform.position;
        Instantiate(VFX_ComboExplosion, origenDeEfecto, Quaternion.identity);
        AudioManager.CrearEfectoSonoro(transform.position, SonidoComboExplosion);
    }

    public void CrearEfectoDeCuracion()
    {
        if (estaMuerto || VFX_EfectoDeCuracion == null) return;

        if (vidaActual < vidaMaxima)
        {
            vidaActual += 20;
            if (vidaActual > vidaMaxima) vidaActual = vidaMaxima;

            GameObject efectoDeCuracion = Instantiate(VFX_EfectoDeCuracion, transform.position, Quaternion.identity);
            AudioManager.CrearEfectoSonoro(transform.position, VFX_A_EfectoDeCuracion);
            Destroy(efectoDeCuracion, 2f);
            efectoDeCuracion.transform.SetParent(transform);
        }
    }

    #endregion
}