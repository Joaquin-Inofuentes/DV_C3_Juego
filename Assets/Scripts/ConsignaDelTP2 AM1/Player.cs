using UnityEngine;
using RTS.Comunicacion;

public class Player : EntidadBase, IPingSender<EntidadBase>, IPingListener<EntidadBase>
{
    public void EnviarPing(TipoMensaje tipo)
    {
        Debug.Log($"[Player] Emite ping de tipo {tipo}");
    }

    public void RecibirPing(EntidadBase emisor, TipoMensaje tipo)
    {
        if (tipo == TipoMensaje.Posicion)
        {
            estadoActual = EstadoIA.Atacando;
        }
    }

    protected override void OnEnable() => EventoPing.OnPing += Escuchar;
    protected override void OnDisable() => EventoPing.OnPing -= Escuchar;

    public void Suscribirse()
    {
        EventoPing.OnPing += Escuchar;
    }

    void Escuchar(IPingSender<EntidadBase> emisor, IPingListener<EntidadBase> receptor, TipoMensaje tipo)
    {
        if (object.ReferenceEquals(receptor, this))
            RecibirPing(emisor as EntidadBase, tipo);
    }

    public override void EjecutarCadaMedioSegundo()
    {
        //Debug.Log(gameObject.name, gameObject);
    }
}