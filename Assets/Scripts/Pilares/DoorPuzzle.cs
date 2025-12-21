using UnityEngine;

public class DoorPuzzle : MonoBehaviour
{
    private bool abierta = false;

    public void AbrirPuerta()
    {
        if (abierta) return;

        abierta = true;
        Debug.Log("PUERTA ABIERTA");

        gameObject.SetActive(false);
    }
}

