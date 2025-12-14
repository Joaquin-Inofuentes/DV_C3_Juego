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

// ENUMS Y STRUCTS (Sin cambios)
public enum ModoPelea { Rango, Melee }
public enum TipoAtaque { BolaDeFuego, BolaDeHielo, RayoElectrico, Melee1, Melee2, Melee3 }
public struct InfoAtaque
{
    public TipoAtaque Tipo;
    public GameObject Prefab;
    public int CooldownIndex;
    public int DanoBase;
    public string NombreAnimacion;
}

public class AccionesJugador : A1_Entidad, IDaniable, IContadormonedas
{
    // ... (El resto de las variables no cambian) ...

    #region Variables de Inspector y Serializadas

    [Header("🏹 Proyectiles y Hitboxes")]
    public GameObject fisico;
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
    public Transform puntoGolpePatada;
    public Transform puntoGolpeEspada;

    [Header("❤️ Estado del Jugador")]
    [SerializeField] private int _vidaMaxima = 100;
    private int _vidaActualInternal;
    public bool estaMuerto = false;
    private bool hitboxGenerada = false;
    public Vector3 Destino;

    [Header("💰 Sistema de Monedas")]
    private int _cantidadDeMonedasInternal;
    [SerializeField] private TextMeshProUGUI textoMonedasUI;


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
    public AudioSource RecibirDanoAudio;
    public AudioSource SonidoDeMorir;
    public GameObject FondoOscuroSangriendo;

    [Header("⏱️ Cooldown Interno (barra horizontal)")]
    public float maxCoolDown = 0f;
    public UnityEngine.UI.Image barraCoolDown;

    [Header("🎯 Referencia a TimerManager (cooldowns de habilidades)")]
    public TimerManager _TimerManager;

    [Header("🛠️ Indicadores Melee/Rango")]
    public GameObject IndicadoresMelee;
    public GameObject IndicadoresMagicos;
    public GameObject Flecha;
    public ModoPelea modoActual = ModoPelea.Rango;

    [Header("💥 Combos")]
    public List<string> AtaquesQImpactaron = new List<string>();
    public float TimerDeCombos = 0f;
    public Transform OrigenDelDisparo;
    public GameObject PrefabDeComboElectrico;
    public GameObject PrefabDeComboFuego;
    public GameObject VFX_ComboExplosion;
    public AudioClip SonidoComboExplosion;
    public GameObject VFX_EfectoDeCuracion;
    public AudioClip VFX_A_EfectoDeCuracion;

    [Header("👣 Sonidos de Pasos")]
    public AudioClip SonidoDeCaminarEnHielo;
    public AudioClip SonidoDeCaminarEnPasto;

    #endregion

    #region Eventos y Delegados
    public delegate void AccionAtaqueDelegado(Vector3 objetivo);
    public static event Action<int> AlRecibirDano;
    public static event Action<int> AlCurarse;
    public static event Action<int> AlObtenerMonedas;
    #endregion

    private float _coolDown = 0f;
    public Vector3 DestinoGuardado;
    private static float tiempoInicio;
    private Dictionary<TipoAtaque, AccionAtaqueDelegado> _accionesAtaque;

    #region Propiedades Públicas (Getters y Setters con Feedbacks)

    public float CoolDown
    {
        get => _coolDown;
        set
        {
            _coolDown = value;
            if (_coolDown > maxCoolDown) maxCoolDown = _coolDown;
            ActualizarBarraCoolDown();
        }
    }

    public int vidaMaxima
    {
        get => _vidaMaxima;
        set => _vidaMaxima = value;
    }

    public int vidaActual
    {
        get => _vidaActualInternal;
        set
        {
            if (estaMuerto) return;

            int vidaPrevia = _vidaActualInternal;
            _vidaActualInternal = Mathf.Clamp(value, 0, _vidaMaxima);

            if (_vidaActualInternal < vidaPrevia) 
            {
                int danoRecibido = vidaPrevia - _vidaActualInternal;
                Debug.Log($"Recibió {danoRecibido} de daño, vida actual: {_vidaActualInternal}");

                Feedbacks.FeedbackRadialVisual(Color_RecibeDano, 1f);

                RecibirDanoAudio.Play();
                EfectoDeRelentizarTiempo();

                AlRecibirDano?.Invoke(danoRecibido);

                if (_vidaActualInternal <= 0)
                {
                    Morir();
                }
            }
            else if (_vidaActualInternal > vidaPrevia) 
            {
                int curacion = _vidaActualInternal - vidaPrevia;
                Debug.Log($"Se curó {curacion} de vida, vida actual: {_vidaActualInternal}");

                Feedbacks.FeedbackRadialVisual(Color_SeCura, 1.5f);

                AlCurarse?.Invoke(curacion);
            }
        }
    }

