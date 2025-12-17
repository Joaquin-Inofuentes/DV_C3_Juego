using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro;

public class LevelManager : MonoBehaviour
{
    public enum DefeatCondition { PlayerDeath, TimeOut }

    [Header("UI y Efectos")]
    public TextMeshProUGUI timerText; // Arrastra tu texto aquí
    public float fontSizeNormal = 36f;
    public float fontSizeGrande = 50f;
    public float velocidadEfecto = 5f;

    [Header("Configuración de Tiempo")]
    [Tooltip("Tiempo máximo en segundos para completar la oleada. (5 minutos = 300 segundos)")]
    public float maxTime;

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
                ActualizarInterfaz();

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
      

    }
    void ActualizarInterfaz()
    {
        if (timerText == null) return;

        timerText.text = FormatTime(currentTime);

        // EFECTO CUANDO QUEDAN 10 SEGUNDOS O MENOS
        if (currentTime <= 10f && currentTime > 0)
        {
            // Cambiamos el color a rojo
            timerText.color = Color.red;

            // Efecto de "Latido" (se agranda y achica)
            // Mathf.PingPong oscila entre 0 y 1
            float lerp = Mathf.PingPong(Time.time * velocidadEfecto, 1);
            timerText.fontSize = Mathf.Lerp(fontSizeNormal, fontSizeGrande, lerp);
        }
        else
        {
            // Estado normal
            timerText.color = Color.white;
            timerText.fontSize = fontSizeNormal;
        }
    }
    public string FormatTime(float timeToDisplay)
    {
        if (timeToDisplay < 0) timeToDisplay = 0;

        int minutos = Mathf.FloorToInt(timeToDisplay / 60);
        int segundos = Mathf.FloorToInt(timeToDisplay % 60);

        return string.Format("{0:00}:{1:00}", minutos, segundos);
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