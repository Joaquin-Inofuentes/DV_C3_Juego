using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
  
    public static event Action<GameObject> OnEnemySpawned;
    public static event Action<GameObject> OnEnemyDied;
    private void OnEnable()
    {
        OnEnemySpawned?.Invoke(gameObject);
        Debug.Log($"[Controlador] {gameObject.name} (ID {GetInstanceID()}) ha nacido y se ha registrado en LevelManager.");
    }

    private void OnDisable()
    {
    }
    public void NotifyDeath()
    {
        OnEnemyDied?.Invoke(gameObject);
        Debug.Log($"[Controlador] {gameObject.name} (ID {GetInstanceID()}) ha notificado su muerte.");
    }
}