    public int CantidadDeMonedas
    {
        get => _cantidadDeMonedasInternal;
        set
        {
            if (value > _cantidadDeMonedasInternal)
            {
                _cantidadDeMonedasInternal = value;

                Debug.Log($"Monedas obtenidas. Total: {_cantidadDeMonedasInternal}");

                Coins?.Play();
                Feedbacks.FeedbackRadialVisual(Color_ObtieneMonedas, 0.8f);

                ActualizarTextoMonedas();

                AlObtenerMonedas?.Invoke(_cantidadDeMonedasInternal);
            }
        }
    }

    public int ContadorDeMonedas => CantidadDeMonedas;

    #endregion

    void Start()
    {
        _vidaActualInternal = _vidaMaxima;
        _cantidadDeMonedasInternal = 0;
        ActualizarTextoMonedas();

        InicializarAccionesDeAtaque();
        if (_TimerManager == null) Debug.LogWarning("[AccionesJugador] No asignaste TimerManager en el Inspector.");
        if (espada != null) espada.SetActive(false);
    }

    public void RecibirDanio(int cantidad)
    {
        vidaActual -= cantidad; 
    }

    public void SumarMonedas(int cantidad)
    {
        CantidadDeMonedas += cantidad; 
    }

    public void CrearEfectoDeCuracion()
    {
        if (estaMuerto || VFX_EfectoDeCuracion == null) return;
        if (vidaActual < vidaMaxima)
        {
            vidaActual += 20;

            GameObject efectoDeCuracion = Instantiate(VFX_EfectoDeCuracion, transform.position, Quaternion.identity);
            efectoDeCuracion.transform.SetParent(transform);
            AudioManager.CrearEfectoSonoro(transform.position, VFX_A_EfectoDeCuracion);
            Destroy(efectoDeCuracion, 2f);
        }
    }

    public void ActualizarTextoMonedas()
    {
        if (textoMonedasUI != null)
        {
            textoMonedasUI.text = _cantidadDeMonedasInternal.ToString();
        }
    }

    #region Lógica de Combate Principal (Refactorizada)

    private void InicializarAccionesDeAtaque()
    {
        _accionesAtaque = new Dictionary<TipoAtaque, AccionAtaqueDelegado>
        {
            { TipoAtaque.BolaDeFuego, AtacarBolaDeFuego },
            { TipoAtaque.Melee1, AtacarBolaDeFuego },
            { TipoAtaque.BolaDeHielo, AtacarBolaDeHielo },
            { TipoAtaque.Melee2, AtacarBolaDeHielo },
            { TipoAtaque.RayoElectrico, AtacarRayo },
            { TipoAtaque.Melee3, AtacarRayo }
        };
    }
    void Awake()
    {


        if (trail != null)
        {
            trail.time = trailTime;
            trail.Clear();
            trail.emitting = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) { AtaqueHardCodeado(); }
        if (Input.GetKeyDown(KeyCode.F2)) { AtaqueHardCodeado2(); }

        GestionarModoDeCombate();
        CargarBarraDeCoolDown();
        CorrerTimerDeCombos();

        float velocidadActual = agent.velocity.magnitude;
        anim.SetFloat("velocidad", velocidadActual);
        if (Vector3.Distance(transform.position, Destino) < 2f)
        {
            Detenerse();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (_TimerManager.magiaBloqueadaPorZona)
            {
                if (modoActual == ModoPelea.Melee)
                    return;

                modoActual = ModoPelea.Melee;
                _TimerManager.enModoMagico = false;

                ActualizarHUDModoMelee();
                return;
            }

            CambiarModoDeCombate();
        }

        if (modoActual == ModoPelea.Melee)
        {
            if (IndicadoresMelee != null && !IndicadoresMelee.activeSelf)
                IndicadoresMelee.SetActive(true);

            if (IndicadoresMelee != null)
            {
                foreach (Transform t in IndicadoresMelee.transform)
                    if (!t.gameObject.activeSelf) t.gameObject.SetActive(true);
            }

            if (IndicadoresMagicos != null && IndicadoresMagicos.activeSelf)
                IndicadoresMagicos.SetActive(false);

            if (espada != null && !espada.activeSelf)
                espada.SetActive(true);
        }



        RotarFlechaHaciaElCursor();
    }
    public override void Atacar(Vector3 Destino, string Nombre)
    {
        transform.LookAt(Destino);
        Debug.Log("Iniciando ataque...");
        if (estaMuerto || _TimerManager == null || _TimerManager.IsTimerCharging(6))
        {
            return;
        }

        if (Enum.TryParse<TipoAtaque>(Nombre, true, out var tipoAtaque))
        {
            if (_accionesAtaque.TryGetValue(tipoAtaque, out var accion))
            {
                _TimerManager.SetTimerToMax(6);
                accion(Destino);
            }
            else
            {
                Debug.LogWarning($"Ataque '{Nombre}' no encontrado en el diccionario de acciones.");
            }
        }
        else
        {
            Debug.LogWarning($"Nombre de ataque desconocido: {Nombre}");
        }
    }

