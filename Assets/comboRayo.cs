using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class comboRayo : MonoBehaviour
{
    [Header("Referencias")]
    public A1_A1_H1_MoustroDelAverno enemigoScript;
    public GameObject vfxestatica;
    public GameObject vfxRayoCae;
    [Header("Estado de combo")]
    public bool estatico;
    public int contadorCombo;
    public float tiempodeCombo;
    
    private Coroutine temporizadorEstatico;
    void Start()
    {
        enemigoScript = GetComponent<A1_A1_H1_MoustroDelAverno>();
    }

    public void ContadorCombo(TipoAtaque tipo)
    {
        Debug.Log("1");
        if (enemigoScript == null || enemigoScript.estaMuerto) return;
        if (tipo == TipoAtaque.RayoElectrico)
        {
            Debug.Log("2");
            contadorCombo++;
            if (contadorCombo >= 2 && !estatico ) 
            {
                ActivarEstatico();
            }
        }
        if (estatico)
        {
          if( tipo == TipoAtaque.Melee3)
            {
                SuperRayo();
            }
        }
    }

    public void ActivarEstatico() 
    {
        estatico = true;
        contadorCombo = 0;
        if (temporizadorEstatico != null) StopCoroutine(temporizadorEstatico);
        temporizadorEstatico = StartCoroutine(TimerEstatico());
        Instantiate(vfxestatica, transform.position, Quaternion.identity);

    }
    IEnumerator TimerEstatico()
    {
        yield return new WaitForSeconds(tiempodeCombo);
        estatico = false;
    }
    public void SuperRayo()
    {
        estatico = false;
        if (temporizadorEstatico != null) StopCoroutine(temporizadorEstatico);
        Instantiate(vfxRayoCae, transform.position, Quaternion.identity);
    }
}
