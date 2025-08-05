using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tp2_Damian Figueredo
//get y set
interface IDaniable
{
    int vidaActual { get; set; }
    int vidaMaxima { get; set; }

    // #3. Al hacer ataques deberian usar interfaces para poder atacar a cualquier enemigo en un futuro y no solo a uno.
    public void RecibirDanio(int cantidad);
    // #7. Las Interfaces creadas no se usan en ningún lado.
    public void Morir();

}
