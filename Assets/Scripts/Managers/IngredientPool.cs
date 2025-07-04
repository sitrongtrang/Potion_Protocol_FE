using System.Collections.Generic;
using UnityEngine;

public class IngredientPool : MonoBehaviour
{
    private Queue<IngredientController> _activeIngredients = new Queue<IngredientController>();
    private Dictionary<string, Queue<IngredientController>> _pooledObjects = new Dictionary<string, Queue<IngredientController>>();
    public static IngredientPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public IngredientController SpawnIngredient(IngredientConfig config, Vector3 position)
    {
        // Exceed cap
        if (_activeIngredients.Count >= GameConstants.MaxIngredients)
        {
            IngredientController oldest = _activeIngredients.Dequeue();
            ReturnToPool(oldest);
        }

        // Get from pool or instantiate
        IngredientController ingredient = GetFromPool(config);
        ingredient.transform.position = position;
        ingredient.transform.rotation = Quaternion.identity;
        ingredient.gameObject.SetActive(true);
        ingredient.Initialize(config);

        _activeIngredients.Enqueue(ingredient);
        return ingredient;
    }

    public void RemoveIngredient(IngredientController ingredient)
    {
        // Remove from active queue
        Queue<IngredientController> newQueue = new Queue<IngredientController>();
        foreach (var ing in _activeIngredients)
        {
            if (ing != ingredient)
                newQueue.Enqueue(ing);
        }
        _activeIngredients = newQueue;

        ReturnToPool(ingredient);
    }

    private void ReturnToPool(IngredientController ingredient)
    {
        ingredient.gameObject.SetActive(false);

        // Return ingredient to the appropriate pool
        string name = ingredient.Config.Name;
        if (!_pooledObjects.ContainsKey(name))
            _pooledObjects[name] = new Queue<IngredientController>();

        _pooledObjects[name].Enqueue(ingredient);
    }

    private IngredientController GetFromPool(IngredientConfig config)
    {
        string name = config.Name;

        // Retrieve an ingredient from pool or instantiate a new one
        if (_pooledObjects.ContainsKey(name) && _pooledObjects[name].Count > 0)
        {
            return _pooledObjects[name].Dequeue();
        }

        // No available object in pool, instantiate a new one
        IngredientController newObj = Instantiate(config.Prefab);
        return newObj;
    }
}
