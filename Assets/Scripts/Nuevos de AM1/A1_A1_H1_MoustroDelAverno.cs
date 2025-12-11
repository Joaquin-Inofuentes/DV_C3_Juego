using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class A1_A1_H1_MoustroDelAverno : A1_A1_Enemigo, IDaniable
{

    [Header("Vida")]
    [SerializeField] private int _vidaMaxima = 100;
    [SerializeField] private int _vidaActual = 100;

    // Estas son las propiedades de la interfaz IDaniable
    public int vidaMaxima
    {
        get => _vidaMaxima;
        set => _vidaMaxima = value;
    }

    public int vidaActual
    {
        get => _vidaActual;
        set => _vidaActual = value;
    }

    // --- VARIABLES ELIMINADAS ---
    // public GameObject PadreDebarraDevida; --> AHORA ESTÁ EN 'GestorBarraDeVida'
    // public GameObject BarraDeVida; --> AHORA ESTÁ EN 'GestorBarraDeVida'
    // private float anchoOriginal; --> AHORA ESTÁ EN 'GestorBarraDeVida'

    public GameObject BolaDeAtaque;
    public GameObject AtaqueActual;

    public bool estaMuerto = false;
    public bool Congelado;
    public bool PrimerAtaqueAAnular;
    public ATK_Congelar EfectoDeCongelado;
    public float offsetCongelamiento = 0f;

    public string ultimoProyectilRecibido = "";
    public GeneradorEnemigos generador;
    public bool ProximoAtaqueExplosionElectrica = false;

    // --- MÉTODO ELIMINADO ---
    // void ActualizarBarraDevida() { ... } --> AHORA SE LLAMA 'ActualizarVisualmenteLaBarraDeVida' EN 'GestorBarraDeVida'

    public string AnimacionActual = "";
    public Vector3 DestinoDelAtaque = Vector3.zero;

    public override void Atacar(Vector3 Destino, string Nombre = "")
    {
        if (estaMuerto) return;
        DestinoDelAtaque = Destino;
        transform.LookAt(Destino);
        AnimatorClipInfo[] clips = anim.GetCurrentAnimatorClipInfo(0);
        if (clips.Length > 0) { AnimacionActual = clips[0].clip.name; }

        if (AnimacionActual.Contains("idle"))
        {
            int ataqueIndex = Random.Range(0, 2);
            if (ataqueIndex == 0) { anim.SetTrigger("ataque1"); }
            else { anim.SetTrigger("ataque2"); }
        }
        else
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            if (state.normalizedTime >= 0.95f) { anim.SetTrigger("ataque1"); }
        }
    }

    public void CreaEfectoDeAtaque()
    {
        // (Tu código permanece igual)
        if (AtaqueActual == null)
        {
            GameObject Ataque = Instantiate(BolaDeAtaque, DestinoDelAtaque, Quaternion.identity);
            if (Ataque.GetComponent<Proyectil>() != null)
            {
                Ataque.GetComponent<Proyectil>().Creador = gameObject;
                Ataque.GetComponent<Proyectil>().AutoDestruir = false;
            }
            AtaqueActual = Ataque;
            Ataque.transform.localScale = new Vector3(25, 25, 25);
            Destroy(Ataque, 2f);
        }
    }

    public override void Colisiono(GameObject Colision, string TipoDeColision) { Debug.Log(Colision + " | " + TipoDeColision, gameObject); }

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
        // (Tu código permanece igual)
        if (gameObject.name.Contains("ArqueraDuende")) return;
        if (Vector3.Distance(destino, DestinoAsignado) < 3) return;
        if (Congelado || estaMuerto) return;
        agent.isStopped = false;
        agent.SetDestination(destino);
        DestinoAsignado = destino;
        if (!S_Caminar.isPlaying)
        {
            S_Caminar.loop = false;
            S_Caminar.Play();
        }
    }

    public AudioClip SonidoAlMorir;
    public virtual void Morir() // Este método es requerido por la interfaz IDaniable
    {
        if (estaMuerto) return;
        try
        {
            if(SonidoAlMorir) AudioManager.CrearEfectoSonoro(transform.position, SonidoAlMorir);
            agent.enabled = false;
            // PadreDebarraDevida.SetActive(false); // OPCIONAL: Ya no es necesario, el gestor se destruirá solo.
            anim.SetBool("life", false);
            StartCoroutine(DesaparecerDespuesDeSegundos(10f));
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        finally { estaMuerto = true; }
        if (generador != null) { generador.EliminarEnemigo(gameObject); }
    }

    public IEnumerator DesaparecerDespuesDeSegundos(float segundos) { yield return new WaitForSeconds(segundos); Destroy(gameObject); }

    public override void OnCollision(Collision collider) { throw new System.NotImplementedException(); }
    public override void OnDisabled() { throw new System.NotImplementedException(); }
    public override void OnEnabled() { throw new System.NotImplementedException(); }

    public bool RecibiraDobleDanoLaProximaVez = false;
    public AudioSource S_RecibirDano;
    public bool PendienteDeCargaElectrica = false;
    public ReportadorDeLasColisiones CargaElectrica;
    public AudioClip AudioAlRecibirDanio;
    public GameObject MensajeDeBugActivado;

    public void RecibirDanio(int cantidad) // Este método es requerido por la interfaz IDaniable
    {
        // (Tu código de recibir daño permanece igual)
        Debug.Log(gameObject.name + " ha recibido " + cantidad + " de daño", gameObject);
        if (ultimoProyectilRecibido.Contains("hitboxCubo") && PendienteDeCargaElectrica == true)
        {
            if (PrimerAtaqueAAnular) { PrimerAtaqueAAnular = false; return; }
            ultimoProyectilRecibido = "";
            vidaActual -= cantidad;
            CargaElectrica.ElectrocutarCercanos();
            Feedbacks.Componente.UIFadeComboScript.MostrarTexto("¡DESCARGA ENCADENADA!", new Color(1f, 0.858f, 0.039f));
            PendienteDeCargaElectrica = false;
        }
        if (AudioAlRecibirDanio) { AudioManager.CrearEfectoSonoro(transform.position, AudioAlRecibirDanio); }

        vidaActual -= cantidad;
        S_RecibirDano.Play();
        if (RecibiraDobleDanoLaProximaVez)
        {
            vidaActual -= cantidad * 2;
            RecibiraDobleDanoLaProximaVez = false;
        }
        if (vidaActual <= 0)
        {
            Morir();
            anim.SetBool("life", false);
        }
        if (Congelado)
        {
            if (PrimerAtaqueAAnular) { PrimerAtaqueAAnular = false; return; }
            if (ultimoProyectilRecibido.Contains("hitboxCubo"))
            {
                vidaActual -= cantidad * 2;
                Congelado = false;
                EfectoDeRopturaDeCongelamiento();
                if (gameObject.name == "ArqueraDuende")
                {
                    vidaActual = 0;
                    Morir();
                    MensajeDeBugActivado.SetActive(true);
                    Destroy(MensajeDeBugActivado, 5f);
                }
            }
        }
    }

    public GameObject VFXDeRopturaDeHielo;
    public AudioSource S_RupturaDeHielo;
    public void EfectoDeRopturaDeCongelamiento()
    {
        EfectoDeCongelado.ReanudarAgente();

        var rend = GetComponentInChildren<Renderer>();
        float altura = rend.bounds.size.y * 0.5f;

        Vector3 pos = transform.position + Vector3.up * altura;

        GameObject efecto = Instantiate(VFXDeRopturaDeHielo, pos, Quaternion.identity);
        Destroy(efecto, 2f);

        S_RupturaDeHielo.Play();
        S_RupturaDeHielo.loop = false;
        IrAlDestino(DestinoAsignado);
        agent.isStopped = false;
        RecibiraDobleDanoLaProximaVez = false;
        RalentizarTiempo();

        if (EfectoDeCongelado) Destroy(EfectoDeCongelado.gameObject);
    }


    // ... (El resto de tus métodos como RalentizarTiempo, RestaurarTiempo, etc. permanecen exactamente iguales)
    void RalentizarTiempo()
    {
        Time.timeScale = 0.3f;
        Feedbacks.Componente.UIFadeComboScript.MostrarTexto("¡FRACTURA CONGELADA!", new Color(0.5f, 1f, 1f));
        if (Feedbacks.Componente.S_MomentoEpico != null)
            Feedbacks.Componente.S_MomentoEpico.Play();
        Invoke(nameof(RestaurarTiempo), 0.3f);
    }
    void RestaurarTiempo() { Time.timeScale = 1f; }


    protected override void Start()
    {
        base.Start();
        // --- LÍNEA ELIMINADA ---
        // anchoOriginal = BarraDeVida.transform.localScale.x;

        if (gameObject.name.Contains("ArqueraDuende"))
        {
            Iniciar();
        }
    }

    protected override void Update()
    {
        base.Update();
        float velocidad = agent.velocity.magnitude;
        if (anim) { anim.SetFloat("velocidad", velocidad); }
        else { Debug.Log("Falta el anim de " + gameObject.name, gameObject); }

        // --- LÍNEA ELIMINADA ---
        // ActualizarBarraDevida(); // Ya no es responsabilidad de esta clase.

        if (Vector3.Distance(transform.position, DestinoAsignado) < 2f) { Detenerse(); }
        if (vidaActual < 10) { Morir(); }
        if (EfectoDeCongelado == null && Congelado) { Congelado = false; RecibiraDobleDanoLaProximaVez = false; }
        if (gameObject.name.Contains("ArqueraDuende"))
        {
            if (objetivoActual != null) { Reaparaciones(); transform.LookAt(objetivoActual.transform.position); }
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
            EnemigoCreado.GetComponent<IDaniable>().vidaMaxima = 50; // Asignar vida máxima al nuevo aliado
            EnemigoCreado.GetComponent<IDaniable>().vidaActual = 50; // Asignar vida al nuevo aliado
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
