using System.Collections;
using System.Collections.Generic;
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
    public override void Atacar(Vector3 Destino, string Nombre = "")
    {
        if (estaMuerto) return;

        AnimatorClipInfo[] clips = anim.GetCurrentAnimatorClipInfo(0);
        if (clips.Length > 0)
        {
            AnimacionActual = clips[0].clip.name;
        }
        //Debug.Log(AnimacionActual);
        if (AnimacionActual.Contains("idle"))
        {
            int ataqueIndex = Random.Range(0, 2); // 0 o 1

            if (ataqueIndex == 0)
            {
                anim.SetTrigger("ataque1");
            }
            else
            {
                anim.SetTrigger("ataque2");
            }
        }
        else if (AnimacionActual.Contains("ataque"))
        {
            //Debug.Log(1);
            if (VerificaSiPuedeAtacar())
            {
                //Debug.Log(2);
                CreaEfectoDeAtaque(Destino);
            }
        }
    }

    bool avisoHecho = false;

    bool VerificaSiPuedeAtacar()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        float t = state.normalizedTime;
        //Debug.Log(t  + " | "+ avisoHecho);
        if (!avisoHecho && t >= 0.95f)
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

    public void CreaEfectoDeAtaque(Vector3 Destino)
    {
        //ModoAtaqueMelee = false;
        if (AtaqueActual == null)
        {

            //Debug.Log(1);
            //Debug.Log("Atacando");
            // Crea un efecto de danio
            //Debug.Log("Atacando");
            // Crea un efecto de daño
            GameObject Ataque = Instantiate(BolaDeAtaque, Destino, Quaternion.identity);
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
        agent.isStopped = true;

    }

    public override void IrAlDestino(Vector3 destino)
    {
        if (estaMuerto) return;
        agent.isStopped = false;
        agent.SetDestination(destino);
    }

    public override void Morir()
    {
        agent.enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        PadreDebarraDevida.SetActive(false);
        // transform.Translate(0, -0.7f, 0); Correcion del bug de eliminar al boss
        anim.SetBool("life", false);
        Debug.Log("Falta animacion de morir");
        StartCoroutine(DesaparecerDespuesDeSegundos(10f)); // espera 3 segundos
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

    public override void RecibirDanio(int cantidad)
    {
        Debug.Log(1);
        Vida -= cantidad;

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
            Vida -= cantidad * 2;
            Congelado = false;
        }
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

    }
}
