using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tp2_Damian Figueredo
//get y set
interface IDaniable
{
    int vidaActual { get; set; }
    int vidaMaxima { get; }

    public void RecibirDanio(int cantidad);
    public void Morir();

}
