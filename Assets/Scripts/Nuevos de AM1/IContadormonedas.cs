using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IContadormonedas
{
   int ContadorDeMonedas {  get; }

   public void SumarMonedas(int Cantidad);
   public void ActualizarTextoMonedas();
}