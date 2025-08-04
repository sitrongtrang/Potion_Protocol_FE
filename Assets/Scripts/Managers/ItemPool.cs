using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    [SerializeField] private ItemController _itemPrefab;
    private Queue<ItemController> _activeItems = new Queue<ItemController>();
    public ItemController[] ActiveItems => _activeItems.ToArray();
    private Queue<ItemController> _pooledObjects = new Queue<ItemController>();
    public static ItemPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public ItemController SpawnItem(ItemConfig config, Vector2 position)
    {
        // Exceed cap
        if (_activeItems.Count >= GameConstants.MaxItems)
        {
            ItemController oldest = _activeItems.Dequeue();
            ReturnToPool(oldest);
        }

        // Get from pool or instantiate
        ItemController item = GetFromPool(config);
        item.transform.position = position;
        item.transform.rotation = Quaternion.identity;
        item.Initialize(config);
        item.gameObject.SetActive(true);

        _activeItems.Enqueue(item);
        return item;
    }

    public void RemoveItem(ItemController item)
    {
        // Remove from active queue
        Queue<ItemController> newQueue = new Queue<ItemController>();
        foreach (var ing in _activeItems)
        {
            if (ing != item)
                newQueue.Enqueue(ing);
        }
        _activeItems = newQueue;

        ReturnToPool(item);
    }

    private void ReturnToPool(ItemController item)
    {
        item.gameObject.SetActive(false);
        _pooledObjects.Enqueue(item);
    }

    private ItemController GetFromPool(ItemConfig config)
    {
        // Retrieve an item from pool
        if (_pooledObjects.Count > 0)
        {
            return _pooledObjects.Dequeue();
        }

        // No available object in pool, instantiate a new one
        ItemController newObj = Instantiate(_itemPrefab);
        return newObj;
    }
}
