using CustomInspector;
// Odin inspector
using UnityEngine;
using UnityEngine.Audio;

public class AssignAudioMixerToAll : MonoBehaviour
{
    [Button(nameof(AsignarConfiguracionesALosAudios))]
    public AudioMixer audioMixer;  // El AudioMixer que deseas asignar a todos los AudioSource

    void AsignarConfiguracionesALosAudios()
    {
        // Obtener todos los AudioSource en la escena
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            // Asignar el AudioMixer al AudioSource (sin especificar grupo)
            audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("VFX")[0];  // O cualquier grupo que prefieras
            audioSource.spatialBlend = 1f;
            audioSource.dopplerLevel = 0f;
            audioSource.rolloffMode = AudioRolloffMode.Custom; // Custom rolloff
            audioSource.minDistance = 0f;                      // Valor común por defecto (se ignora con Custom, pero necesario igual)
            audioSource.maxDistance = 30f;                   // Max distance
            audioSource.spread = 0f;                           // Spread (0 = mono, 360 = estéreo total)
            audioSource.pitch = 1f;                            // Pitch
        }
    }
}
