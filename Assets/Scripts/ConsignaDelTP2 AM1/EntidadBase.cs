using UnityEngine;
using RTS.GridSystem;
using RTS.Comunicacion;
using System.Collections.Generic;

public abstract class EntidadBase : MonoBehaviour
{
    public Vector2Int CeldaActual => GridManager.PosToCelda(transform.position);
    public Vector3 ultimaPosicion;
    public EstadoIA estadoActual;
    public List<EntidadBase> entidadesCercanas = new List<EntidadBase>();

    protected virtual void Start()
    {
        ultimaPosicion = transform.position;
        GridManager.Registrar(this);
        PingManager.RegistrarEntidad(this);
    }

    protected virtual void OnDestroy()
    {
        GridManager.Remover(this);
        PingManager.RemoverEntidad(this);
    }

    protected virtual void OnDisable()
    {
        PingManager.RemoverEntidad(this);
    }

    protected virtual void OnEnable()
    {
        ultimaPosicion = transform.position;
        PingManager.RegistrarEntidad(this);
    }
    void OnTriggerEnter(Collider other)
    {
        EntidadBase otraEntidad = other.GetComponent<EntidadBase>();
        if (otraEntidad != null && otraEntidad != this && !entidadesCercanas.Contains(otraEntidad))
        {
            entidadesCercanas.Add(otraEntidad);
        }
    }

    void OnTriggerExit(Collider other)
    {
        EntidadBase otraEntidad = other.GetComponent<EntidadBase>();
        if (otraEntidad != null && entidadesCercanas.Contains(otraEntidad))
        {
            entidadesCercanas.Remove(otraEntidad);
        }
    }


    public abstract void EjecutarCadaMedioSegundo();

}