using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueJugador : MonoBehaviour
{
    public enum ModoPelea { Rango, Melee }
    public enum TipoAtaque { BolaDeFuego, BolaDeHielo, RayoElectrico, Melee1, Melee2, Melee3 }

    public ModoPelea modoActual = ModoPelea.Rango;
    public TimerManager _TimerManager;
    public Animator anim;
    public Transform Origen;
    public float fuerzaDisparo = 500f;
    public bool estaMuerto = false;

    [Header("Prefabs")]
    public GameObject BolaDeFuego;
    public GameObject BolaDeHielo;
    public GameObject Rayo;
    public GameObject PrefabDeComboFuego;
    public GameObject PrefabDeComboElectrico;
    public GameObject VFX_ComboExplosion;
    public AudioClip SonidoComboExplosion;

    [Header("Combos")]
    public List<string> AtaquesQImpactaron = new List<string>();
    public float TimerDeCombos = 0f;
    public Transform OrigenDelDisparo;

    private Dictionary<TipoAtaque, Iataque> _ataques;

    void Start()
    {
        _ataques = new Dictionary<TipoAtaque, Iataque>
        {
            { TipoAtaque.BolaDeFuego, new ataqueFuego() },
            { TipoAtaque.Melee1, new ataqueFuego() }, 
            { TipoAtaque.BolaDeHielo, new ataqueHielo() },
            { TipoAtaque.Melee2, new ataqueHielo() },
            { TipoAtaque.RayoElectrico, new ataqueRayo() },
            { TipoAtaque.Melee3, new ataqueRayo() }
        };
    }

    public void Atacar(Vector3 destino, string nombre)
    {
        if (estaMuerto || _TimerManager == null || _TimerManager.IsTimerCharging(6))
            return;

        if (Enum.TryParse(nombre, true, out TipoAtaque tipo) && _ataques.TryGetValue(tipo, out var ataque))
        {
            _TimerManager.SetTimerToMax(6);
            ataque.Ejecutar(this, destino);
        }
        else
        {
            Debug.LogWarning($"Ataque '{nombre}' no encontrado.");
        }
    }

    public void InstanciarProyectil(GameObject prefab, string nombreAtaque, Vector3 destino)
    {
        transform.LookAt(destino);
        Vector3 direccion = (destino - Origen.position).normalized;
        Quaternion rotacion = Quaternion.LookRotation(direccion);

        GameObject ataqueInstanciado = Instantiate(prefab, Origen.position, rotacion);
        ataqueInstanciado.name = nombreAtaque;

        if (ataqueInstanciado.TryGetComponent<Rigidbody>(out var rb))
            rb.AddForce(direccion * fuerzaDisparo);

        if (ataqueInstanciado.TryGetComponent<Proyectil>(out var proyectil))
            proyectil.Creador = gameObject;

        if (ataqueInstanciado.TryGetComponent<Hitbox>(out var hitbox))
            hitbox.Creador = gameObject;

        Destroy(ataqueInstanciado, 10f);
    }
}