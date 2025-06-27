using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodigoGenerico : MonoBehaviour
{
    public AudioClip Sonido;
    public void EjecutarSonido()
    {
        AudioManager.CrearEfectoSonoro(transform.position,Sonido);
    }
}
