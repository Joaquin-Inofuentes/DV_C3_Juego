using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform objetivo;
    public Drakkar.GameUtils.DrakkarTrail trail;

    void Start()
    {
        // Activamos el trail al arrancar
        trail.enabled = true;
    }

    void Update()
    {
        if (objetivo != null)
        {
            // Movimiento oscilante
            float x = Mathf.Sin(Time.time * 5f) * 0.5f;
            objetivo.localPosition = new Vector3(x, 0, 0);
        }
    }
}
