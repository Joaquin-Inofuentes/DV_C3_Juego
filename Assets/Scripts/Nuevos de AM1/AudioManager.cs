using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static public AudioManager instance;

 
    // En Resources/Audio/
    // Carpeta: Assets/Resources/Audio/

    public void Start()
    {
        EscanearSonidosDisponibles();
    }

    void Update()
    {
        if(clips.Count == 0)
            EscanearSonidosDisponibles();
        
        if(clips.Count != nombresDeClips.Count)
        {
            EscanearSonidosDisponibles();
        }

        if (instance == null)
            instance = this;
    }
    public List<string> nombresDeClips = new List<string>();

    // 1️⃣ Escanea y carga todos los clips de Resources/Audio/
    public static Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    public void EscanearSonidosDisponibles()
    {
        clips.Clear();
        AudioClip[] cargados = Resources.LoadAll<AudioClip>("Audio");

        foreach (AudioClip clip in cargados)
        {
            if (!clips.ContainsKey(clip.name))
            {
                clips.Add(clip.name, clip);
                nombresDeClips.Add(clip.name); // ✅ guardás el nombre
            }
        }
    }

    public static AudioClip ObtenerAudioPorNombre(string nombre)
    {
        if (clips.TryGetValue(nombre, out AudioClip clip))
            return clip;

        Debug.LogWarning("Clip no encontrado: " + nombre);
        return null;
    }

}
