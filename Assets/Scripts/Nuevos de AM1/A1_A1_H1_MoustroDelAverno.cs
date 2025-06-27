using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class A1_A1_H1_MoustroDelAverno : A1_A1_Enemigo
{
    public GameObject BolaDeAtaque;
    public GameObject AtaqueActual;

    public GameObject PadreDebarraDevida;
    public GameObject BarraDeVida;

    private float anchoOriginal;
    private bool estaMuerto = false;
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
            // Si, la animacion termino
            if (t == 0.95f)
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

    public override void Morir()
    {
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

    public bool RecibiraDobleDanoLaProximaVez = false;
    public AudioSource S_RecibirDano; // Sonido al recibir daño

    public bool PendienteDeCargaElectrica = false;
    public ReportadorDeLasColisiones CargaElectrica;

    public override void RecibirDanio(int cantidad)
    {
        //if()
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
         
            Color amarilloElectrico = new Color(1f, 0.858f, 0.039f);
            Feedbacks.Componente.UIFadeComboScript.MostrarTexto("¡DESCARGA ENCADENADA!", amarilloElectrico);

            PendienteDeCargaElectrica = false;
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

        Color celesteHielo = new Color(0.5f, 1f, 1f);
        Feedbacks.Componente.UIFadeComboScript.MostrarTexto("¡FRACTURA CONGELADA!", celesteHielo);
        
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

    }



    protected override void Update()
    {
        base.Update(); // Llama al Update del padre
        float velocidad = agent.velocity.magnitude;
        //Debug.Log("Velocidad agente: " + velocidad);
        anim.SetFloat("velocidad", velocidad);
        ActualizarBarraDevida();
        if (Vector3.Distance(transform.position, DestinoAsignado) < 2f)
        {
            Detenerse();
        }
        if (Vida < 10)
        {
            Morir();
        }

        if (EfectoDeCongelado == null && Congelado)
        {
            Congelado = false; // Si el efecto de congelación ya no existe, desactivar el estado de congelación
            RecibiraDobleDanoLaProximaVez = false; // Reanudar el estado del monstruo
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

}
