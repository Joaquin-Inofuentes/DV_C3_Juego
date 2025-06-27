using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Vocespersonaje : MonoBehaviour
{
    public AudioSource ColisionesRio;
    public AudioClip[] ClipsChoquesRio;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "riocolision")
        {
            if (!ColisionesRio.isPlaying)
            {
                var clip = ClipsChoquesRio[Random.Range(0, ClipsChoquesRio.Length)];
                ColisionesRio.PlayOneShot(clip);
            }
        }
    }
}
