using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    static public AudioManager instance;

    public AudioMixer audioMixer;

    public Slider masterScrollbar;
    public Slider vfxScrollbar;
    public Slider ambienteScrollbar;

    void Iniciar()
    {
        // Sincronizar al iniciar
        UpdateScrollbarsFromMixer();
    }

    void actualizar2()
    {
        // Reflejar el estado del mixer en las scrollbars
        UpdateScrollbarsFromMixer();
    }

    void UpdateScrollbarsFromMixer()
    {


        if (audioMixer.GetFloat("VFX", out float vfxDb))
            vfxScrollbar.value = Mathf.Pow(10f, vfxDb / 20f);

        else if (audioMixer.GetFloat("Ambiente", out float ambienteDb))
            ambienteScrollbar.value = Mathf.Pow(10f, ambienteDb / 20f);
        else if (audioMixer.GetFloat("Master", out float masterDb))
            masterScrollbar.value = Mathf.Pow(10f, masterDb / 20f);
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
    }

    public void SetVFXVolume(float value)
    {
        audioMixer.SetFloat("VFX", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
    }

    public void SetAmbienteVolume(float value)
    {
        audioMixer.SetFloat("Ambiente", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
    }


    public List<string> nombresDeClips = new List<string>();
    // TP2 - Bruno Barzola
    // Uso de diccionarios para almacenar los clips de audio
    public static Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    private bool sonidosEscaneados = false; // Nuevo flag

    public void Start()
    {
        EscanearSonidosDisponibles();
        Iniciar();
    }

    void Update()
    {
        actualizar2();
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


    public AudioMixer AudioMixer;
    public static GameObject CrearEfectoSonoro(Vector3 posicion, AudioClip audioClip)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("AudioClip es nulo.");
            return null;
        }

        GameObject parlante = new GameObject("EfectoSonoro_" + audioClip.name);
        parlante.transform.position = posicion;

        AudioSource audioSource = parlante.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.loop = false;
        audioSource.Play();

        Object.Destroy(parlante, audioClip.length);
        AudioMixer audioMixer = instance.AudioMixer;
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("VFX")[0];
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 30f;

        return parlante;
    }


    public static GameObject CrearEfectoSonoro(Vector3 posicion, AudioClip audioClip, bool Es3D)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("AudioClip es nulo.");
            return null;
        }

        GameObject parlante = new GameObject("EfectoSonoro_" + audioClip.name);
        parlante.transform.position = posicion;

        AudioSource audioSource = parlante.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.loop = false;
        audioSource.Play();

        Object.Destroy(parlante, audioClip.length);
        AudioMixer audioMixer = instance.AudioMixer;
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("VFX")[0];
        audioSource.spatialBlend = Es3D ? 1 : 0;
        audioSource.dopplerLevel = 0f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 30f;

        return parlante;
    }

    public static GameObject CrearEfectoSonoro(Vector3 posicion, AudioClip audioClip, bool Es3D,float Volumen)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("AudioClip es nulo.");
            return null;
        }

        GameObject parlante = new GameObject("EfectoSonoro_" + audioClip.name);
        parlante.transform.position = posicion;

        AudioSource audioSource = parlante.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.loop = false;
        audioSource.Play();

        Object.Destroy(parlante, audioClip.length);
        AudioMixer audioMixer = instance.AudioMixer;
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("VFX")[0];
        audioSource.spatialBlend = Es3D ? 1 : 0;
        audioSource.dopplerLevel = 0f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 30f;
        audioSource.volume = Volumen; // Ajustar el volumen del AudioSource

        return parlante;
    }

    public static GameObject CrearEfectoSonoro(Vector3 posicion, AudioClip audioClip, float Volumen = 1)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("AudioClip es nulo.");
            return null;
        }

        GameObject parlante = new GameObject("EfectoSonoro_" + audioClip.name);
        parlante.transform.position = posicion;

        AudioSource audioSource = parlante.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.loop = false;
        audioSource.Play();

        Object.Destroy(parlante, audioClip.length);
        AudioMixer audioMixer = instance.AudioMixer;
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("VFX")[0];
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 30f;
        audioSource.volume = Volumen; // Ajustar el volumen del AudioSource

        return parlante;
    }


    public static GameObject CrearEfectoSonoro(Vector3 posicion, AudioClip audioClip, float Volumen = 1,float RadioDeImpacto = 50)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("AudioClip es nulo.");
            return null;
        }

        GameObject parlante = new GameObject("EfectoSonoro_" + audioClip.name);
        parlante.transform.position = posicion;

        AudioSource audioSource = parlante.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.loop = false;
        audioSource.Play();

        Object.Destroy(parlante, audioClip.length);
        AudioMixer audioMixer = instance.AudioMixer;
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("VFX")[0];
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = RadioDeImpacto;
        audioSource.volume = Volumen; // Ajustar el volumen del AudioSource

        return parlante;
    }
}
