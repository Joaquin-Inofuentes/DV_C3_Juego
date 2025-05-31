using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AccionesJugador : A1_Entidad
{


    public GameObject BolaDeFuego;
    public GameObject BolaDeHielo;
    public GameObject Rayo;
    public GameObject ataqueRapido;
    public GameObject Flechazo;
    public GameObject hitboxCuboPrefab;
    public float fuerzaDisparo = 500f;
    public Transform Origen;
    private bool estaMuerto = false;
    public bool modoMelee = false;
    public Transform puntoGolpePatada;
    public Transform puntoGolpeEspada;
    // Agrega este campo y propiedad en tu clase AccionesJugador
    public float maxCoolDown = 0f;
    private bool hitboxGenerada = false;
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
    public float _coolDown = 0f;

    // Asume que tienes una referencia al RawImage de la barra de cooldown
    public UnityEngine.UI.RawImage barraCoolDown;

    // Llama a este método en Update y cuando cambie el CoolDown
    private void ActualizarBarraCoolDown()
    {
        if (barraCoolDown == null || maxCoolDown == 0f) return;
        float porcentaje = 1f - Mathf.Clamp01(_coolDown / maxCoolDown);
        var rt = barraCoolDown.rectTransform;
        float anchoBase = barraCoolDown.texture != null ? barraCoolDown.texture.width : rt.rect.width;
        // Reemplaza la línea de cálculo de anchoBase y el ajuste de la barra por lo siguiente:
        float anchoMaximo = 200f;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, anchoMaximo * porcentaje);
    }

    // En tu método Update, agrega:
    void CargarBarraDeCoolDown()
    {
        if (CoolDown > 0)
            CoolDown -= Time.deltaTime;
        if (CoolDown < 0)
            CoolDown = 0;

        float velocidadActual = agent.velocity.magnitude;
        anim.SetFloat("velocidad", velocidadActual);
        if (Vector3.Distance(gameObject.transform.position, Destino) < 1)
        {
            Detenerse();
        }

        ActualizarBarraCoolDown();
    }

    public override void Atacar(Vector3 Destino, string Nombre)
    {
        if (estaMuerto) return;
        // Joaco_ Indica q animacion se esta ejecutando
        //if (CoolDown != 0) return;
        if (_TimerManager.IsTimerCharging(6)) return; // Significa q el general se esta recargando
        _TimerManager.SetTimerToMax(6);
        //Debug.Log(1);
        GameObject ProyectilUsado = null;

        if (Nombre == "BolaDeFuego")
        {
            //Debug.Log(2);
            if (!modoMelee)
            {
                //Debug.Log(3);
                if (_TimerManager.IsTimerCharging(0)) return; // Significa q se esta recargando
                anim.SetTrigger("magic1");
                _TimerManager.SetTimerToMax(0);
                ProyectilUsado = BolaDeFuego;
            }
            else
            {
                //Debug.Log(4);
                if (_TimerManager.IsTimerCharging(3)) return; // Significa q se esta recargando
                Debug.Log("Ver algo aca");
                anim.SetTrigger("melee1");
                _TimerManager.SetTimerToMax(3);
                Debug.Log("Algo anad abie");
            }

        }
        if (Nombre == "BolaDeHielo")
        {

            if (!modoMelee)
            {
                if (_TimerManager.IsTimerCharging(1)) return; // Significa q se esta recargando
                anim.SetTrigger("magic2");
                ProyectilUsado = BolaDeHielo;
                _TimerManager.SetTimerToMax(1);
            }
            else
            {
                if (_TimerManager.IsTimerCharging(4)) return; // Significa q se esta recargando
                anim.SetTrigger("melee2");
                _TimerManager.SetTimerToMax(4);
            }
            anim.SetFloat("velocidad", 0);
            agent.isStopped = true;
        }
        if (Nombre == "Rayo")
        {
            if (!modoMelee)
            {
                if (_TimerManager.IsTimerCharging(2)) return; // Significa q se esta recargando
                anim.SetTrigger("magic3"); //nuevo
                ProyectilUsado = Rayo;
                _TimerManager.SetTimerToMax(2);
            }
            else
            {
                if (_TimerManager.IsTimerCharging(5)) return; // Significa q se esta recargando
                anim.SetTrigger("melee3"); //nuevo
                _TimerManager.SetTimerToMax(5);
            }
            anim.SetFloat("velocidad", 0);
            agent.isStopped = true;
        }

        transform.LookAt(Destino);
        if(ProyectilUsado == null) return;
        Vector3 direccion = (Destino - Origen.transform.position).normalized;
        //Debug.Log(Nombre + " " + Destino, gameObject);
        GameObject Ataque = Instantiate(
            ProyectilUsado,
            Origen.transform.position,
            Quaternion.LookRotation(direccion));

        if (Ataque.GetComponent<Proyectil>() != null)
        {
            Ataque.GetComponent<Proyectil>().Creador = gameObject;
            Ataque.GetComponent<Proyectil>().AutoDestruir = true;
        }

        Rigidbody rb = Ataque.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direccion * fuerzaDisparo);
        }

        /*   else
           {
               Vector3 posicionHitbox = Origen.position + transform.forward * 1.5f; // 🔴 NUEVO
               Quaternion rotacionHitbox = transform.rotation; // 🔴 NUEVO

               GameObject hitbox = Instantiate(hitboxCuboPrefab, posicionHitbox, rotacionHitbox); // 🔴 NUEVO
               Destroy(hitbox, 0.5f); // 🔴 NUEVO
           }
           */
        Invoke("RegistrarCoolDown", 0.1f);
    }

    // Joaco_CoolDownTiempos
    public void RegistrarCoolDown()
    {
        float speed = 1f;
        string nombreAnimacion = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        //Debug.Log(nombreAnimacion);
        switch (nombreAnimacion)
        {
            case string n when n.Contains("01"):
                speed = 8f;
                break;
            case string n when n.Contains("02"):
                speed = 4f;
                break;
            case string n when n.Contains("03"):
                speed = 4f;
                break;
            case string n when n.Contains("ataque_pesadoPersonaje"):
                speed = 2f;
                break;
            case string n when n.Contains("ataque_rapidoPersonaje"):
                speed = 3f;
                break;
            case string n when n.Contains("ataque_fuertePersonaje"):
                speed = 0.9f;
                break;
            default:
                speed = 1.0f;
                break;
        }
        //Debug.Log(animacion.GetCurrentAnimatorClipInfo(0)[0].clip.length + " | " + speed);
        float Tiempo = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length / speed;
        CoolDown = Tiempo > 2 ? 2 : Tiempo;
    }

    public void GenerarHitboxAtaqueRapido() => GenerarHitbox(puntoGolpeEspada, 15);
    public void GenerarHitboxAtaquePesado() => GenerarHitbox(puntoGolpeEspada, 35);
    public void GenerarHitboxPie() => GenerarHitbox(puntoGolpePatada, 10);
    public void GenerarHitbox(Transform puntoDeGolpe, int danio)
    {
        if (hitboxGenerada) return;

        if (hitboxCuboPrefab != null && puntoDeGolpe != null)
        {
            GameObject hitbox = Instantiate(hitboxCuboPrefab, puntoDeGolpe.position, puntoDeGolpe.rotation);
            Hitbox componenteHitbox = hitbox.GetComponent<Hitbox>();
            if (componenteHitbox != null)
                componenteHitbox.ConfigurarDanio(danio);
            hitboxGenerada = true;
            Destroy(hitbox, 0.5f);
            Invoke(nameof(ResetHitboxFlag), 0.1f); // Se puede ajustar el tiempo
        }

    }
    private void ResetHitboxFlag()
    {
        hitboxGenerada = false;
    }


    public override void Detenerse()
    {
        agent.isStopped = true;
    }
    public ParticleSystem Particulas;
    public override void IrAlDestino(Vector3 destino)
    {
        if (estaMuerto) return;
        agent.isStopped = false;
        //Debug.Log(1);
        transform.LookAt(destino);
        agent.SetDestination(destino);
        Destino = destino;
        Particulas.gameObject.transform.position = destino;
        Particulas.Play();
        //Debug.Log(2);

    }


    public override void Morir()
    {
        if (estaMuerto) return;
        Feedbacks.FeedbackRadialVisual(Color_Muere, 4);
        estaMuerto = true;
        anim.SetTrigger("life"); //nuevo
    }

    public override void OnCollision(Collision collider)
    {
        throw new System.NotImplementedException();
    }
    public Feedbacks Feedbacks;
    public Color Color_RecibeDano;
    public Color Color_ObtieneMonedas;
    public Color Color_FueAvistado;
    public Color Color_Muere;
    public Color Color_SeCura;
    public TimerManager _TimerManager;

    public override void RecibirDanio(int cantidad)
    {
        Vida -= cantidad;
        Debug.Log(gameObject.name + " Recibio daño de " + cantidad + " le queda " + Vida, gameObject);
        Feedbacks.FeedbackRadialVisual(Color_RecibeDano, 1);
        if (Vida <= 0)
        {
            Morir();
            Invoke("CargaEscenaDerrota", 3f);
        }
    }
    void CargaEscenaDerrota()
    {
        SceneManager.LoadScene("Derrota");
    }

    // + Agente: Navmeshagent
    // AtaqueElegido(string): void
    // GameManager
    // Inputs

    // Start is called before the first frame update
    void Start()
    {

    }
    public Vector3 Destino;
    // Update is called once per frame
    public GameObject IndicadoresMelee;
    void Update()
    {

        CargarBarraDeCoolDown();

        //  if (CoolDown > 0)
        //   CoolDown -= Time.deltaTime;
        // if (CoolDown < 0)
        // CoolDown = 0;

        float velocidadActual = agent.velocity.magnitude;
        anim.SetFloat("velocidad", velocidadActual);
        if (Vector3.Distance(gameObject.transform.position, Destino) < 1)
        {
            Detenerse();

        }
        //if nuevo
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            modoMelee = !modoMelee; // alterna entre melee y rango
            if (modoMelee)
            {
                IndicadoresMelee.SetActive(true);
                Debug.Log("Modo cambiado a MELEE");
                anim.SetLayerWeight(0, 0f); // capa 0 = Rango
                anim.SetLayerWeight(1, 1f); // capa 1 = Melee

            }
            else
            {
                IndicadoresMelee.SetActive(false);
                Debug.Log("Modo cambiado a rango");
                anim.SetLayerWeight(0, 1f); // capa 0 = Rango
                anim.SetLayerWeight(1, 0f); // capa 1 = Melee

            }
            //fin el if nuevo
        }
        RotarFlechaHaciaElCursor();
    }


    public GameObject Flecha;
    public void RotarFlechaHaciaElCursor()
    {
        if (Flecha == null) return;

        // 1. Obtener la posición del mouse en el mundo
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plano = new Plane(Vector3.up, Flecha.transform.position);
        float distancia;
        Vector3 puntoMundo = Flecha.transform.position;
        if (plano.Raycast(ray, out distancia))
        {
            puntoMundo = ray.GetPoint(distancia);
        }

        // 2. Calcular la dirección desde la flecha al punto del mouse (solo en X y Z)
        Vector3 direccion = puntoMundo - Flecha.transform.position;
        direccion.y = 0; // Solo rotar en el eje Y

        if (direccion.sqrMagnitude > 0.001f)
        {
            // 3. Calcular la rotación solo en el eje Y
            Quaternion rotacion = Quaternion.LookRotation(direccion, Vector3.up);
            // 4. Ajustar la rotación en X a 90 grados y sumar 90 en Z
            Vector3 euler = rotacion.eulerAngles;
            euler.x = 90;
            euler.z -= 90;
            Flecha.transform.rotation = Quaternion.Euler(euler);
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
