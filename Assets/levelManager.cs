using UnityEngine;
using UnityEngine.SceneManagement; 



public class LevelManager : MonoBehaviour
{

    public enum DefeatCondition { PlayerDeath, TimeOut }


    [Header("Configuración de Tiempo")]
    [Tooltip("Tiempo máximo en segundos para completar la oleada. (5 minutos = 300 segundos)")]
    public float maxTime = 300f;

    [Header("Configuración de Enemigos")]
    [Tooltip("Total de enemigos que el jugador debe derrotar en esta oleada.")]
    public int totalEnemies;



    private float currentTime;
    private bool timerIsRunning = false;
    private AccionesJugador AccionesJugador;

   
    


    void Start()
    {
        InitializeLevel();
    }

    void Update()
    {
        if (AccionesJugador != null && AccionesJugador.estaMuerto )
        {
            Derrota();
        }
        if (timerIsRunning)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                 Debug.Log("Tiempo restante: " + FormatTime(currentTime));
            }
            else
            {
                currentTime = 0;
                timerIsRunning = false;
                Derrota();

            }
        }
    }


    public void InitializeLevel()
    {
        currentTime = maxTime;
        timerIsRunning = true;
        AccionesJugador = FindObjectOfType<AccionesJugador>();
    }

   

    private void Derrota()
    {
        timerIsRunning = false;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("¡DERROTA! El jugador ha muerto.");
        SceneManager.LoadScene(currentSceneIndex);
    }






    public string FormatTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}