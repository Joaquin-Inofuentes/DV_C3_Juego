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





/*
 * 
 * 



___Parcial 2 Correcciones
#1. Cuidado con la cantidad de escenas creadas, si no la usan se borra para reducir peso en el proyecto.



#3. Al hacer ataques deberian usar interfaces para poder atacar a cualquier enemigo en un futuro y no solo a uno. HECHO
#5. En el AudioManager la key de los sonidos podría ser un enum. NO
    Respuesta de alumno = "Sorry. Es q eso iria en contra de la idea principal del audio manager de que usa resources para obtener todos los audios"
#6. Si todos los enemigos actualizan su barra de vida de la misma manera lo deberían programar en la clase padre.
#7. Las Interfaces creadas no se usan en ningún lado.




_ Final
La consigna del final pide lo siguiente. Y entre parentesis esta la propuesta q la hable con el profe y me dio su OK. De en donde aplicar cada una



#9. Events (Accionar acciones de todos los suscriptos)
#13. Getters y setters (Feedback visual al aumentar vida o recibir danio y monedas)
#11. Diccionarios (AudioManager.Clips [String Nombre, Clip Audio])


#4. El codigo del player se encarga de demasiadas cosas, no deberia actualizar el canvas por ejemplo. Y tienen un sistema tan complejo de ataque que se merece un script en particular para administrar eso
#2. Para los nombres de los ataques es mejor que usen enums
#8. Delegates (Ataque físico y mágico)
#10. Structs (ataques [tipo, daño, melee])
#12. Enums (Modo de pelea Físico/Mágico)

















___Parcial 2 Correcciones
#1. Cuidado con la cantidad de escenas creadas, si no la usan se borra para reducir peso en el proyecto.
#2. Para los nombres de los ataques es mejor que usen enums
#3. Al hacer ataques deberian usar interfaces para poder atacar a cualquier enemigo en un futuro y no solo a uno.
#4.0. El codigo del player se encarga de demasiadas cosas, 
    #4.1. no deberia actualizar el canvas por ejemplo.
    #4.2. Y tienen un sistema tan complejo de ataque que se merece un script en particular para administrar eso
#5. En el AudioManager la key de los sonidos podría ser un enum.
        Respuesta de alumno = 
            "Sorry. Es q eso iria en contra de la idea principal del audio manager de que usa resources para obtener todos los audios"
#6. Si todos los enemigos actualizan su barra de vida de la misma manera lo deberían programar en la clase padre.
#7. Las Interfaces creadas no se usan en ningún lado.




_ Final
La consigna del final pide lo siguiente. Y entre parentesis esta la propuesta q la hable con el profe y me dio su OK. De en donde aplicar cada una
#8. Delegates (Ataque físico y mágico)
#9. Events (Accionar acciones de todos los suscriptos)
#10. Structs (ataques [tipo, daño, melee])
#11. Diccionarios (Key Enum de ataques Value acciones)
#12. Enums (Modo de pelea Físico/Mágico)
#13. Getters y setters (Feedback visual al aumentar vida o recibir danio y monedas q lo hara )







 */ 