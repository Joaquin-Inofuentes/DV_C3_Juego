using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void EmpezarPartida()
    {
        PlayerPrefs.SetString("RetryScene", "EscenaPrincipal v3");
        SceneManager.LoadScene("EscenaPrincipal v3");
    }

    public void ModoSupervivencia()
    {
        SceneManager.LoadScene("Supervivencia");
    }
    public void Reintentar()
    {
        string escena = PlayerPrefs.GetString("RetryScene", "EscenaPrincipal v3");
        SceneManager.LoadScene(escena);
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void Creditos()
    {
        SceneManager.LoadScene("Creditos");
    }
    public void LoadVictoryScene()
    {
        SceneManager.LoadScene("Victoria");
    }

    public void LoadDefeatScene()
    {
        SceneManager.LoadScene("Derrota");
    }
    public void Salir()
    {
        Application.Quit(); 
    }
}
