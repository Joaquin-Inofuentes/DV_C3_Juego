using Drakkar.GameUtils;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Acjugador : A1_Entidad, IDaniable, IContadormonedas
{
    [Header("❤️ Estado del Jugador")]
    [SerializeField] private int _vidaMaxima = 100;
    private int _vidaActualInternal;
    private bool estaMuerto = false;
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
    public Image barraCoolDown;

    [Header("🛠️ Indicadores Melee/Rango")]
    public GameObject IndicadoresMelee;
    public GameObject Flecha;

    [Header("👣 Sonidos de Pasos")]
    public AudioClip SonidoDeCaminarEnHielo;
    public AudioClip SonidoDeCaminarEnPasto;

    [Header("🔗 Referencias externas")]
    public TimerManager _TimerManager;
    public AtaqueJugador ataqueJugador; // referencia al nuevo script de ataque

    #region Eventos y Delegados
    public static event Action<int> AlRecibirDano;
    public static event Action<int> AlCurarse;
    public static event Action<int> AlObtenerMonedas;
    #endregion

    private float _coolDown = 0f;
    public Vector3 DestinoGuardado;
    private bool hitboxGenerada = false;

    #region Propiedades Públicas

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
                Feedbacks.FeedbackRadialVisual(Color_RecibeDano, 1f);
                RecibirDanoAudio.Play();
                EfectoDeRelentizarTiempo();
                AlRecibirDano?.Invoke(danoRecibido);

                if (_vidaActualInternal <= 0)
                    Morir();
            }
            else if (_vidaActualInternal > vidaPrevia)
            {
                int curacion = _vidaActualInternal - vidaPrevia;
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

        if (_TimerManager == null) Debug.LogWarning("[AccionesJugador] No asignaste TimerManager en el Inspector.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
          //  ataqueJugador.AtaqueHardCodeado();

        if (Input.GetKeyDown(KeyCode.F2))
           // ataqueJugador.AtaqueHardCodeado2();

        GestionarModoDeCombate();
        CargarBarraDeCoolDown();

        float velocidadActual = agent.velocity.magnitude;
        anim.SetFloat("velocidad", velocidadActual);
        if (Vector3.Distance(transform.position, Destino) < 2f)
            Detenerse();

        if (Input.GetKeyDown(KeyCode.Alpha4))
            CambiarModoDeCombate();

        RotarFlechaHaciaElCursor();
    }

    public void RecibirDanio(int cantidad) => vidaActual -= cantidad;
    public void SumarMonedas(int cantidad) => CantidadDeMonedas += cantidad;

    public void CrearEfectoDeCuracion()
    {
        if (estaMuerto) return;
        if (vidaActual < vidaMaxima)
            vidaActual += 20;
    }

    public void ActualizarTextoMonedas()
    {
        if (textoMonedasUI != null)
            textoMonedasUI.text = _cantidadDeMonedasInternal.ToString();
    }

    #region Gestión de Modos
    private void GestionarModoDeCombate()
    {
        if (_TimerManager.magiaBloqueadaPorZona && _TimerManager.enModoMagico)
            _TimerManager.enModoMagico = false;

        ataqueJugador.modoActual = _TimerManager.enModoMagico ? AtaqueJugador.ModoPelea.Rango : AtaqueJugador.ModoPelea.Melee;
    }

    private void CambiarModoDeCombate()
    {
        if (TimerManager.Controler.enTransicionVisual) return;

        ataqueJugador.modoActual = (ataqueJugador.modoActual == AtaqueJugador.ModoPelea.Rango)
            ? AtaqueJugador.ModoPelea.Melee
            : AtaqueJugador.ModoPelea.Rango;

        _TimerManager.TransicionarModoVisual();
        _TimerManager.enModoMagico = (ataqueJugador.modoActual == AtaqueJugador.ModoPelea.Rango);

        if (ataqueJugador.modoActual == AtaqueJugador.ModoPelea.Melee)
        {
            anim.SetLayerWeight(0, 0f);
            anim.SetLayerWeight(1, 1f);
            IndicadoresMelee.SetActive(true);
        }
        else
        {
            anim.SetLayerWeight(0, 1f);
            anim.SetLayerWeight(1, 0f);
            IndicadoresMelee.SetActive(false);
        }
    }
    #endregion

    #region Salud y Daño
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
    public void EfectoDeRelentizarTiempo() { Time.timeScale = 0.6f; Invoke(nameof(RestablecerTiempo), 0.5f); }
    public void RestablecerTiempo() => Time.timeScale = 1f;
    #endregion

    #region Hitboxes
    public void GenerarHitboxAtaqueRapido() => GenerarHitbox(puntoGolpeEspada, 15, "Melee1");
    public void GenerarHitboxAtaquePesado() => GenerarHitbox(puntoGolpeEspada, 35, "Melee3");
    public void GenerarHitboxPie()
    {
        GenerarHitbox(puntoGolpePatada, 10, "Melee2");
        EmpujarEnemigosConPatada();
    }
    public Transform puntoGolpePatada;
    public Transform puntoGolpeEspada;
    public GameObject hitboxCuboPrefab;

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
    private void ResetHitboxFlag() => hitboxGenerada = false;
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

    #region Movimiento
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
            S_Caminar.Stop();
    }
    #endregion

    #region UI
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
        barraCoolDown.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200f * porcentaje);
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
   public override void Atacar(Vector3 a, string ataque) { }
   public override void OnCollision(Collision collider) { }
   public override void Colisiono(GameObject colision, string tipoDeColision)
    {
        /* if (colision.TryGetComponent<IDaniable>(out var daniable))
        {
            daniable.RecibirDanio(10);
            Feedbacks.FeedbackRadialVisual(Color_FueAvistado, 1f);
        }*/
    }
    #endregion
}