using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro;



public class LevelManager : MonoBehaviour
{

    public enum DefeatCondition { PlayerDeath, TimeOut }


    [Header("Configuración de Tiempo")]
    [Tooltip("Tiempo máximo en segundos para completar la oleada. (5 minutos = 300 segundos)")]
    public float maxTime = 300f;

    [Header("Configuración de Enemigos")]
    [Tooltip("Total de enemigos que el jugador debe derrotar en esta oleada.")]
    public int totalEnemies;

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

   

    private void Derrota()
    {
        timerIsRunning = false;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("¡DERROTA! El jugador ha muerto.");
        SceneManager.LoadScene(currentSceneIndex);
    }
}