using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogos : MonoBehaviour
{
    [Header("Datos del diálogo")]
    public List<string> nombreClips;           // Nombre del clip por línea
    public List<string> textos;                // Texto del subtítulo por línea
    public List<float> tiempos;                // Tiempo de duración por línea

    [Header("UI")]
    public TextMeshProUGUI textoUI;
    public GameObject burbuja;

    private Coroutine coroutineDialogo;
    private int indiceActual = -1;

    public void ReproducirDialogo(string nombreClip)
    {
        if (coroutineDialogo != null)
            StopCoroutine(coroutineDialogo);

        coroutineDialogo = StartCoroutine(EjecutarDialogo(nombreClip));
    }

    private IEnumerator EjecutarDialogo(string nombreClip)
    {
        // Buscar el primer índice que coincide con ese clip
        indiceActual = nombreClips.FindIndex(x => x == nombreClip);

        if (indiceActual == -1)
        {
            Debug.LogWarning("No se encontró el clip: " + nombreClip);
            yield break;
        }

        burbuja.SetActive(true);

        while (indiceActual < nombreClips.Count && nombreClips[indiceActual] == nombreClip)
        {
            textoUI.text = textos[indiceActual];
            yield return new WaitForSeconds(tiempos[indiceActual]);
            indiceActual++;
        }

        textoUI.text = "";
        burbuja.SetActive(false);
        indiceActual = -1;
    }
}
