using UnityEngine;

public class ElectricPillar : MonoBehaviour
{
    public int pillarID;
    public ElectricPuzzleManager puzzleManager;

    [Header("Visual")]
    public GameObject vfxActivacion;
    private GameObject vfxInstanciado;

    public Light luzPilar;
    public Renderer rend;
    public Material materialActivo;
    private Material materialOriginal;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoError;

    private bool activado = false;

    void Awake()
    {
        if (rend != null)
            materialOriginal = rend.material;

        if (luzPilar != null)
            luzPilar.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (activado) return;

        if (collision.gameObject.CompareTag("ElectricSpell"))
        {
            puzzleManager.TryActivatePillar(this);
            Destroy(collision.gameObject);
        }
    }

    public void ActivarCorrecto()
    {
        activado = true;

        // 🔥 VFX (SE GUARDA LA REFERENCIA)
        if (vfxActivacion != null && vfxInstanciado == null)
        {
            vfxInstanciado = Instantiate(
                vfxActivacion,
                transform.position,
                Quaternion.identity
            );
        }

        if (luzPilar != null)
            luzPilar.enabled = true;

        if (rend != null && materialActivo != null)
            rend.material = materialActivo;

        if (audioSource != null && sonidoCorrecto != null)
            audioSource.PlayOneShot(sonidoCorrecto);
    }

    public void Resetear()
    {
        activado = false;

        // 🔥 ACA SE APAGA EL EFECTO
        if (vfxInstanciado != null)
        {
            Destroy(vfxInstanciado);
            vfxInstanciado = null;
        }

        if (luzPilar != null)
            luzPilar.enabled = false;

        if (rend != null && materialOriginal != null)
            rend.material = materialOriginal;
    }

    public void Error()
    {
        if (audioSource != null && sonidoError != null)
            audioSource.PlayOneShot(sonidoError);
    }
}




