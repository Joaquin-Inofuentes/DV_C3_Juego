using System.Collections.Generic;
using UnityEngine;
using RTS.Comunicacion;
using RTS.GridSystem;

public class PingManager : MonoBehaviour
{
    public float intervalo = 0.2f;
    public float tiempo;
    public static List<EntidadBase> todasEntidades = new();
    public int CantidadEntidades => todasEntidades.Count;
    public int CantidadEntidadesRegistradas;
    public static void RegistrarEntidad(EntidadBase e)
    {
        if (!todasEntidades.Contains(e))
            todasEntidades.Add(e);
    }

    public static void RemoverEntidad(EntidadBase e)
    {
        if (todasEntidades.Contains(e))
            todasEntidades.Remove(e);
    }

    void Update()
    {
        tiempo += Time.deltaTime;
        if (tiempo >= intervalo)
        {
            CantidadEntidadesRegistradas = CantidadEntidades;
            if (todasEntidades.Contains(null) || todasEntidades.Count == 0) // Evitar errores si hay entidades nulas
            {
                EscanearEntidadesEnEscena();
            }
            tiempo = 0f;
            EjecutarPings();
        }
    }

    public static void EscanearEntidadesEnEscena()
    {
        todasEntidades.Clear();
        EntidadBase[] entidades = GameObject.FindObjectsOfType<EntidadBase>();
        foreach (var entidad in entidades)
        {
            todasEntidades.Add(entidad);
            RegistrarEntidad(entidad);
            entidad.gameObject.SetActive(false);
            entidad.gameObject.SetActive(true);
        }
    }



    void EjecutarPings()
    {
        //Debug.Log($"[PingManager] Ejecutando pings para {todasEntidades.Count} entidades registradas.");
        foreach (var entidad in todasEntidades)
        {
            // ActualizarPosicionYEnviarPing
            #region 
            Vector3 actual = entidad.transform.position;
            //Debug.Log($"[PingManager] {entidad.name} ha cambiado de posición: {entidad.ultimaPosicion} -> {actual}");
            entidad.ultimaPosicion = actual;
            GridManager.ActualizarCelda(entidad, actual);

            if (entidad is IPingSender<EntidadBase> sender)
            {
                if (GridManager.grilla.Count == 0)
                {
                    GridManager.CompletarMapaEntidadesSiVacio();
                }

                var cercanos = GridManager.ObtenerCercanos(actual, entidad.gameObject.GetComponent<A1_Entidad>().DistanciaParaPerseguir);
                //Debug.Log($"[__PingManager] Entidades cercanas a {entidad.name}: {cercanos.Count} encontradas.");
                List<EntidadBase> entidadesValidas = new List<EntidadBase>();
                foreach (var otro in cercanos)
                {
                    //Debug.Log($"[PingManager] Evaluando entidad cercana: {otro.name}");
                    //          [PingManager] Evaluando entidad cercana: Jugador 1
                    if (otro is IPingListener<EntidadBase> listener && otro != entidad)
                    {
                        //Debug.Log("Se actualizo la posicion");
                        entidadesValidas.Add(otro);
                        //Debug.Log($"[PingManager] {otro.name} es un oyente válido. Enviando ping.");
                        EventoPing.OnPing?.Invoke(sender, listener, TipoMensaje.Posicion);
                    }
                }
                entidad.entidadesCercanas = entidadesValidas;

            }
            #endregion

            entidad.EjecutarCadaMedioSegundo();
        }
    }
}