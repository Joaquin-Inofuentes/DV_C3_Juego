using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class A1_A1_H1_MoustroDelAverno : A1_A1_Enemigo
{
    public GameObject BolaDeAtaque;
    public GameObject AtaqueActual;

    public GameObject PadreDebarraDevida;
    public GameObject BarraDeVida;

    private float anchoOriginal;
    public bool estaMuerto = false;
    public bool Congelado;
    public bool PrimerAtaqueAAnular;
    public ATK_Congelar EfectoDeCongelado; // Efecto visual de congelación
    public string ultimoProyectilRecibido = "";

    public bool ProximoAtaqueExplosionElectrica = false;

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

    public string AnimacionActual = "";
    public Vector3 DestinoDelAtaque = Vector3.zero;
    public override void Atacar(Vector3 Destino, string Nombre = "")
    {
        return;
        //Debug.Log(0);
        if (estaMuerto) return;
        //Debug.Log(1);
        DestinoDelAtaque = Destino; // Guardar el destino del ataque
        transform.LookAt(Destino); // Mirar hacia el destino
        AnimatorClipInfo[] clips = anim.GetCurrentAnimatorClipInfo(0);
        if (clips.Length > 0)
        {
            AnimacionActual = clips[0].clip.name;
        }
        //Debug.Log(AnimacionActual);
        if (AnimacionActual.Contains("idle"))
        {
            int ataqueIndex = Random.Range(0, 2); // 0 o 1
            Debug.Log($"[DEBUG] Ataque seleccionado: {ataqueIndex} para {gameObject.name}");
            if (ataqueIndex == 0)
            {
                anim.SetTrigger("ataque1");
            }
            else
            {
                anim.SetTrigger("ataque2");
            }
        }
        else
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            float t = state.normalizedTime;

            if (t >= 0.95f)
            {
                anim.SetTrigger("ataque1");
            }
        }

        /* Joaco_Lo cambie. En el "GEO_Moustro" Ahora llamara al AlFinalizarLlamarA al finalizar la animacion de ataque
        else if (AnimacionActual.Contains("ataque"))
        {
            //Debug.Log(1);
            if (VerificaSiPuedeAtacar())
            {
                //Debug.Log(2);
                CreaEfectoDeAtaque(Destino);
            }
        }
        */
    }

    bool avisoHecho = false;

    bool VerificaSiPuedeAtacar()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        float t = state.normalizedTime;
        Debug.Log(t + " | " + avisoHecho, gameObject);
        //Debug.Log(t  + " | "+ avisoHecho);
        if (!avisoHecho && t >= 0.90f)
        {
            //Debug.Log("Terminó una vuelta de la animación de ataque");
            avisoHecho = true;
            return true;
        }

        // Resetear aviso cuando la animación vuelve a empezar
        if (t < 1f)
        {
            avisoHecho = false;
            return false;
        }
        else
        {
            avisoHecho = false;
            return false;
        }

    }

    // Se lo llama desde el AlFinalizarLlamarA de finalización de la animación de ataque
    // Del gameobjeto "GEO_Moustro"
    public void CreaEfectoDeAtaque()
    {

        //ModoAtaqueMelee = false;
        if (AtaqueActual == null)
        {

            //Debug.Log(1);
            //Debug.Log("Atacando");
            // Crea un efecto de danio
            //Debug.Log("Atacando");
            // Crea un efecto de daño
            GameObject Ataque = Instantiate(BolaDeAtaque, DestinoDelAtaque, Quaternion.identity);
            if (Ataque.GetComponent<Proyectil>() != null)
            {
                Ataque.GetComponent<Proyectil>().Creador = gameObject;
                Ataque.GetComponent<Proyectil>().AutoDestruir = false;
            }
            AtaqueActual = Ataque;
            Ataque.transform.localScale = new Vector3(25, 25, 25);
            // Destruye ese efecto

            Destroy(Ataque, 2f);


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
        if (agent == null) return;
        agent.isStopped = true;
        S_Caminar.loop = false;
        S_Caminar.Stop();
    }
    public Vector3 DestinoAsignado = Vector3.zero;
    public override void IrAlDestino(Vector3 destino)
    {

        if (gameObject.name.Contains("ArqueraDuende"))
        {
            return;
        }
        if (Vector3.Distance(destino, DestinoAsignado) < 3) return;

        if (Congelado) return;
        if (estaMuerto) return;
        agent.isStopped = false;
        agent.SetDestination(destino);
        DestinoAsignado = destino;

        // Reproducir sonido si no está sonando
        if (!S_Caminar.isPlaying)
        {
            //S_Caminar.clip = AudioManager.ObtenerAudioPorNombre("Correr_en_pasto");
            S_Caminar.loop = false;
            S_Caminar.Play();
        }
    }

    public AudioClip SonidoAlMorir; // Sonido al morir

    public override void Morir()
    {
        if (estaMuerto) return;
        try
        {
            AudioManager.CrearEfectoSonoro(transform.position, SonidoAlMorir); // Reproducir sonido al morir
            agent.enabled = false;
            PadreDebarraDevida.SetActive(false);
            // transform.Translate(0, -0.7f, 0); Correcion del bug de eliminar al boss
            anim.SetBool("life", false);
            StartCoroutine(DesaparecerDespuesDeSegundos(10f)); // espera 3 segundos
            Debug.Log("Falta animacion de morir");
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        finally
        {
            estaMuerto = true;
        }
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

    public bool RecibiraDobleDanoLaProximaVez = false;
    public AudioSource S_RecibirDano; // Sonido al recibir daño

    public bool PendienteDeCargaElectrica = false;
    public ReportadorDeLasColisiones CargaElectrica;
    public AudioClip AudioAlRecibirDanio; // Sonido al recibir daño
    public GameObject MensajeDeBugActivado; // Mensaje de bug activado
    public override void RecibirDanio(int cantidad)
    {
        Debug.Log(gameObject.name + " ha recibido " + cantidad + " de daño", gameObject);
        if (ultimoProyectilRecibido.Contains("hitboxCubo")
            && PendienteDeCargaElectrica == true)
        {
            if (PrimerAtaqueAAnular)
            {
                PrimerAtaqueAAnular = false;
                return;
            }

            ultimoProyectilRecibido = "";
            Vida -= cantidad;
            CargaElectrica.ElectrocutarCercanos();

            Feedbacks.Componente.UIFadeComboScript.MostrarTexto("¡DESCARGA ENCADENADA!", new Color(1f, 0.858f, 0.039f));


            PendienteDeCargaElectrica = false;
        }
        if (AudioAlRecibirDanio)
        {
            AudioManager.CrearEfectoSonoro(transform.position, AudioAlRecibirDanio); // Reproducir sonido al recibir daño
        }
        //Debug.Log(1);
        Vida -= cantidad;
        S_RecibirDano.Play(); // Reproducir sonido al recibir daño
        if (RecibiraDobleDanoLaProximaVez)
        {
            Vida -= cantidad * 2; // Doble daño
            RecibiraDobleDanoLaProximaVez = false; // Resetea el estado
        }

        if (Vida <= 0)
        {
            Morir();
            anim.SetBool("life", false);
        }
        if (Congelado)
        {
            if (PrimerAtaqueAAnular)
            {
                PrimerAtaqueAAnular = false;
                return;
            }
            if (ultimoProyectilRecibido.Contains("hitboxCubo"))
            {
                Vida -= cantidad * 2;
                Congelado = false;
                EfectoDeRopturaDeCongelamiento();
                if (gameObject.name == "ArqueraDuende")
                {
                    Vida = 0;
                    Morir(); // Matar al enemigo si es ArqueraDuende
                    MensajeDeBugActivado.SetActive(true);
                    Destroy(MensajeDeBugActivado, 5f); // Destruir el mensaje después de 5 segundos
                }
            }
        }
    }

    public GameObject VFXDeRopturaDeHielo; // Efecto visual de congelación
    public AudioSource S_RupturaDeHielo; // Sonido de ruptura de congelamiento
    public void EfectoDeRopturaDeCongelamiento()
    {
        EfectoDeCongelado.ReanudarAgente(); // Reanudar el agente
        // Crear VFX Visual de ruptura de congelamiento
        GameObject efecto = Instantiate(VFXDeRopturaDeHielo, transform.position, Quaternion.identity);
        Destroy(efecto, 2f); // Destruir el efecto después de 2 segundos
        // Reproducir sonido de ruptura de congelamiento
        S_RupturaDeHielo.Play();
        S_RupturaDeHielo.loop = false; // Asegurarse de que no sea loop

        IrAlDestino(DestinoAsignado); // Reanudar movimiento
        agent.isStopped = false;
        RecibiraDobleDanoLaProximaVez = false; // Reanudar el estado del monstruo

        RalentizarTiempo(); // Ralentizar el tiempo al romper el hielo
        if (EfectoDeCongelado) Destroy(EfectoDeCongelado.gameObject); // Destruir el efecto de congelación

    }


    void RalentizarTiempo()
    {
        Time.timeScale = 0.3f;

        Feedbacks.Componente.UIFadeComboScript.MostrarTexto("¡FRACTURA CONGELADA!", new Color(0.5f, 1f, 1f));

        if (Feedbacks.Componente.S_MomentoEpico != null)
            Feedbacks.Componente.S_MomentoEpico.Play();

        Invoke(nameof(RestaurarTiempo), 0.3f);
    }


    void RestaurarTiempo()
    {
        Time.timeScale = 1f;
    }



    protected override void Start()
    {
        base.Start(); // Llama al Start del padre
                      // Cï¿½digo propio de ArquerasElfas
        anchoOriginal = BarraDeVida.transform.localScale.x;


        if (gameObject.name.Contains("ArqueraDuende"))
        {
            Iniciar();
        }

    }



    protected override void Update()
    {
        base.Update(); // Llama al Update del padre
        float velocidad = agent.velocity.magnitude;
        //Debug.Log("Velocidad agente: " + velocidad);
        if (anim)
        {
            anim.SetFloat("velocidad", velocidad);
        }
        else
        {
            Debug.Log("Falta el anim de " + gameObject.name, gameObject);
        }
        ActualizarBarraDevida();
        if (Vector3.Distance(transform.position, DestinoAsignado) < 2f)
        {
            Detenerse();
        }
        if (Vida < 10)
        {
            //Debug.Log("se tiene " + Vida);
            Morir();
        }

        if (EfectoDeCongelado == null && Congelado)
        {
            Congelado = false; // Si el efecto de congelación ya no existe, desactivar el estado de congelación
            RecibiraDobleDanoLaProximaVez = false; // Reanudar el estado del monstruo
        }


        if (gameObject.name.Contains("ArqueraDuende"))
        {
            //Debug.Log(1);

            if (objetivoActual != null)
            {
                Reaparaciones();
            }

            if (objetivoActual != null)
            {
                transform.LookAt(objetivoActual.transform.position);
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 RangoDeAtaque = Vector3.one * DistanciaParaAtaqueMelee * 2; // RangoDeAtaque del cubo de ataque melee
        RangoDeAtaque.y = 0.5f; // Asegúrate de que el cubo tenga un grosor visible
        // Cubo para distancia melee (rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, RangoDeAtaque);

        // Cubo para distancia de persecución (azul)

        Vector3 RangoDePerseguir = Vector3.one * DistanciaParaPerseguir * 2; // RangoDeAtaque del cubo de ataque melee
        RangoDePerseguir.y = 0.5f; // Asegúrate de que el cubo tenga un grosor visible
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, RangoDePerseguir);
    }

    public void CongelarEnemigo()
    {
        Debug.Log("Congelar enemigo", gameObject);
    }

    public void DescongelarEnemigo()
    {
        Debug.Log("Descongelar enemigo", gameObject);
    }

    [Header("Configuración")]
    public List<Transform> puntosSpawn;
    public GameObject proyectilPrefab;

    float tiempoSiguienteCambio;
    float intervaloCambio;

    float tiempoSiguienteAtaque;
    float intervaloAtaque;


    void Iniciar()
    {
        //anim = GetComponent<Animator>();
        GenerarNuevoIntervaloCambio();
        GenerarNuevoIntervaloAtaque();
        tiempoSiguienteCambio = Time.time + intervaloCambio;
        tiempoSiguienteAtaque = Time.time + intervaloAtaque;
    }

    void Reaparaciones()
    {
        string nombreAnim = anim.GetCurrentAnimatorClipInfo(0).Length > 0 ? anim.GetCurrentAnimatorClipInfo(0)[0].clip.name : "";

        if (nombreAnim.Contains("Ataque"))
            return;

        // Cambio de posición aleatorio
        if (Time.time >= tiempoSiguienteCambio)
        {
            CambiarDePosicion();
            GenerarNuevoIntervaloCambio();
            tiempoSiguienteCambio = Time.time + intervaloCambio;
        }

        // Ataque aleatorio
        if (Time.time >= tiempoSiguienteAtaque)
        {
            ActivarYAtacar();
            GenerarNuevoIntervaloAtaque();
            tiempoSiguienteAtaque = Time.time + intervaloAtaque;
        }
    }

    public AudioClip SonidoDeTeletransportacion;
    void CambiarDePosicion()
    {
        if (estaMuerto) return; // No crear aliados si el monstruo está muerto
        int index = Random.Range(0, puntosSpawn.Count);
        CrearAliado(transform.position); // Crear aliado al cambiar de posición
        transform.position = puntosSpawn[index].position;
        AudioManager.CrearEfectoSonoro(transform.position, SonidoDeTeletransportacion);
        //transform.LookAt(objetivo.position); // Mirar hacia el objetivo
    }

    void GenerarNuevoIntervaloCambio()
    {
        intervaloCambio = Random.Range(3.1f, 6f);
    }

    void GenerarNuevoIntervaloAtaque()
    {
        intervaloAtaque = Random.Range(1f, 3f);
    }

    public void ActivarYAtacar()
    {
        anim.SetTrigger("ataque1");
    }

    public void EndAnimationAtack()
    {
        Debug.Log("Se inicio el ataque");
        Atacar(objetivoActual.position);
        Vector3 dir = objetivoActual.position - transform.position;
        transform.right = dir;

        GameObject nuevo = Instantiate(proyectilPrefab, transform.position, Quaternion.identity);
        nuevo.GetComponent<Proyectil>().Inicializar(dir.normalized);
        nuevo.GetComponent<Proyectil>().Creador = gameObject; // Asignar el creador del proyectil
    }

    public void GenerarHitboxAtaqueRapido()
    {
        Debug.Log("Hitbox generado");
        // Tu lógica de daño
    }

    public void AsignarEnemigo(GameObject gameObject)
    {
        Objetivo = gameObject;
        objetivoActual = gameObject.transform;
    }



    public GameObject P_PosionDeVida;
    public void OnDestroy()
    {
        if (gameObject.name == "ArqueraDuende")
        {
            SceneManager.LoadScene("Victoria");
        }
        if (P_PosionDeVida != null)
        {
            float randomX = Random.Range(0f, 1f);
            if (randomX < 0.60f) return; // 20% de probabilidad de crear el efecto de veneno
            GameObject posion = Instantiate(P_PosionDeVida, transform.position, Quaternion.identity);
        }
    }

    public List<GameObject> AliadosCreados = new List<GameObject>(); // Lista de aliados creados
    public GameObject AliadoPrefab; // Prefab del aliado a crear
    public GameObject EfectoDeAparicionDeEnemigo; // Efecto visual al crear el aliado
    public GameObject VFX_ReAparecer; // Efecto visual al ReAparecer
    public void CrearAliado(Vector3 posicion)
    {
        if (estaMuerto) return; // No crear aliados si el monstruo está muerto
        Destroy(Instantiate(VFX_ReAparecer, posicion, Quaternion.identity), 2f); // Destruir el efecto después de 2 segundos
        StartCoroutine(CrearAliadoTrasDelay(posicion));
    }
    int CantidadDeAliados = 5; // Contador de aliados creados
    private IEnumerator CrearAliadoTrasDelay(Vector3 posicion)
    {
        yield return new WaitForSeconds(5f);
        if (AliadoPrefab != null)
        {
            LimpiarAliadosNulosOMissing();
            if (AliadosCreados.Count >= CantidadDeAliados) yield break; // No crear más aliados si ya se alcanzó el límite
            if (estaMuerto) yield break; // No crear aliados si el monstruo está muerto
            GameObject EnemigoCreado = Instantiate(AliadoPrefab, posicion, Quaternion.identity);
            Destroy(Instantiate(EfectoDeAparicionDeEnemigo, posicion, Quaternion.identity), 2f); // Destruir el efecto después de 2 segundos
            EnemigoCreado.GetComponent<A1_A1_Enemigo>().VidaMax = 50; // Asignar vida máxima al nuevo aliado
            EnemigoCreado.GetComponent<A1_A1_Enemigo>().Vida = 50; // Asignar vida al nuevo aliado
            EnemigoCreado.GetComponent<NavMeshAgent>().speed = 7f;
            EnemigoCreado.GetComponent<NavMeshAgent>().acceleration = 7f; // Asignar velocidad al nuevo aliado
            AliadosCreados.Add(EnemigoCreado); // Agregar el nuevo aliado a la lista
        }
    }
    /// <summary>
    /// Elimina de la lista AliadosCreados todos los elementos que sean null o estén missing.
    /// </summary>
    public void LimpiarAliadosNulosOMissing()
    {
        AliadosCreados.RemoveAll(item => item == null);
    }
}
