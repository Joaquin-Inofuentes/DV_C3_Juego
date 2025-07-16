using UnityEngine;
using System.Collections.Generic;

public class Lobo : A1_A1_H1_MoustroDelAverno
{
    public static List<Lobo> Manada = new List<Lobo>();

    public bool esAlfa = false;

    [Header("Stats del Alfa")]
    public float multiplicadorVida = 2f;
    public float multiplicadorDanio = 2f;
    public float multiplicadorVelocidad = 1.5f;
    public Vector3 escalaAlfa = new Vector3(1.5f, 1.5f, 1.5f);

    [Header("Aura del Alfa")]
    public GameObject auraPrefab;
    private GameObject auraInstanciada;

    [Header("Aullido")]
    public AudioClip sonidoAullido;
    public string triggerAnimacionAullido = "Aullar";


    protected override void Start()
    {
        base.Start(); // ← Esto ejecuta el Start() del enemigo padre, que seguramente crea o posiciona la barra roja
        Manada.Add(this);
        Invoke(nameof(IntentarConvertirseEnAlfa), 1f);
    }

    void OnDestroy()
    {
        Manada.Remove(this);
    }

    void IntentarConvertirseEnAlfa()
    {
        if (Manada.Count >= 3 && !Manada.Exists(l => l.esAlfa))
        {
            Lobo elegido = Manada[Random.Range(0, Manada.Count)];
            elegido.ConvertirseEnAlfa();
        }
    }

    void ConvertirseEnAlfa()
    {
        esAlfa = true;

        // Aumentar stats
        Vida = Mathf.RoundToInt(Vida * multiplicadorVida);
        VidaMax = Mathf.RoundToInt(VidaMax * multiplicadorVida);
        DañoDeAtaque = Mathf.RoundToInt(DañoDeAtaque * multiplicadorDanio);
        Velocidad = Mathf.RoundToInt(Velocidad * multiplicadorVelocidad);
        if (agent != null)
        {
            agent.speed *= multiplicadorVelocidad;
        }

        // Cambiar tamaño del modelo
        transform.localScale = escalaAlfa;

        // Instanciar aura roja
        if (auraPrefab != null)
        {
            auraInstanciada = Instantiate(auraPrefab, transform);
        }

        // Todos los lobos aúllan
        foreach (Lobo l in Manada)
        {
            l.Aullar();
        }

        Debug.Log($"{name} se convirtió en LOBO ALFA");
    }
    public void Aullar()
    {
        if (anim != null)
        {
            anim.SetTrigger(triggerAnimacionAullido);
        }

        if (S_Caminar != null && sonidoAullido != null)
        {
            S_Caminar.clip = sonidoAullido;
            S_Caminar.Play();
        }
    }

}
