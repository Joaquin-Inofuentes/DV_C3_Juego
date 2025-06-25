using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//tp2_Damian Figueredo
//get
public interface IContadormonedas
{
   int ContadorDeMonedas {  get; }

   public void SumarMonedas(int Cantidad);
   public void ActualizarTextoMonedas();
}