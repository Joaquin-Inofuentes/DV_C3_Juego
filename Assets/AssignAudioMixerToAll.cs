using CustomInspector;
// Odin inspector
using UnityEngine;
using UnityEngine.Audio;

public class AssignAudioMixerToAll : MonoBehaviour
{
    
    public AudioMixer audioMixer;  // El AudioMixer que deseas asignar a todos los AudioSource

    void Start()
    {
        // Obtener todos los AudioSource en la escena
        /*AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            // Asignar el AudioMixer al AudioSource (sin especificar grupo)
            audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("VFX")[0];  // O cualquier grupo que prefieras
            audioSource.spatialBlend = 1f;
            audioSource.dopplerLevel = 0f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = 1f;
            audioSource.maxDistance = 30f;
        }*/
    }
}
