using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics.Internal;
using UnityEditor;
using UnityEngine;

public class A1_A1_H3_Duende : A1_A1_Enemigo
{
    [Header("Configuración")]
    public List<Transform> puntosSpawn;
    public GameObject proyectilPrefab;
    public Transform objetivo;

    float tiempoSiguienteCambio;
    float intervaloActual;





    public GameObject BolaDeAtaque;
    public GameObject AtaqueActual;

    public GameObject PadreDebarraDevida;
    public GameObject BarraDeVida;

    private float anchoOriginal;
    private bool estaMuerto = false;

    void ActualizarBarraDevida()
    {
        // 1. Rotar solo en eje Y hacia la cámara
        Vector3 camPos = Camera.main.transform.position;
        Vector3 dir = camPos - PadreDebarraDevida.transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            PadreDebarraDevida.transform.rotation = Quaternion.LookRotation(dir);

        // 2. Calcular porcentaje real
        float porcentajeSinClamp = Vida / (float)VidaMax;
        float porcentaje = Mathf.Clamp01(porcentajeSinClamp);
        //Debug.Log($"[DEBUG] Vida: {Vida}, VidaMax: {VidaMax}, SinClamp: {porcentajeSinClamp}, Clamp01: {porcentaje}");

        // 3. Escalar ancho de la barra
        Vector3 escala = BarraDeVida.transform.localScale;
        BarraDeVida.transform.localScale = new Vector3(anchoOriginal * porcentaje, escala.y, escala.z);

        // 4. Mover localmente a la izquierda
        float offset = (anchoOriginal - (anchoOriginal * porcentaje)) / 2f;
        BarraDeVida.transform.localPosition = new Vector3(-offset, BarraDeVida.transform.localPosition.y, BarraDeVida.transform.localPosition.z);
    }

    public override void Atacar(Vector3 Destino, string Nombre = "")
    {
        Debug.Log("iniciando ataque", gameObject);
        if (estaMuerto) return;
        //ModoAtaqueMelee = false;
        if (AtaqueActual == null)
        {

            Debug.Log("Atacando",gameObject);
            // Crea un efecto de danio
            // Debug.Log("Atacando");
            // Crea un efecto de daño
            GameObject Ataque = Instantiate(BolaDeAtaque, Destino, Quaternion.identity);
            if (Ataque.GetComponent<Proyectil>() != null)
            {
                Ataque.GetComponent<Proyectil>().Creador = gameObject;
            }
            AtaqueActual = Ataque;
            Ataque.transform.localScale = new Vector3(50, 50, 50);
            // Destruye ese efecto
            Destroy(Ataque, 1f);
            if (ModoAtaqueMelee == true)
            {
                anim.SetTrigger("boss_ataque1");
            }
        }
        if (AtaqueActual != null)
        {
            //Debug.Log("Esta atacando " + gameObject, gameObject);
        }

        //Debug.Log(Nombre, gameObject);
    }

    public override void Colisiono(GameObject Colision, string TipoDeColision)
    {
        Debug.Log(Colision + " | " + TipoDeColision, gameObject);
    }

    public override void Detenerse()
    {
        agent.isStopped = true;

    }

    public override void IrAlDestino(Vector3 destino)
    {
        Debug.DrawLine(destino, transform.position);
        if (estaMuerto) return;
        agent.isStopped = false;
        agent.SetDestination(destino);

        // Rotar el GameObject para que mire hacia el destino (solo en el eje Y)
        Vector3 direccion = destino - transform.position;
        direccion.y = 0;
        if (direccion != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direccion);
    }

    public override void Morir()
    {
        PadreDebarraDevida.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        agent.enabled = false;
        // -0.591
        //transform.Translate(0, -0.591f, 0);
        anim.SetBool("life", false);
        Debug.Log("Falta animacion de morir");
        Destroy(gameObject, 3f); // Joaco_Lo agregue para q desaparezca el duende
        //StartCoroutine(DesaparecerDespuesDeSegundos(10f)); // espera 3 segundos
        if (estaMuerto) return;
        estaMuerto = true;
    }

