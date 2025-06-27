using UnityEngine;
using TMPro;

public class UIFade : MonoBehaviour
{
    public GameObject textObject;
    public float fadeDuration = 2f;

    private TextMeshProUGUI text;

    private void OnEnable()
    {
        if (text == null)
            text = textObject.GetComponent<TextMeshProUGUI>();

        if (text == null)
        {
            Debug.LogError("No se encontró el componente TextMeshProUGUI.");
            return;
        }

        text.color = new Color(1f, 1f, 1f, 1f);
        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color originalColor = text.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        gameObject.SetActive(false);
    }
}


