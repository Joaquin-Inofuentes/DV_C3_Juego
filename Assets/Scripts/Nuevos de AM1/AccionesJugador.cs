using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necesario para Image

public class AccionesJugador : A1_Entidad
{
    [Header("🏹 Proyectiles y Hitboxes")]
    public GameObject BolaDeFuego;
    public GameObject BolaDeHielo;
    public GameObject Rayo;
    public GameObject ataqueRapido;
    public GameObject Flechazo;
    public GameObject hitboxCuboPrefab;
    public float fuerzaDisparo = 500f;
    public Transform Origen;

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
    public Feedbacks Feedbacks;
    public Color Color_RecibeDano;
    public Color Color_ObtieneMonedas;
    public Color Color_FueAvistado;
    public Color Color_Muere;
    public Color Color_SeCura;

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
    public Image barraCoolDown; // Opcional: si ya no necesitás la barra horizontal, dejala en null

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
    }

    void Update()
    {
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
                // Cambiar a modo melee
                modoMelee = true;
                _TimerManager.enModoMagico = false;
                IndicadoresMelee.SetActive(true);
                Debug.Log("Modo cambiado a MELEE");
                anim.SetLayerWeight(0, 0f); // capa rango
                anim.SetLayerWeight(1, 1f); // capa melee
            }
            else
            {
                // Cambiar a modo rango
                modoMelee = false;
                _TimerManager.enModoMagico = true;
                IndicadoresMelee.SetActive(false);
                Debug.Log("Modo cambiado a RANGO");
                anim.SetLayerWeight(0, 1f); // capa rango
                anim.SetLayerWeight(1, 0f); // capa melee
            }
        }


        // 5) Rotar flecha hacia cursor si existe
        RotarFlechaHaciaElCursor();
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
    /// Actualiza la barra horizontal de cooldown (opcional).
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

    /// <summary>
    /// Actualiza el fillAmount de cada ícono según los valores de TimerManager.
    /// fillAmount = 1 → ícono completamente cubierto (cooldown recién iniciado).
    /// fillAmount = 0 → ícono completamente libre (cooldown terminado).
    /// </summary>

    public override void Atacar(Vector3 Destino, string Nombre)
    {
        if (estaMuerto) return;
        if (_TimerManager == null) return;

        // 1) Chequear cooldown “general” (índice 6)
        if (_TimerManager.IsTimerCharging(6)) return;
        _TimerManager.SetTimerToMax(6);

        GameObject ProyectilUsado = null;

        // --- HECHIZOS / ATAQUES ---
        if (Nombre == "BolaDeFuego")
        {
            if (!modoMelee)
            {
                if (_TimerManager.IsTimerCharging(0)) return;
                anim.SetTrigger("magic1");
                ProyectilUsado = BolaDeFuego;
                _TimerManager.SetTimerToMax(0);
            }
            else
            {
                if (_TimerManager.IsTimerCharging(3)) return;
                anim.SetTrigger("melee1");
                _TimerManager.SetTimerToMax(3);
            }
        }
        else if (Nombre == "BolaDeHielo")
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
                if (_TimerManager.IsTimerCharging(4)) return;
                anim.SetTrigger("melee2");
                _TimerManager.SetTimerToMax(4);
            }
            anim.SetFloat("velocidad", 0f);
            agent.isStopped = true;
        }
        else if (Nombre == "Rayo")
        {
            if (!modoMelee)
            {
                if (_TimerManager.IsTimerCharging(2)) return;
                anim.SetTrigger("magic3");
                ProyectilUsado = Rayo;
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

        if (ataque.TryGetComponent<Proyectil>(out var proyectil))
        {
            proyectil.Creador = gameObject;
            proyectil.AutoDestruir = true;
        }

        if (ataque.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(direccion * fuerzaDisparo);
        }

        // 3) Registrar cooldown interno basado en animación
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
    public void GenerarHitboxPie() => GenerarHitbox(puntoGolpePatada, 10);

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

    public override void IrAlDestino(Vector3 destino)
    {
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
            Debug.Log("Falta particulas de caminar");
        }

        // Reproducir sonido si no está sonando
        if (!S_Caminar.isPlaying)
        {
            //S_Caminar.clip = AudioManager.ObtenerAudioPorNombre("Correr_en_pasto");
            S_Caminar.loop = true;
            S_Caminar.Play();
        }

    }

    public override void Morir()
    {
        if (estaMuerto) return;
        Feedbacks.FeedbackRadialVisual(Color_Muere, 4);
        estaMuerto = true;
        anim.SetTrigger("life");
    }

    public override void OnCollision(Collision collider)
    {
        // Implementar si hace falta
    }

    public AudioSource RecibirDanoAudio;
    public override void RecibirDanio(int cantidad)
    {
        Vida -= cantidad;
        Debug.Log($"{gameObject.name} recibió {cantidad} de daño. Vida restante: {Vida}", gameObject);
        Feedbacks.FeedbackRadialVisual(Color_RecibeDano, 1);

        if (Vida <= 0)
        {
            Morir();
            Invoke(nameof(CargaEscenaDerrota), 3f);
        }
        RecibirDanoAudio.Play();
    }

    void CargaEscenaDerrota()
    {
        SceneManager.LoadScene("Derrota");
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
}