    private IEnumerator DesaparecerDespuesDeSegundos(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        Destroy(gameObject);
    }

    public override void OnCollision(Collision collider)
    {
        throw new System.NotImplementedException();
    }

    public override void OnDisabled()
    {
        throw new System.NotImplementedException();
    }

    public override void OnEnabled()
    {
        throw new System.NotImplementedException();
    }

    public override void RecibirDanio(int cantidad)
    {
        anim.SetTrigger("danio");
        Vida -= cantidad;
        if (Vida <= 0)
        {
            Morir();
            anim.SetBool("life", false);
        }
    }

    protected override void Start()
    {
        base.Start(); // Llama al Start del padre
                      // Cï¿½digo propio de ArquerasElfas
        anchoOriginal = BarraDeVida.transform.localScale.x;
        Iniciar();
    }

    protected override void Update()
    {
        base.Update(); // Llama al Update del padre
        Reapariciones(); // Llama al metodo de reapariciones
        float velocidad = agent.velocity.magnitude;
        //Debug.Log("Velocidad agente: " + velocidad);
        anim.SetFloat("velocidad", velocidad);
        ActualizarBarraDevida();
        if (Objetivo)
        {
            if (agent.velocity.magnitude >= 0 &&
                Vector3.Distance(Objetivo.transform.position, transform.position)
                < DistanciaParaAtaqueLargo)
            {
                agent.isStopped = true; // Detiene el agente
            }
        }
        if (Vida <= 0)
        {
            Morir();
            anim.SetBool("life", false);
        }
    }

    public void OnDrawGizmos()
    {

        UtilidadesDeGizmos.DibujarCirculoPlano(transform.position, DistanciaParaAtaqueMelee, 10, Color.yellow);

        UtilidadesDeGizmos.DibujarCirculoPlano(transform.position, DistanciaParaAtaqueLargo, 20, Color.red);

        UtilidadesDeGizmos.DibujarCirculoPlano(transform.position, DistanciaParaPerseguir, 32, Color.blue);
    }














    void Iniciar()
    {
        anim = GetComponent<Animator>();
        GenerarNuevoIntervalo();
        tiempoSiguienteCambio = Time.time + intervaloActual;
    }

    void Reapariciones()
    {
        string nombreAnim = anim.GetCurrentAnimatorStateInfo(0).IsName("") ? "" : anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        // Si está en animación que contiene "Ataque", no mover
        if (nombreAnim.Contains("Ataque"))
            return;

        if (Time.time >= tiempoSiguienteCambio)
        {
            CambiarDePosicion();
            GenerarNuevoIntervalo();
            tiempoSiguienteCambio = Time.time + intervaloActual;
        }
    }

    void CambiarDePosicion()
    {
        int index = Random.Range(0, puntosSpawn.Count);
        transform.position = puntosSpawn[index].position;
    }

    void GenerarNuevoIntervalo()
    {
        intervaloActual = Random.Range(2f, 6f);
    }

    /// <summary>
    /// Llamado externamente para activar ataque.
    /// </summary>
    public void ActivarYAtacar()
    {
        anim.SetTrigger("Atacar");
    }

    /// <summary>
    /// Evento al final de la animación de ataque.
    /// </summary>
    public void EndAnimationAtack()
    {
        Vector3 dir = objetivo.position - transform.position;
        transform.right = dir;

        GameObject nuevo = Instantiate(proyectilPrefab, transform.position, Quaternion.identity);
        nuevo.GetComponent<Proyectil>().Inicializar(dir.normalized);
    }














    /*
        Emptys donde aparecera y desaparecera cada 2 a 6 segundos de manera aleatorio pero en ese intervalo

        Al tocar X elemento se activara y empezara a atacar

        Metodos

        - Orquestador
        Tener 2 timers aleatorios


        - CambiarDePosicion
        Selecciona 1 empty aleatorio de una lista
        Se asigna su transform position

        - Atacar 
        Solo iniciar animacion de ataque

        - EndAnimationAtack
        _ al finalizar animacion de ataque
        Mirar
        Crear proyectil
	        Hacia la pos del player
     */
}
