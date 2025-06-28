using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParcheRaroParaMagico6 : MonoBehaviour
{
    public Image image;

    void Update()
    {
        
        // 1. Si no está asignada, la busca
        if (image == null)
            image = GetComponent<Image>();

        // 2. Si el alpha es menor a 1, lo corrige
        if (image != null && image.color.a < 1f)
        {
        
            Color c = image.color;
            c.a = 1f;
            image.color = c;
        }
    }
}
