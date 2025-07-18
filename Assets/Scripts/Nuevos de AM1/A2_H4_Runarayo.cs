using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A2_H4_Runarayo : A2_Trampa
{
    public GameObject proyectilPrefab;
    public float fuerzaDisparo = 10f;
    public Transform[] puntosDeGeneracion = new Transform[6];
    public GameObject EfectoEspecial;
    public bool activado = true; 
    public override void Activate()
    {
        base.Activate();
        // Agregar Logica de desactivar Sonido, Apariencia, Creacion etc

    }

    public override void Desactivar()
    {
        base.Desactivar();
        
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Electrico"))
        {
           Destroy(collision.gameObject);
           GameObject efecto = Instantiate(EfectoEspecial, collision.transform.position, Quaternion.identity);
            
            for (int i = 0; i < puntosDeGeneracion.Length; i++)
            {
                Vector3 posicionGeneracion = puntosDeGeneracion[i].position;
                Vector3 direccionDisparo = puntosDeGeneracion[i].forward;
                GameObject proyectil = Instantiate(proyectilPrefab, posicionGeneracion, Quaternion.identity);

                Rigidbody rb = proyectil.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(direccionDisparo * fuerzaDisparo, ForceMode.Impulse);
                }
            }
            Destroy(efecto, 2f);
            activado = false;
        }
        else
        {

           if(collision.gameObject.name.Contains("Fuego") || collision.gameObject.name.Contains("Hielo"))
                Destroy(collision.gameObject);
        }
        
    }

    protected override void Start()
    {
        base.Start(); // Llama al Start del padre
        StartCoroutine(Activacion(3f));
    }
    IEnumerator Activacion (float segundos)
    {
        yield return new WaitForSeconds(segundos);
        activado=true;
    }

    protected override void Update()
    {
        base.Update(); // Llama al Update del padre
        // Código propio de ArquerasElfas
    }
}
