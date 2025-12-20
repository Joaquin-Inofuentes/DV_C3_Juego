using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class arbolEnllamas : MonoBehaviour
{
    bool fuego = false;
    public float tiempoParaFuego = 2.5f;
    public float tiempoParaDesaparecer = 2.0f;
    public GameObject efectoEncendido;
    public Transform puntoDeEfecto;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (fuego) return;
        if (collision.gameObject.CompareTag("HechizoDeFuego"))
        {
            StartCoroutine(Prendidofuego());
            Destroy(collision.gameObject);
            Destroy(this.gameObject, tiempoParaFuego + tiempoParaDesaparecer);
        }
    }

    IEnumerator Prendidofuego()
    {
        fuego = true;
        if (efectoEncendido != null)
        {
            GameObject fx = Instantiate(efectoEncendido, puntoDeEfecto.position, Quaternion.identity);
            fx.SetActive(true);
            ParticleSystem ps = fx.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                ps.Clear(true);
                ps.Play(true);
            }

            Destroy(fx, tiempoParaFuego + 0.5f);
        }
        yield return new WaitForSeconds(tiempoParaFuego);

    }

}
