using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teletransportacion_Sonoro : MonoBehaviour
{
    public string NombreDelClip = "";
    public void ReproducirSonido(Vector3 Posicion)
    {
        GameObject Objeto = AudioManager.CrearEfectoSonoro(Posicion, NombreDelClip);
    }
}
