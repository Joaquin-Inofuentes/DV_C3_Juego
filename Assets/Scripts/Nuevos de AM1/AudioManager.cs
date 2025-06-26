using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static public AudioManager instance;

    public List<string> nombresDeClips = new List<string>();
    // TP2 - Bruno Barzola
    // Uso de diccionarios para almacenar los clips de audio
    public static Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    private bool sonidosEscaneados = false; // Nuevo flag

    public void Start()
    {
        EscanearSonidosDisponibles();
    }

    void Update()
    {
        if (!sonidosEscaneados)
        {
            EscanearSonidosDisponibles();
        }

        if (instance == null)
            instance = this;
    }

    // 1️⃣ Escanea y carga todos los clips de Resources/Audio/
    public void EscanearSonidosDisponibles()
    {
        if (sonidosEscaneados) return; // Si ya se ejecutó, no hace nada

        clips.Clear();
        nombresDeClips.Clear(); // Limpiar la lista antes de llenarla

        AudioClip[] cargados = Resources.LoadAll<AudioClip>("Audio");

        foreach (AudioClip clip in cargados)
{
    if (!clips.ContainsKey(clip.name))
    {
        clips.Add(clip.name, clip);
        nombresDeClips.Add(clip.name);
        clip.LoadAudioData(); // 🔊 ← esta línea carga el audio en RAM
    }
}

        sonidosEscaneados = true; // Marcar como ejecutado
    }

    public static AudioClip ObtenerAudioPorNombre(string nombre)
    {
        if (clips.TryGetValue(nombre, out AudioClip clip))
            return clip;

        Debug.LogWarning("Clip no encontrado: " + nombre);
        return null;
    }
}
