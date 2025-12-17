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
     
        if (AccionesJugador != null && AccionesJugador.estaMuerto)
        {
            Derrota();
        }
        if (timerIsRunning)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
              
                if (currentTime < 0)
                {
                    currentTime = 0;
                }

            }
            else
            {
                currentTime = 0;
                timerIsRunning = false;
                Victoria();
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
        Debug.Log("Timer inicializado en: " + currentTime);

        AccionesJugador = FindObjectOfType<AccionesJugador>();
    }

    private void Victoria()
    {
        timerIsRunning = false;

        PlayerPrefs.SetString("RetryScene", SceneManager.GetActiveScene().name);

        SceneManager.LoadScene("Victoria");
    }


    private void Derrota()
    {
        timerIsRunning = false;

        PlayerPrefs.SetString("RetryScene", SceneManager.GetActiveScene().name);

        SceneManager.LoadScene("Derrota");
    }
}