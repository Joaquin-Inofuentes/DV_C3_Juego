using UnityEngine;
using RTS.Comunicacion;
using System.Linq; // Asegúrate de tener esto para usar Any y métodos de extensión

public class Enemigo : EntidadBase, IPingListener<EntidadBase>, IPingSender<EntidadBase>
{
    public A1_A1_Enemigo Componente;

    public GameObject EnemigoFijado;
    public override void EjecutarCadaMedioSegundo()
    {

        //Debug.Log($"{name} ejecutando lógica de IA.");



        if (EnemigoFijado != null && EnemigoFijado.activeInHierarchy)
        {

            // Si el enemigo fijado está dentro del rango de persecución y no está en rango de ataque cuerpo a cuerpo
            if (Vector3.Distance(transform.position, EnemigoFijado.transform.position) < Componente.DistanciaParaPerseguir
                &&
                Vector3.Distance(transform.position, EnemigoFijado.transform.position) > Componente.DistanciaParaAtaqueMelee
                )
            {
                Componente?.IrAlDestino(EnemigoFijado.transform.position);
                estadoActual = EstadoIA.Persiguiendo;
                Debug.Log($"[Enemy] {name} PERSIGUIENDO a {EnemigoFijado.name}");
            }
            // Si el enemigo fijado está dentro del rango de ataque cuerpo a cuerpo
            else if (Vector3.Distance(transform.position, EnemigoFijado.transform.position) < Componente.DistanciaParaAtaqueMelee)
            {
                Componente.Atacar(EnemigoFijado.transform.position);
                Componente.Detenerse();
                estadoActual = EstadoIA.Atacando;
                //Debug.Log($"[Enemy] {name} ATACANDO a {EnemigoFijado.name}");
                return;
            }
            // Si o si persigue al enemigo. Aunque se aleje mucho, lo persigue
            else if (Vector3.Distance(transform.position, EnemigoFijado.transform.position) > Componente.DistanciaParaAtaqueMelee)
            {
                Componente?.IrAlDestino(EnemigoFijado.transform.position);
                estadoActual = EstadoIA.Persiguiendo;
                Debug.Log($"[Enemy] {name} PERSIGUIENDO a {EnemigoFijado.name}");
            }

            return;
        }
        else
        {
            BuscarEnemigoCercano();
        }

    }

    public void BuscarEnemigoCercano()
    {
        int IndiceDelEnemigoMasCercano = entidadesCercanas.IndiceConComponente<Player, EntidadBase>();
        if (IndiceDelEnemigoMasCercano != -1)
        {
            Debug.Log(IndiceDelEnemigoMasCercano);
            EnemigoFijado = entidadesCercanas[IndiceDelEnemigoMasCercano].gameObject;
        }
    }


    public void RecibirPing(EntidadBase emisor, TipoMensaje tipo)
    {
        //Debug.Log($"[Enemy] {name} recibió {tipo} desde {emisor.name}");
        if (emisor.GetComponent<AccionesJugador>() == null)
        {
            return;
        }
        if (tipo == TipoMensaje.Posicion)
        {
            if (Componente == null) return;
            float distancia = Vector3.Distance(transform.position, emisor.transform.position);
            if (distancia < Componente.DistanciaParaAtaqueMelee)
            {
                Componente.Atacar(emisor.transform.position);
                estadoActual = EstadoIA.Atacando;
                //Debug.Log($"[Enemy] {name} ATACANDO a {emisor.name}");
            }
            else if (distancia < Componente.DistanciaParaPerseguir)
            {
                Componente.IrAlDestino(emisor.transform.position);
                Debug.DrawLine(transform.position, emisor.transform.position, Color.red, 2f);
                estadoActual = EstadoIA.Persiguiendo;
                //Debug.Log($"[Enemy] {name} PERSIGUIENDO a {emisor.name}");
            }
        }
    }

    public void EnviarPing(TipoMensaje tipo)
    {
        Debug.Log($"[Enemy] {name} emite ping de tipo {tipo}");
    }

    protected override void OnEnable() => EventoPing.OnPing += Escuchar;
    protected override void OnDisable() => EventoPing.OnPing -= Escuchar;

    void Escuchar(IPingSender<EntidadBase> emisor, IPingListener<EntidadBase> receptor, TipoMensaje tipo)
    {
        if (object.ReferenceEquals(receptor, this))
            RecibirPing(emisor as EntidadBase, tipo);
    }
}