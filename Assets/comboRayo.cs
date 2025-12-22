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
    public Transform puntoDeEfecto;
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
        
        if (enemigoScript == null || enemigoScript.estaMuerto) return;
        if (tipo == TipoAtaque.RayoElectrico)
        {
    
            contadorCombo++;
            if (contadorCombo >= 2 && !estatico ) 
            {
                ActivarEstatico();
            }
        }
        if (estatico)
        {
            Debug.Log(tipo);

          if( tipo == TipoAtaque.Melee1)
            {
                Debug.Log("2");
                SuperRayo();
                
            }
        }
    }

    public void ActivarEstatico() 
    {
        estatico = true;
        Debug.Log(estatico);
        contadorCombo = 0;
        if (temporizadorEstatico != null) StopCoroutine(temporizadorEstatico);
        temporizadorEstatico = StartCoroutine(TimerEstatico());
        if (vfxestatica != null)
        {
            GameObject fx = Instantiate(vfxestatica, puntoDeEfecto.position, Quaternion.identity);
            fx.SetActive(true);
            ParticleSystem ps = fx.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                ps.Clear(true);
                ps.Play(true);
            }

            Destroy(fx, tiempodeCombo);
        }
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
        if (vfxRayoCae != null)
        {
            GameObject fx = Instantiate(vfxRayoCae, puntoDeEfecto.position, Quaternion.identity);
            fx.SetActive(true);
            ParticleSystem ps = fx.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                ps.Clear(true);
                ps.Play(true);
            }

            Destroy(fx, tiempodeCombo);
        }
    }
}
