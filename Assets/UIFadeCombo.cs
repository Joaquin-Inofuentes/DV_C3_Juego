using UnityEngine;
using TMPro;

public class UIFadeCombo : MonoBehaviour
{
    public TextMeshProUGUI textoCombo;
    public float fadeDuration = 2f;

    public void MostrarTexto(string mensaje, Color color)
    {
        textoCombo.text = mensaje;
        textoCombo.color = new Color(color.r, color.g, color.b, 1f); // color personalizado con alfa 1

        StopAllCoroutines();
        gameObject.SetActive(true);
        StartCoroutine(FadeOut());
    }


    private System.Collections.IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color originalColor = textoCombo.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            textoCombo.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        textoCombo.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        gameObject.SetActive(false);
    }
}


