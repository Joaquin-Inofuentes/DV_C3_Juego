using UnityEngine;

public class ObjectInstantiator : MonoBehaviour
{
    [Tooltip("El Prefab del GameObject que será instanciado.")]
    public GameObject prefabToInstantiate;

    /// <summary>
    /// Crea una nueva instancia del 'prefabToInstantiate' en la posición global especificada.
    /// </summary>
    /// <param name="position">La posición Vector3 donde se creará el nuevo objeto.</param>
    /// <returns>El GameObject recién instanciado.</returns>
    public void CreateObjectAtPosition(Vector3 position)
    {
        if (prefabToInstantiate == null)
        {
            Debug.LogError("ERROR: No hay un Prefab asignado a 'Prefab To Instantiate' en el Inspector.");
        }

        // Instanciar el objeto en la posición dada, usando la rotación por defecto (Quaternion.identity)
        GameObject newObject = Instantiate(prefabToInstantiate, position, Quaternion.identity);

        Debug.Log($"Objeto '{prefabToInstantiate.name}' creado en la posición: {position}");
    }
}