    private void AtacarBolaDeFuego(Vector3 destino)
    {
        if (modoActual == ModoPelea.Rango)
        {
            if (_TimerManager.IsTimerCharging(0)) return;
            GameObject prefabDelAtaque;
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
            InstanciarProyectil(prefabDelAtaque, "BolaDeFuego", destino);
            Invoke(nameof(RegistrarCoolDown), 0.1f);
        }
        else 
        {
            if (_TimerManager.IsTimerCharging(3)) return;
            anim.SetTrigger("melee1");
            _TimerManager.SetTimerToMax(3);
            if (PasoTiempo(2)) CrearEfectoDeExplosion();
        }
    }

    private void AtacarBolaDeHielo(Vector3 destino)
    {
        if (modoActual == ModoPelea.Rango)
        {
            if (_TimerManager.IsTimerCharging(1)) return;
            anim.SetTrigger("magic2");
            _TimerManager.SetTimerToMax(1);
            InstanciarProyectil(BolaDeHielo, "BolaDeHielo", destino);
            Invoke(nameof(RegistrarCoolDown), 0.1f);
        }
        else // Melee
        {
            if (_TimerManager.IsTimerCharging(4)) return;
            anim.SetTrigger("melee2");
            _TimerManager.SetTimerToMax(4);
        }
    }

    private void AtacarRayo(Vector3 destino)
    {
        if (modoActual == ModoPelea.Rango)
        {
            if (_TimerManager.IsTimerCharging(2)) return;
            GameObject prefabDelAtaque;
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
            InstanciarProyectil(prefabDelAtaque, "RayoElectrico", destino);
            Invoke(nameof(RegistrarCoolDown), 0.1f);
        }
        else 
        {
            if (_TimerManager.IsTimerCharging(5)) return;
            anim.SetTrigger("melee3");
            _TimerManager.SetTimerToMax(5);
        }
    }

    private void InstanciarProyectil(GameObject prefab, string nombreAtaque, Vector3 destino)
    {
        transform.LookAt(destino);
        Vector3 direccion = (destino - Origen.position).normalized;
        Quaternion rotacion = Quaternion.LookRotation(direccion);

        GameObject ataqueInstanciado = Instantiate(prefab, Origen.position, rotacion);
        ataqueInstanciado.name = nombreAtaque;
        Debug.Log($"{ataqueInstanciado.name} instanciado en {Origen.position}", ataqueInstanciado);

        if (ataqueInstanciado.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(direccion * fuerzaDisparo);
        }

        var proyectil = ataqueInstanciado.GetComponent<Proyectil>();
        if (proyectil != null) proyectil.Creador = gameObject;

        var hitbox = ataqueInstanciado.GetComponent<Hitbox>();
        if (hitbox != null) hitbox.Creador = gameObject;

        Destroy(ataqueInstanciado, 10f);
    }
    #endregion

