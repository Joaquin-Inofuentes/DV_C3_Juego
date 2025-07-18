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
    // TP 2_Inofuentes Joaquin
    //get y set
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
    public UnityEngine.UI.Image barraCoolDown; // Opcional: si ya no necesitás la barra horizontal, dejala en null

    [Header("🎯 Referencia a TimerManager (cooldowns de habilidades)")]
    public TimerManager _TimerManager;

    [Header("🖼️ Íconos de Habilidad con Fill (Tipo: Filled, Radial 360)")]
    /// <summary>
    /// Índices:
    /// 0 ▶ Bola de Fuego
    /// 1 ▶ Bola de Hielo
    /// 2 ▶ Rayo
    /// 3 ▶ Melee1
    /// 4 ▶ Melee2
    /// 5 ▶ Melee3
    /// 6 ▶ Cooldown “general” (opcional)
    /// </summary>
    /// 
    [Header("🛠️ Indicadores Melee/Rango")]
    public GameObject IndicadoresMelee;

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

        if (Input.GetKeyDown(KeyCode.F1))
        {
            AtaqueHardCodeado();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            AtaqueHardCodeado2();
        }

        if (_TimerManager.magiaBloqueadaPorZona)
        {
            if (_TimerManager.enModoMagico)
            {
                _TimerManager.enModoMagico = false;
               
            }
        }

        modoMelee = !_TimerManager.enModoMagico;



        // 1) Reducir y actualizar el CoolDown interno (barra horizontal)
        CargarBarraDeCoolDown();


        // 3) Actualizar animación de movimiento y detenerse si llegó
        float velocidadActual = agent.velocity.magnitude;
        anim.SetFloat("velocidad", velocidadActual);
        if (Vector3.Distance(transform.position, Destino) < 2f)
        {
            Detenerse();
        }

        // 4) Alternar modo melee/rango con la tecla 4 (ejemplo)
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (!modoMelee)
            {
                modoMelee = true;

                if (!TimerManager.Controler.enTransicionVisual && Input.GetKeyDown(KeyCode.Alpha4))
                {
                    // Cambiar de modo
                    TimerManager.Controler.TransicionarModoVisual();
                    TimerManager.Controler.enModoMagico = !TimerManager.Controler.enModoMagico;
                }

                Debug.Log("Modo cambiado a MELEE");
                anim.SetLayerWeight(0, 0f);
                anim.SetLayerWeight(1, 1f);
                if (espada != null)
                    espada.SetActive(true);
                IndicadoresMelee.SetActive(true);
            }

            else if (modoMelee)
            {
                // Cambiar a modo magia
                modoMelee = false;
                // 🔁 Ejecutar animación visual
                _TimerManager.TransicionarModoVisual();

                _TimerManager.enModoMagico = true;
                Debug.Log("Modo cambiado a RANGO");
                anim.SetLayerWeight(0, 1f); // capa 0 = Rango
                anim.SetLayerWeight(1, 0f); // capa 1 = Melee
                if (espada != null)
                {
                    espada.SetActive(false);
                }
                IndicadoresMelee.SetActive(false);


            }
        }


        // 5) Rotar flecha hacia cursor si existe
        RotarFlechaHaciaElCursor();
        CorrerTimerDeCombos();
    }

    /// <summary>
    /// Reduce _coolDown cada frame y actualiza la barra horizontal (si está asignada).
    /// </summary>
    void CargarBarraDeCoolDown()
    {
        if (_coolDown > 0f)
            _coolDown -= Time.deltaTime;

        if (_coolDown < 0f)
            _coolDown = 0f;

        ActualizarBarraCoolDown();
    }

    /// <summary>
    /// Actualiza la barra horizontal de TimerDeCombos (opcional).
    /// Si no se usa, dejar barraCoolDown en null en el Inspector.
    /// </summary>
    private void ActualizarBarraCoolDown()
    {
        if (barraCoolDown == null || maxCoolDown == 0f) return;

        float porcentaje = 1f - Mathf.Clamp01(_coolDown / maxCoolDown);
        float anchoMaximo = 200f;
        barraCoolDown.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            anchoMaximo * porcentaje
        );
    }

    public List<string> AtaquesQImpactaron = new List<string>();
    public float TimerDeCombos = 0f;
    void CorrerTimerDeCombos()
    {
        if (TimerDeCombos > 0f)
            TimerDeCombos -= Time.deltaTime;
        if (TimerDeCombos <= 0f)
        {
            AtaquesQImpactaron.Clear(); // Reiniciar lista de ataques
        }
    }

    public Transform OrigenDelDisparo;
    public void AtaqueHardCodeado()
    {
        AtaquesQImpactaron = new List<string> { "Melee1", "Melee2" };
        TimerDeCombos = 2f; // Duración del combo
        Vector3 Posicion = OrigenDelDisparo.position;
        CrearCuboGenerico(Posicion);
        Atacar(Posicion, "BolaDeFuego");
    }

    public void AtaqueHardCodeado2()
    {
        AtaquesQImpactaron = new List<string> { "Melee3", "Melee2" };
        TimerDeCombos = 2f; // Duración del combo
        //gameObject.transform.LookAt(GameManager.PosicionDelMouseEnElEspacio);
        Vector3 Posicion = OrigenDelDisparo.position;
        CrearCuboGenerico(Posicion);
        Atacar(Posicion, "RayoElectrico");
    }



    // Pseudocódigo:
    // 1. Instanciar un cubo genérico en la posición del jugador (o donde desees).
    // 2. Guardar la referencia al cubo instanciado.
    // 3. Destruir el cubo después de 2 segundos.

    public void CrearCuboGenerico(Vector3 Posicion)
    {
        // Crear un cubo genérico en la posición del jugador
        GameObject cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubo.transform.position = Posicion;

        // Opcional: ajustar tamaño, color, etc.
        cubo.transform.localScale = Vector3.one;
        cubo.GetComponent<Renderer>().material.color = Color.yellow;
        cubo.GetComponent<BoxCollider>().enabled = false; // Desactivar colisión si no es necesario
        cubo.transform.rotation = transform.rotation; // Asegurar rotación por defecto
        // Destruir el cubo después de 2 segundos
        Destroy(cubo, 2f);
    }

    public GameObject PrefabDeComboElectrico;
    public GameObject PrefabDeComboFuego;

    public Hitbox Llamarada;
    public Hitbox RayoElectrico;
    /// <summary>
    /// Actualiza el fillAmount de cada ícono según los valores de TimerManager.
    /// fillAmount = 1 → ícono completamente cubierto (TimerDeCombos recién iniciado).
    /// fillAmount = 0 → ícono completamente libre (TimerDeCombos terminado).
    /// </summary>
    public override void Atacar(Vector3 Destino, string Nombre)
    {
        if (estaMuerto) return;
        if (_TimerManager == null) return;

        // 1) Chequear TimerDeCombos “general” (índice 6)
        if (_TimerManager.IsTimerCharging(6)) return;
        _TimerManager.SetTimerToMax(6);
        float TiempoParaDestuirse = 3;
        GameObject ProyectilUsado = null;
        // --- HECHIZOS / ATAQUES ---
        if (Nombre == "BolaDeFuego" || Nombre == "Melee1")
        {
            if (!modoMelee) // Modo magico
            {
                if (_TimerManager.IsTimerCharging(0)) return;
                // Revisa el timer
                // Sustituye la línea problemática por una comparación manual de la lista
                if (TimerDeCombos > 0 && AtaquesQImpactaron.Count == 2
                    && AtaquesQImpactaron[0] == "Melee1"
                    && AtaquesQImpactaron[1] == "Melee2")
                {
                    Debug.Log("Se usara combo llamarada");
                    ProyectilUsado = PrefabDeComboFuego; // Asigna el proyectil de combo de fuego
                    AtaquesQImpactaron.Clear(); // Limpia la lista de ataques
                    TimerDeCombos = 0f; // Reinicia el timer de combos
                    TiempoParaDestuirse = 3f;
                }
                // Ejecuta la funcion de ataque combo llamarada

                // else ejecuta la otra
                else
                {
                    ProyectilUsado = BolaDeFuego;
                }

                anim.SetTrigger("magic1");
                _TimerManager.SetTimerToMax(0);
                RegistrarAhora();
            }
            else
            {
                if (_TimerManager.IsTimerCharging(3)) return;
                anim.SetTrigger("melee1");
                _TimerManager.SetTimerToMax(3);
                if (PasoTiempo(2))
                {
                    CrearEfectoDeExplosion();
                }
            }
        }
        else if (Nombre == "BolaDeHielo" || Nombre == "Melee2")
        {
            if (!modoMelee)
            {
                if (_TimerManager.IsTimerCharging(1)) return;
                anim.SetTrigger("magic2");
                ProyectilUsado = BolaDeHielo;
                _TimerManager.SetTimerToMax(1);
            }
            else
            {
                Debug.Log("Instanciando efecto de patada");
                if (_TimerManager.IsTimerCharging(4)) return;
                anim.SetTrigger("melee2");
                _TimerManager.SetTimerToMax(4);
            }
            anim.SetFloat("velocidad", 0f);
            agent.isStopped = true;
        }
        else if (Nombre == "RayoElectrico" || Nombre == "Melee3")
        {
            if (!modoMelee) // Modo magico
            {
                if (_TimerManager.IsTimerCharging(2)) return;
                if (TimerDeCombos > 0 && AtaquesQImpactaron.Count == 2
                    && AtaquesQImpactaron[0] == "Melee3"
                    && AtaquesQImpactaron[1] == "Melee2")
                {
                    Debug.Log("Se usara electrico combo");
                    ProyectilUsado = PrefabDeComboElectrico; // Asigna el proyectil de combo de rayo
                    AtaquesQImpactaron.Clear(); // Limpia la lista de ataques
                    TimerDeCombos = 0f; // Reinicia el timer de combos
                    TiempoParaDestuirse = 3;
                }
                else
                {
                    ProyectilUsado = Rayo;
                }
                anim.SetTrigger("magic3");
                _TimerManager.SetTimerToMax(2);
            }
            else
            {
                if (_TimerManager.IsTimerCharging(5)) return;
                anim.SetTrigger("melee3");
                _TimerManager.SetTimerToMax(5);
            }
            anim.SetFloat("velocidad", 0f);
            agent.isStopped = true;
        }

        // 2) Instanciar proyectil si corresponde
        transform.LookAt(Destino);
        if (ProyectilUsado == null) return;

        Vector3 direccion = (Destino - Origen.position).normalized;
        GameObject ataque = Instantiate(
            ProyectilUsado,
            Origen.position,
            Quaternion.LookRotation(direccion)
        );


        if (TiempoParaDestuirse > 0)
        {
            Debug.Log("Se empezo a registrar el acciones de jugador");
            ataque.transform.position = Origen.position;
            Destroy(ataque, TiempoParaDestuirse);
            if (Nombre == "RayoElectrico")
            {
                RayoElectrico.gameObject.SetActive(true);
                RayoElectrico.AutoDesactivarse = true; // Asegurarse de que se desactive automáticamente
                RayoElectrico.TiempoDeVida = TiempoParaDestuirse;
                //RayoElectrico.AccionesJugadorAsociadas = gameObject.GetComponent<AccionesJugador>();
                ataque.gameObject.name = Nombre;
            }
            else
            {
                Llamarada.gameObject.SetActive(true);
                Llamarada.AutoDesactivarse = true; // Asegurarse de que se desactive automáticamente
                Llamarada.TiempoDeVida = TiempoParaDestuirse;
                //Llamarada.AccionesJugadorAsociadas = gameObject.GetComponent<AccionesJugador>();
                ataque.gameObject.name = Nombre;
            }

        }

        if (ataque.TryGetComponent<Proyectil>(out var proyectil))
        {
            proyectil.Creador = gameObject;
            proyectil.AutoDestruir = true;
        }

        if (ataque.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(direccion * fuerzaDisparo);
        }

        // 3) Registrar TimerDeCombos interno basado en animación
        Invoke(nameof(RegistrarCoolDown), 0.1f);
    }

    /// <summary>
    /// Calcula la duración del CoolDown interno según la animación actual.
    /// </summary>
    public void RegistrarCoolDown()
    {
        float speed = 1f;
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

    public void GenerarHitboxAtaqueRapido() => GenerarHitbox(puntoGolpeEspada, 15);
    public void GenerarHitboxAtaquePesado() => GenerarHitbox(puntoGolpeEspada, 35);
    public void GenerarHitboxPie()
    {
        GenerarHitbox(puntoGolpePatada, 10);
        EmpujarEnemigosConPatada();
    }

    public void GenerarHitbox(Transform puntoDeGolpe, int danio)
    {
        if (hitboxGenerada) return;

        if (hitboxCuboPrefab != null && puntoDeGolpe != null)
        {
            GameObject hitbox = Instantiate(
                hitboxCuboPrefab,
                puntoDeGolpe.position,
                gameObject.transform.rotation
            );
            if (hitbox.TryGetComponent<Hitbox>(out var componenteHitbox))
                componenteHitbox.ConfigurarDanio(danio);

            hitboxGenerada = true;
            Destroy(hitbox, 0.5f);
            Invoke(nameof(ResetHitboxFlag), 0.1f);
        }
    }
    void EmpujarEnemigosConPatada()
    {
        Collider[] enemigos = Physics.OverlapSphere(puntoGolpePatada.position, 2f);

        foreach (var col in enemigos)
        {
            var enemigo = col.GetComponent<A1_A1_H1_MoustroDelAverno>();//interface Idamageable
            if (enemigo != null)
            {
                Rigidbody rb = col.attachedRigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    Vector3 direccion = (col.transform.position - transform.position).normalized;


                    NavMeshAgent agent = col.GetComponent<NavMeshAgent>();
                    if (agent != null)
                    {
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(agent.transform.position, out hit, 2f, NavMesh.AllAreas))
                        {
                            agent.Warp(hit.position);
                            agent.enabled = false;

                            // Aumentar fuerza para probar
                            float fuerzaEmpuje = 5f;
                            rb.AddForce(direccion * fuerzaEmpuje, ForceMode.Impulse);

                            Debug.Log($"Empujando enemigo {col.name} con fuerza {direccion * fuerzaEmpuje}");

                            StartCoroutine(ReactivarAgente(agent, 1f));
                        }
                    }
                    else
                    {
                        // Sin NavMeshAgent, aplicar fuerza normal
                        float fuerzaEmpuje = 5f;
                        rb.AddForce(direccion * fuerzaEmpuje, ForceMode.Impulse);
                        Debug.Log($"Empujando enemigo sin NavMeshAgent {col.name} con fuerza {direccion * fuerzaEmpuje}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Rigidbody no válido o kinematic para {col.name}");
                }
            }
        }
    }


    private IEnumerator ReactivarAgente(NavMeshAgent agent, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (agent != null)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(agent.transform.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                agent.enabled = true;
                agent.isStopped = false; // aseguramos que esté activo y listo para moverse
            }
            else
            {
                Debug.LogWarning("No se encontró posición válida para reactivar agente.");
                // Como fallback, podrías desactivar agente o manejarlo distinto aquí.
            }
        }
    }






    private void ResetHitboxFlag()
    {
        hitboxGenerada = false;
    }

    public override void Detenerse()
    {
        agent.isStopped = true;
        S_Caminar.loop = false;
        S_Caminar.Stop();
    }

    public Vector3 DestinoGuardado;
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
        if (!Particulas)
        {
            //Debug.Log("Falta particulas de caminar");
        }

        // Reproducir sonido si no está sonando
        if (!S_Caminar.isPlaying)
        {
            //S_Caminar.clip = AudioManager.ObtenerAudioPorNombre("Correr_en_pasto");
            S_Caminar.loop = true;
            S_Caminar.Play();
        }

    }



    public override void OnCollision(Collision collider)
    {
        // Implementar si hace falta
    }
    //TP2_DamianFigueredo interface de vida
    //get y set de vida
    public int vidaMaxima
    {
        get => VidaMax;
    }
    public int vidaActual
    {
        get => Vida;
        set => Vida = value;
    }
    public AudioSource RecibirDanoAudio;
    public override void RecibirDanio(int cantidad)
    {
        Vida -= cantidad;
        Debug.Log("recibio" + cantidad + "daño, su vida actual es " + vidaActual);
        Feedbacks.FeedbackRadialVisual(Color_RecibeDano, 1);
        EfectoDeRelentizarTiempo();
        if (Vida <= 0)
        {
            Morir();
            Invoke(nameof(CargaEscenaDerrota), 3f);
        }
        RecibirDanoAudio.Play();
    }
    public AudioSource SonidoDeMorir;
    public GameObject FondoOscuroSangriendo;
    public override void Morir()
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



    /// <summary>
    /// Rotar la flecha hacia el cursor (solo en el plano XZ).
    /// </summary>
    public GameObject Flecha;
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
    void Awake()
    {
        if (trail == null) return;
        // Configuramos el tiempo de vida del rastro
        trail.time = trailTime;

        // Arrancamos limpios y sin emitir
        trail.Clear();
        trail.emitting = false;
    }
    public void ActivarEfectoPatada()
    {
        Instantiate(explosionPrefab, puntoDeImpacto.position, Quaternion.identity);
    }
    public void ActivarTrail()
    {
        // Limpia cualquier resto antiguo
        trail.Clear();
        // Empieza a emitir vértices de rastro
        trail.emitting = true;
    }

    public void DesactivarTrail()
    {
        // Solo paramos la emisión de nuevos vértices.
        // NO desactivamos 'enabled' ni volvemos a llamar a Clear().
        trail.emitting = false;
        // Los polígonos generados seguirán vivos y se irán desvaneciendo
        // automáticamente durante 'trail.time' segundos.
    }
    private IEnumerator ClearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        trail.Clear();
    }

    public override void Colisiono(GameObject col, string TipoDeColision)
    {
        /*
        Debug.Log(
            "El _" 
            + Colision.name  
            + "_ Colisiona con _" 
            + gameObject.name 
            + "_ Con _" 
            + TipoDeColision 
            + "_ Tipo de colision"
            , gameObject);
        */
        // El _Enemigo_ Colisiona con _Jugador v2_ Con _TriggerStay_ Tipo de colision
        A3_Interactuable interactivo = col.GetComponent<A3_Interactuable>();
        if (interactivo != null)
        {
            interactivo.Interactuar();
        }
        Debug.DrawLine(col.transform.position, gameObject.transform.position);
    }
    //TP 2_Damian Figueredo
    public int ContadorDeMonedas
    {
        get { return CantidadDeMonedas; }
    }
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



    public AudioClip SonidoDeCaminarEnHielo;
    public AudioClip SonidoDeCaminarEnPasto;
    public void ReproducirSonidoDeCaminar()
    {
        float PosicionEnZ = Math.Abs(transform.position.z);
        if (PosicionEnZ < 195
            && PosicionEnZ > 175
            )
            S_Caminar.clip = SonidoDeCaminarEnHielo;
        else
        {
            S_Caminar.clip = SonidoDeCaminarEnPasto;
        }
    }









    private static float tiempoInicio;

    public static void RegistrarAhora()
    {
        tiempoInicio = Time.time;
    }

    public static bool PasoTiempo(float segundos)
    {
        return Time.time - tiempoInicio <= segundos;
    }


    public GameObject VFX_ComboExplosion;
    public AudioClip SonidoComboExplosion;
    // Efecto de curación al lanzar el hechizo de fuego
    public void CrearEfectoDeExplosion() // Se activa por timer despues de lanzar el primer hechizo de fuego y atacar con melee
    {
        if (estaMuerto) return;
        if (VFX_ComboExplosion == null || SonidoComboExplosion == null) return;
        Feedbacks.Componente.UIFadeComboScript.MostrarTexto("¡ONDA FLAMEANTE!", new Color(1f, 0.352f, 0f));


        // Posicion del efecto: Origen del ataque
        Vector3 OrigenDeEfecto = transform.position;
        // Crea el efecto q daña a todos los enemigos cercanos
        GameObject efecto = Instantiate(VFX_ComboExplosion, OrigenDeEfecto, Quaternion.identity);
        // Crea efecto sonoro de combo
        AudioManager.CrearEfectoSonoro(transform.position, SonidoComboExplosion);
    }


    public GameObject VFX_EfectoDeCuracion;
    public AudioClip VFX_A_EfectoDeCuracion;
    public void CrearEfectoDeCuracion()
    {
        Debug.Log("Creando efecto de curación");
        if (estaMuerto) return;
        if (VFX_EfectoDeCuracion == null) return;
        // Posicion del efecto: Origen del ataque
        vidaActual += 20; // Curar al jugador al lanzar el hechizo de fuego
        if (vidaActual > vidaMaxima) vidaActual = vidaMaxima; // Asegurarse de no superar la vida máxima
        else if (vidaActual < vidaMaxima)
        {
            GameObject EfectoDeCuracion = Instantiate(VFX_EfectoDeCuracion, transform.position, Quaternion.identity);
            AudioManager.CrearEfectoSonoro(transform.position, VFX_A_EfectoDeCuracion);
            Destroy(EfectoDeCuracion, 2f); // Destruir el efecto después de 2 segundos
            EfectoDeCuracion.transform.SetParent(transform);
        }
    }
}
