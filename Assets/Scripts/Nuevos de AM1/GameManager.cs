using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    /*
     + PadreDeTextDeCreditos : GameObject

+ ContadorDeMonedas


    + IniciarPartida() : void
+ MostrarCreditos() : void
+ OcultarCreditos() : void
+ PausarYMostrarMenu () : void
+ Reiniciar () : void
     */



    public static GameManager Componente;



    // Propiedades públicas
    public GameObject PadreDeTextDeCreditos; // Objeto donde se muestran los créditos


    void Start()
    {
        // Inicialización si es necesario
    }



    void Update()
    {
        if (Componente == null)
        {
            Componente = this;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            PosicionDelMouseEnElEspacio = hit.point;
        }
    }

    // Método para iniciar la partida
    public void IniciarPartida()
    {
        // Lógica para iniciar la partida (por ejemplo, cargar la escena, resetear puntuaciones)
        Debug.Log("Partida Iniciada");
    }

    // Método para mostrar los créditos
    public void MostrarCreditos()
    {
        // Activar el GameObject que contiene los créditos
        PadreDeTextDeCreditos.SetActive(true);
    }

    // Método para ocultar los créditos
    public void OcultarCreditos()
    {
        // Desactivar el GameObject de los créditos
        PadreDeTextDeCreditos.SetActive(false);
    }

    // Método para pausar el juego y mostrar el menú
    public void PausarYMostrarMenu()
    {
        Time.timeScale = 0; // Pausar el juego
        // Aquí podrías activar una UI de menú (si tienes una)
        Debug.Log("Juego Pausado y Menú Mostrado");
    }

    // Método para reiniciar la partida
    public void Reiniciar()
    {
        Debug.Log("Partida Reiniciada");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void CambiarDeEscena(string NombreDeLaEscena)
    {
        Debug.Log("Partida Reiniciada");

        if (string.IsNullOrEmpty(NombreDeLaEscena))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else
            SceneManager.LoadScene(NombreDeLaEscena);
    }


    public static Vector3 PosicionDelMouseEnElEspacio;

    public void Victoria(Collider Objeto)
    {
        if(Objeto.gameObject.name != "Jugador 1")
        {
            Debug.LogError("Objeto es nulo al intentar registrar victoria.");
            return;
        }
        Debug.Log("Victoria alcanzada con " + Objeto.name);
        SceneManager.LoadScene("Victoria");
    }
}
