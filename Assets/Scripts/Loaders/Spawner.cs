using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private ScriptableObject _config;

    void Awake()
    {
        if (_config is not ISpawnConfig config)
        {
            Debug.LogError("Assigned spawn data does not implement ISpawnData.");
            return;
        }

        GameObject instance = Instantiate(config.Prefab, transform.position, Quaternion.identity);
        if (instance.TryGetComponent<ISpawnable>(out var spawnable))
        {
            spawnable.Initialize(config);
        }
        else
        {
            Debug.LogWarning("Spawned object doesn't implement ISpawnable");
        }
    
    //     var type = config.GetType();
    //     var spawnableType = typeof(ISpawnable<>).MakeGenericType(type);
    //     var component = instance.GetComponent(spawnableType);

    //     if (component != null)
    //     {
    //         spawnableType.GetMethod("Initialize").Invoke(component, new object[] { config });
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Spawned object does not implement ISpawnable<" + type.Name + ">");
    //     }
    }
}