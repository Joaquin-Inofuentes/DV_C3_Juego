using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ReportadorDeLasColisiones : MonoBehaviour
{
    public GameObject Emisor; // Este será el que se "envía" como quien tocó

    private void OnCollisionEnter(Collision c) => ProcesarColision(c.gameObject, "CollisionEnter");
    private void OnCollisionStay(Collision c) => ProcesarColision(c.gameObject, "CollisionStay");
    private void OnCollisionExit(Collision c) => ProcesarColision(c.gameObject, "CollisionExit");

    private void OnTriggerEnter(Collider other) => ProcesarColision(other.gameObject, "TriggerEnter");
    private void OnTriggerStay(Collider other) => ProcesarColision(other.gameObject, "TriggerStay");
    private void OnTriggerExit(Collider other) => ProcesarColision(other.gameObject, "TriggerExit");

    public bool Persiguiendo = true;
    public bool RecibioElMensaje = false;


    private void ProcesarColision(GameObject obj, string tipo)
    {
        if (gameObject.name == "SE TOCA Y SE GANA" && RecibioElMensaje == false)
        {
            // [DEBUG] Colisión con Jugador 1 de tipo TriggerEnter en InvisibleFase1
            if (tipo == "TriggerEnter" && obj.name == "Jugador 1")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Victoria");
            }
        }

        if (gameObject.name == "InvisibleFase1" && RecibioElMensaje == false)
        {
            // [DEBUG] Colisión con Jugador 1 de tipo TriggerEnter en InvisibleFase1
            if (tipo == "TriggerEnter" && obj.name == "Jugador 1")
            {
                Debug.Log($"[DEBUG] Colisión con {obj.name} de tipo {tipo} en {gameObject.name}", obj);
                RecibioElMensaje = true;
                Mensajes.Instance.textoTutorial.text = "¡Obedezcan las leyes del maná... o ardan con ellas!";
                Emisor.SetActive(true);
                Invoke(nameof(DesactivarPadreDeMensajes), 2f);
                gameObject.GetComponent<BoxCollider>().enabled = false;
                Mensajes.Instance.ReproducirSonidoDeAlerta();
            }
            return;
        }

        if (gameObject.name == "InvisibleFase1") return;

        if (obj.layer != 7) return;

        if (obj.name == Emisor.name)
        {
            return;
        }
        if (Emisor == null)
        {
            Debug.LogWarning("[WARNING] Emisor no asignado en CollisionInvoker.");
            return;
        }
        var Enemigo = Emisor.GetComponent<A1_Entidad>();
        if (Enemigo != null)
        {
            // Distancia hasta el enemigo
            float DistanciaAlEnemigo = Vector3.Distance(Enemigo.transform.position, obj.transform.position);
            // Si, Esta colisionando con el rango de ataque
            if (!tipo.Contains("Exit"))
            {
                // Si esta cerca para ataque melee
                if (DistanciaAlEnemigo < Enemigo.DistanciaParaAtaqueMelee && Enemigo.ModoAtaqueMelee == true && Enemigo.agent != null)
                {
                    Enemigo.agent.isStopped = true;
                    Enemigo.Atacar(obj.transform.position, "Melee");
                }

                // Si esta debo acercarme
                if (
                    (
                    (DistanciaAlEnemigo >= Enemigo.DistanciaParaAtaqueMelee && Enemigo.ModoAtaqueMelee == true)
                    ||
                    (DistanciaAlEnemigo >= Enemigo.DistanciaParaAtaqueLargo && Enemigo.ModoAtaqueMelee == false)
                    )
                    &&
                    DistanciaAlEnemigo <= Enemigo.DistanciaParaPerseguir
                    )
                {
                    if (!Enemigo.agent) return;
                    if (Enemigo.agent.enabled == false) return;
                    Enemigo.agent.isStopped = false;
                    Enemigo.Objetivo = obj.gameObject;
                    Enemigo.IrAlDestino(obj.transform.position);
                }
            }
            else
            {
                if (Enemigo.agent != null && Enemigo.agent.isStopped == false)
                {
                    Enemigo.agent.isStopped = true;
                }
            }
            var Jugador = obj.GetComponent<A1_Entidad>();
            if (Jugador != null)
            {
                Jugador.Colisiono(Emisor, tipo);

                if (obj.GetComponent<AccionesJugador>() != null && Persiguiendo
                    &&
                    DistanciaAlEnemigo <= Enemigo.DistanciaParaPerseguir)
                {
                    obj.GetComponent<AccionesJugador>().Feedbacks.FeedbackRadialVisual
                        (
                        obj.GetComponent<AccionesJugador>().Color_FueAvistado
                        , 2f
                        );
                    Persiguiendo = false;
                }
            }
            else
            {
                //Debug.Log($"Objeto sin A1_Entidad: {obj.name} ({tipo})");
            }
        }
    }

    private void DesactivarPadreDeMensajes()
    {
        Emisor.SetActive(false);
    }
}
