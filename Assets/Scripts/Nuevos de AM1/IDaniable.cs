using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    interface IDaniable
{
    int vidaActual {  get; set; }
    int vidaMaxima { get; }

    public void RecibirDanio(int cantidad);
    public void Morir();

}
