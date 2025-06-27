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


    public GameObject Audio;
    public static GameObject CrearEfectoSonoro(Vector3 Posicion, string Nombre)
    {
        // Si el diccionario está vacío, escanear sonidos
        if (clips.Count == 0)
        {
            if (instance != null)
                instance.EscanearSonidosDisponibles();
        }

        // Obtener el clip de audio por nombre
        AudioClip clip = ObtenerAudioPorNombre(Nombre);
        if (clip == null)
        {
            // Mostrar los nombres posibles e indicar que se debe usar uno de ellos
            string disponibles = string.Join(", ", instance.nombresDeClips);
            Debug.LogWarning($"Clip no encontrado: {Nombre}. Nombres disponibles: {disponibles}");
            return null;
        }

        // Crear un GameObject vacío en la posición indicada
        GameObject parlante = new GameObject("EfectoSonoro_" + Nombre);
        parlante.transform.position = Posicion;

        // Agregar componente AudioSource
        AudioSource audioSource = parlante.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();

        // Por esta línea:
        Object.Destroy(parlante, clip.length);

        return parlante;
    }



    public static GameObject CrearEfectoSonoro(Vector3 posicion, AudioClip audioClip)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("AudioClip es nulo.");
            return null;
        }

        GameObject parlante = new GameObject("EfectoSonoro_" + audioClip.name);
        parlante.transform.position = posicion;

        AudioSource nuevoAudioSource = parlante.AddComponent<AudioSource>();
        nuevoAudioSource.clip = audioClip;
        nuevoAudioSource.loop = false;
        nuevoAudioSource.Play();

        Object.Destroy(parlante, audioClip.length);

        return parlante;
    }
}
