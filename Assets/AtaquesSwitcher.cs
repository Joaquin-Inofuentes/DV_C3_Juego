using UnityEngine;
using UnityEngine.UI;

public class AttackIconSwitcher : MonoBehaviour
{
    public Image[] IconosAtaque; 

    public Sprite[] IconosFisicos; 
    public Sprite[] IconosMagicos;    

    private bool isMagicMode = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isMagicMode = !isMagicMode;
            UpdateIcons();
        }
    }

    void UpdateIcons()
    {
        Sprite[] iconsToUse = isMagicMode ? IconosMagicos : IconosFisicos;

        for (int i = 0; i < IconosAtaque.Length; i++)
        {
            if (i < iconsToUse.Length)
                IconosAtaque[i].sprite = iconsToUse[i];
        }
    }
}

