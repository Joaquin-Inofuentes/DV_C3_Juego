using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    public RawImage rawImage;
    public float fadeDuration = 2f;

    private void OnEnable()
    {
        // Aseguramos color blanco opaco antes de empezar el fade
        rawImage.color = new Color(1f, 1f, 1f, 1f);
        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);

            // Debug: Mostramos el valor actual de alpha
            Debug.Log("Alpha: " + alpha);

            rawImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        rawImage.color = new Color(1f, 1f, 1f, 0f);
        gameObject.SetActive(false);
    }
}
