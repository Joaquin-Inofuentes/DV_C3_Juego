using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro;

public class LevelManager : MonoBehaviour
{
    public enum DefeatCondition { PlayerDeath, TimeOut }

    [Header("Configuración de Tiempo")]
    [Tooltip("Tiempo máximo en segundos para completar la oleada. (5 minutos = 300 segundos)")]
    public float maxTime;

    public TMP_Text timerText;
    private int minutos, segundos;

    private float currentTime;
    private bool timerIsRunning = false;
    private AccionesJugador AccionesJugador;

    void Awake()
    {
        Debug.Log("LEVEL MANAGER AWAKE", this);
    }

    void Start()
    {
        if (timerText == null)
        {
            Debug.LogError("❌ timerText NO está asignado en el LevelManager", this);
        }

        Debug.Log("[LevelManager] Start ejecutado", this);
        InitializeLevel();
    }


    void Update()
    {
        // ❌ NO en modo supervivencia
        /*
        if (enemigosActivos.Instance != null && 
            !enemigosActivos.Instance.AreAnyEnemiesAlive())
        {
            Victoria();   
        }
        */

        if (AccionesJugador != null && AccionesJugador.estaMuerto)
        {
            Derrota();
        }
        if (timerIsRunning)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                Debug.Log("CurrentTime ahora: " + currentTime);

                if (currentTime < 0)
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
        Debug.Log(currentTime);
        Debug.Log("deltaTime: " + Time.deltaTime + " | timeScale: " + Time.timeScale);
        Debug.Log("timerIsRunning = " + timerIsRunning);

        minutos = (int)(currentTime / 60f);
        segundos = (int)(currentTime - minutos * 60f);
        timerText.text = minutos.ToString("00") + ":" + segundos.ToString("00");

    }



    public void InitializeLevel()
    {
        currentTime = maxTime;
        timerIsRunning = true;
        Debug.Log("Timer inicializado en: " + currentTime);

        AccionesJugador = FindObjectOfType<AccionesJugador>();
    }

   private void Victoria()
    {
        timerIsRunning = false;
        Debug.Log("¡VICTORIA! Todos los enemigos han sido derrotados.");
    }

    private void Derrota()
    {
        timerIsRunning = false;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("¡DERROTA! El jugador ha muerto.");
        SceneManager.LoadScene(currentSceneIndex);
    }
}