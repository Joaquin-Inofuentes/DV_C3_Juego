using UnityEngine;

public class A1_A1_H2_ArquerasElfas : A1_A1_Enemigo, IDaniable
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



    public GameObject BolaDeAtaque;
    public GameObject AtaqueActual;

    private bool estaMuerto = false;

    // --- NUEVAS VARIABLES PARA EL TEMPORIZADOR ---
    private float tiempoParaDesaparecer = -1f; // -1 indica que el temporizador no está activo.
    private const float DURACION_DESAPARICION = 10f; // Constante para el tiempo de espera.

    #region Implementacion de Interfaz IDaniable
    // "Traduce" las variables de la clase base a los nombres requeridos por la interfaz.

    #endregion

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        // Si el enemigo no está muerto, ejecuta su lógica normal.
        if (!estaMuerto)
        {
            if (agent != null && anim != null)
            {
                float velocidad = agent.velocity.magnitude;
                anim.SetFloat("velocidad", velocidad);
            }
        }
        // Si el enemigo ESTÁ muerto, maneja el temporizador para desaparecer.
        else
        {
            // Si el temporizador está activo (es mayor que 0)
            if (tiempoParaDesaparecer > 0)
            {
                // Resta el tiempo que pasó desde el último frame.
                tiempoParaDesaparecer -= Time.deltaTime;

                // Si el tiempo se acabó, destruye el objeto.
                if (tiempoParaDesaparecer <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Este método implementa la función Morir de la interfaz IDaniable
    /// y sobreescribe la de la clase base A1_A1_Enemigo.
    /// </summary>
    public void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;

        anim.SetBool("life", false);
        Debug.Log("Arquera Elfa ha muerto. Desaparecerá en " + DURACION_DESAPARICION + " segundos.");

        // Desactivar componentes para que deje de moverse e interactuar.
        if (agent != null) agent.enabled = false;
        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        // --- REEMPLAZO DE LA COROUTINE ---
        // Inicia el temporizador para la destrucción del objeto.
        tiempoParaDesaparecer = DURACION_DESAPARICION;
    }

    // --- COROUTINE ELIMINADA ---
    // private IEnumerator DesaparecerDespuesDeSegundos(float segundos) { ... }

    /// <summary>
    /// Este método implementa la función RecibirDanio de la interfaz IDaniable
    /// y sobreescribe la de la clase base A1_A1_Enemigo.
    /// </summary>
    public void RecibirDanio(int cantidad)
    {
        if (estaMuerto) return;

        anim.SetTrigger("danio");

        // Usa la propiedad de la interfaz para reducir la vida.
        // Esto es más limpio que castear con 'as'.
        IDaniable daniable = this;
        daniable.vidaActual -= cantidad;

        // Si la vida llega a cero, llama al método Morir de la interfaz.
        if (daniable.vidaActual <= 0)
        {
            daniable.Morir();
        }
    }

    #region Metodos Heredados (Sin cambios)
    public override void Atacar(Vector3 Destino, string Nombre = "")
    {
        if (estaMuerto) return;
        // Tu lógica de ataque aquí...
    }

    public override void Colisiono(GameObject Colision, string TipoDeColision)
    {
        Debug.Log(Colision + " | " + TipoDeColision, gameObject);
    }

    public override void Detenerse()
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }

    public override void IrAlDestino(Vector3 destino)
    {
        if (estaMuerto || agent == null) return;
        agent.isStopped = false;
        agent.SetDestination(destino);
    }

    public override void OnEnabled() { }
    public override void OnDisabled() { }
    public override void OnCollision(Collision collider) { }

    void IDaniable.RecibirDanio(int cantidad)
    {
        throw new System.NotImplementedException();
    }

    void IDaniable.Morir()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}