    #region Gestión de Modos (Melee/Rango)

    private void GestionarModoDeCombate()
    {
        if (_TimerManager.magiaBloqueadaPorZona)
        {
            if (_TimerManager.enModoMagico)
            {
                _TimerManager.enModoMagico = false;
            }
        }
        modoActual = _TimerManager.enModoMagico ? ModoPelea.Rango : ModoPelea.Melee;
    }

    public void CambiarModoDeCombate()
    {
        if (TimerManager.Controler.enTransicionVisual) return;

        // Alternar modo
        modoActual = (modoActual == ModoPelea.Rango) ? ModoPelea.Melee : ModoPelea.Rango;

        _TimerManager.TransicionarModoVisual();
        _TimerManager.enModoMagico = (modoActual == ModoPelea.Rango);

        if (modoActual == ModoPelea.Melee)
        {
            Debug.Log("Modo cambiado a MELEE");

            anim.SetLayerWeight(0, 0f);
            anim.SetLayerWeight(1, 1f);

            if (espada != null) espada.SetActive(true);

            if (IndicadoresMelee != null) IndicadoresMelee.SetActive(true);
            if (IndicadoresMagicos != null) IndicadoresMagicos.SetActive(false);
        }
        else 
        {
            Debug.Log("Modo cambiado a RANGO");

            anim.SetLayerWeight(0, 1f);
            anim.SetLayerWeight(1, 0f);

            if (espada != null) espada.SetActive(false);

            if (IndicadoresMelee != null) IndicadoresMelee.SetActive(false);
            if (IndicadoresMagicos != null) IndicadoresMagicos.SetActive(true);
        }
    }

    #endregion

    #region Salud y Daño
    // El método ahora es más simple, solo invoca al setter de la propiedad vidaActual.


