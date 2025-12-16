using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro;



public class LevelManager : MonoBehaviour
{

    public enum DefeatCondition { PlayerDeath, TimeOut }


    [Header("Configuración de Tiempo")]
    [Tooltip("Tiempo máximo en segundos para completar la oleada. (5 minutos = 300 segundos)")]
    public float maxTime = 300f;

    public TMP_Text timerText;
    private int minutos, segundos;

    private float currentTime;
    private bool timerIsRunning = false;
    private AccionesJugador AccionesJugador;

   
    


    void Start()
    {
        InitializeLevel();
    }

    void Update()
    {
        if (enemigosActivos.Instance != null && !enemigosActivos.Instance.AreAnyEnemiesAlive())
        {
            Victoria();   
        }
        if (AccionesJugador != null && AccionesJugador.estaMuerto)
        {
            Derrota();
        }
        if (timerIsRunning)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                if(currentTime < 0)
                {
                    currentTime = 0;
                }

            }
            else
            {
                currentTime = 0;
                timerIsRunning = false;
                Derrota();

            }
           
        }
        minutos = (int)(currentTime / 60f);
        segundos = (int)(currentTime - minutos * 60f);
        timerText.text = minutos.ToString("00") + ":" + segundos.ToString("00");
    }



    public void InitializeLevel()
    {
        currentTime = maxTime;
        timerIsRunning = true;
        AccionesJugador = FindObjectOfType<AccionesJugador>();
    }

   private void Victoria()
    {
        timerIsRunning = false;
        Debug.Log("¡VICTORIA! Todos los enemigos han sido derrotados.");
        // Aquí puedes agregar lógica adicional para manejar la victoria, como cargar una nueva escena o mostrar una pantalla de victoria.
    }

    private void Derrota()
    {
        timerIsRunning = false;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("¡DERROTA! El jugador ha muerto.");
        SceneManager.LoadScene(currentSceneIndex);
    }
}