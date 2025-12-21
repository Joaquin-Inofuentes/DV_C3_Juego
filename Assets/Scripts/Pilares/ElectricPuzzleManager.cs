using UnityEngine;

public class ElectricPuzzleManager : MonoBehaviour
{
    [Header("Configuración del patrón")]
    [Tooltip("Orden correcto de activación (IDs de pilares)")]
    public int[] patron;

    private int indiceActual = 0;
    private bool puzzleResuelto = false;

    public DoorPuzzle puerta;

    public void TryActivatePillar(ElectricPillar pillar)
    {
        if (puzzleResuelto) return;

        if (pillar.pillarID == patron[indiceActual])
        {
            pillar.ActivarCorrecto();
            indiceActual++;
            Debug.Log("Pilar activado");

            if (indiceActual >= patron.Length)
            {
                puzzleResuelto = true;
                puerta.AbrirPuerta();
            }
        }
        else
        {
            ResetPuzzle();
        }
    }

    private void ResetPuzzle()
{
    indiceActual = 0;

    ElectricPillar[] pilares = FindObjectsOfType<ElectricPillar>();
    foreach (var p in pilares)
    {
        p.Resetear();
        p.Error();
    }
}

}


