using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A2_H5_TierraSanta : A2_Trampa
{
 
public class TierraSanta : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {

            if (other.gameObject.name.Contains("Jugador 1"))
            {
                Debug.Log("Se registro entrada");
            } 
   }


private void OnTriggerExit(Collider other)
{
        AccionesJugador jugador = other.GetComponent<AccionesJugador>();
        if (jugador != null && jugador._TimerManager != null)
        {
            jugador._TimerManager.magiaBloqueadaPorZona = false;
            Debug.Log("Salió de Tierra Santa: magia desbloqueada");
        }
}
}

public override void Activate()
{
    base.Activate();
}

public override void Desactivar()
{
    base.Desactivar();
    // Agregar Logica de desactivar Sonido, Apariencia, Creacion etc
}

public override void OnCollisionEnter(Collision collider)
{
    throw new System.NotImplementedException();
}

protected override void Start()
{
    base.Start(); // Llama al Start del padre
                  // Código propio de ArquerasElfas
}

protected override void Update()
{
    base.Update(); // Llama al Update del padre
                   // Código propio de ArquerasElfas
}
}