    public void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;
        Feedbacks.FeedbackRadialVisual(Color_Muere, 4);
        anim.SetTrigger("life");
        SonidoDeMorir.Play();
        FondoOscuroSangriendo.SetActive(true);
        Invoke(nameof(CargaEscenaDerrota), 3f);
    }

    void CargaEscenaDerrota() => SceneManager.LoadScene("Derrota");
    public void EfectoDeRelentizarTiempo()
    {
        Time.timeScale = 0.6f;
        Invoke(nameof(RestablecerTiempo), 0.5f);
    }
    public void RestablecerTiempo() => Time.timeScale = 1f;

    #endregion

    #region Sistema de Monedas
    // El método ahora es más simple, solo invoca al setter de la propiedad.

    #endregion


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
                    float fuerzaEmpuje = 5f;
                    if (col.TryGetComponent<NavMeshAgent>(out var agent))
                    {
                        if (NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                        {
                            agent.Warp(hit.position);
                            agent.enabled = false;
                            rb.AddForce(direccion * fuerzaEmpuje, ForceMode.Impulse);
                            StartCoroutine(ReactivarAgente(agent, 1f));
                        }
                    }
                    else
                    {
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
    #endregion

    #region Movimiento y Navegación
    public override void IrAlDestino(Vector3 destino)
    {
        if (estaMuerto || Vector3.Distance(destino, DestinoGuardado) < 2) return;

        DestinoGuardado = destino;
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

    public override void Detenerse()
    {
        agent.isStopped = true;
        if (S_Caminar.isPlaying)
        {
            S_Caminar.Stop();
        }
    }
    #endregion

    #region Sistema de Combos
    void CorrerTimerDeCombos()
    {
        if (TimerDeCombos > 0f)
        {
            TimerDeCombos -= Time.deltaTime;
        }
        else if (TimerDeCombos <= 0f)
        {
            AtaquesQImpactaron.Clear();
        }
    }
    public void CrearEfectoDeExplosion()
    {
        if (estaMuerto || VFX_ComboExplosion == null || SonidoComboExplosion == null) return;

        Feedbacks.Componente.UIFadeComboScript.MostrarTexto("¡ONDA FLAMEANTE!", new Color(1f, 0.352f, 0f));
        Vector3 origenDeEfecto = transform.position;
        Instantiate(VFX_ComboExplosion, origenDeEfecto, Quaternion.identity);
        AudioManager.CrearEfectoSonoro(transform.position, SonidoComboExplosion);
    }

    #endregion

    #region UI y Feedbacks Visuales
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

    #region Resto de Métodos (sin cambios lógicos)
    public void ActivarEfectoPatada()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, puntoDeImpacto.position, Quaternion.identity);
        }
    }
    public void ActivarTrail() { if (trail != null) { trail.Clear(); trail.emitting = true; } }
    public void DesactivarTrail() { if (trail != null) trail.emitting = false; }
    public void RegistrarCoolDown()
    {
        float speed = 1f;
        if (anim.GetCurrentAnimatorClipInfo(0).Length > 0)
        {
            var clipInfo = anim.GetCurrentAnimatorClipInfo(0)[0];
            string nombreAnimacion = clipInfo.clip.name;
            if (nombreAnimacion.Contains("01")) speed = 8f;
            else if (nombreAnimacion.Contains("02")) speed = 4f;
            else if (nombreAnimacion.Contains("03")) speed = 4f;
            else if (nombreAnimacion.Contains("ataque_pesadoPersonaje")) speed = 2f;
            else if (nombreAnimacion.Contains("ataque_rapidoPersonaje")) speed = 3f;
            else if (nombreAnimacion.Contains("ataque_fuertePersonaje")) speed = 0.9f;
            float tiempoAnim = clipInfo.clip.length / speed;
            CoolDown = Mathf.Min(tiempoAnim, 2f);
        }
    }
    private void ActualizarHUDModoMelee()
    {
        if (IndicadoresMelee != null) IndicadoresMelee.SetActive(true);
        if (IndicadoresMagicos != null) IndicadoresMagicos.SetActive(false);

        if (espada != null) espada.SetActive(true);

        anim.SetLayerWeight(0, 0f);
        anim.SetLayerWeight(1, 1f);
    }
    public void ForzarModoMeleeVisual()
    {
        modoActual = ModoPelea.Melee;
        if (_TimerManager != null) _TimerManager.enModoMagico = false;

        if (anim != null)
        {
            anim.SetLayerWeight(0, 0f);
            anim.SetLayerWeight(1, 1f);
        }
        if (espada != null) espada.SetActive(true);

        if (IndicadoresMelee != null)
        {
            IndicadoresMelee.SetActive(true);

            foreach (Transform t in IndicadoresMelee.transform)
                t.gameObject.SetActive(true);
        }

        if (IndicadoresMagicos != null) IndicadoresMagicos.SetActive(false);
    }

    public void ForzarRefrescoHUDAlSalirDeZona()
    {
        if (modoActual == ModoPelea.Rango)
        {
            if (IndicadoresMagicos != null) IndicadoresMagicos.SetActive(true);
            if (IndicadoresMelee != null) IndicadoresMelee.SetActive(false);
            if (espada != null) espada.SetActive(false);
            if (anim != null) { anim.SetLayerWeight(0, 1f); anim.SetLayerWeight(1, 0f); }
        }
        else
        {
            ForzarModoMeleeVisual();
        }
    }



    public override void OnCollision(Collision collider) { }
    public override void Colisiono(GameObject col, string TipoDeColision)
    {
        if (col.TryGetComponent<A3_Interactuable>(out var interactivo))
        {
            interactivo.Interactuar();
        }
    }
    public void ReproducirSonidoDeCaminar()
    {
        float posicionZ = Mathf.Abs(transform.position.z);
        if (posicionZ < 195 && posicionZ > 175) S_Caminar.clip = SonidoDeCaminarEnHielo;
        else S_Caminar.clip = SonidoDeCaminarEnPasto;
    }
    public static void RegistrarAhora() => tiempoInicio = Time.time;
    public static bool PasoTiempo(float segundos) => Time.time - tiempoInicio <= segundos;
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
        if (cubo.TryGetComponent<Renderer>(out var renderer)) renderer.material.color = Color.yellow;
        if (cubo.TryGetComponent<BoxCollider>(out var collider)) collider.enabled = false;
        cubo.transform.rotation = transform.rotation;
        Destroy(cubo, 2f);
    }
    #endregion
}