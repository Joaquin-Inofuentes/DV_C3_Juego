using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemigosActivos : MonoBehaviour
{
    // PATRÓN SINGLETON: Permite que otros scripts accedan a él fácilmente.
    public static enemigosActivos Instance;

    // La lista de enemigos activos (la "suscripción" que quieres que use el enemigo)
    public List<GameObject> activeEnemies = new List<GameObject>();

    private void Awake()
    {
        // Aseguramos que solo haya una instancia de este Manager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// El enemigo llama a este método para AGREGARSE (SUSCRIBIRSE) a la lista.
    /// </summary>
    /// <param name="enemyObject">El enemigo que se está registrando.</param>
    public void RegisterEnemy(GameObject enemyObject)
    {
        if (enemyObject != null && !activeEnemies.Contains(enemyObject))
        {
            activeEnemies.Add(enemyObject);
            Debug.Log($"[Manager] Enemigo registrado: {enemyObject.name}. Total: {activeEnemies.Count}");
        }
    }

    /// <summary>
    /// El enemigo llama a este método para ELIMINARSE (DESUSCRIBIRSE) de la lista.
    /// </summary>
    /// <param name="enemyObject">El enemigo que se está dando de baja.</param>
    public void DeregisterEnemy(GameObject enemyObject)
    {
        if (enemyObject != null && activeEnemies.Contains(enemyObject))
        {
            activeEnemies.Remove(enemyObject);
            Debug.Log($"[Manager] Enemigo dado de baja: {enemyObject.name}. Restantes: {activeEnemies.Count}");

            // Llama a la comprobación de victoria inmediatamente.
            CheckWinCondition();
        }
    }

    /// <summary>
    /// La condición de victoria que tu script de cronómetro podría necesitar.
    /// </summary>
    public void CheckWinCondition()
    {
        if (activeEnemies.Count == 0)
        {
            Debug.Log("¡Nivel superado! La lista de enemigos está vacía.");

            // **AQUÍ VA LA LÓGICA DE NOTIFICACIÓN AL CRONÓMETRO**
            // Si tienes un script de Cronómetro, este es el lugar para notificarle que se ha ganado.
            // Por ejemplo: TimerScript.Instance.TriggerVictory();
        }
    }

    /// <summary>
    /// Utilidad para que el cronómetro verifique si la lista está vacía.
    /// </summary>
    public bool AreAnyEnemiesAlive()
    {
        return activeEnemies.Count > 0;
    }
}
