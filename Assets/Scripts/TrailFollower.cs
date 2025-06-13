using UnityEngine;

public class TrailFollower : MonoBehaviour
{
    public Transform objetivo; // El objeto a seguir (la espada)

    void LateUpdate()
    {
        if (objetivo == null) return;
        transform.position = objetivo.position;
        transform.rotation = objetivo.rotation;
    }
}
