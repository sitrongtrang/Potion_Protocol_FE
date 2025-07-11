using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManagerTest : MonoBehaviour
{
    [SerializeField] private List<ItemConfig> _itemTypes;
    private float _spawnCooldown = 5;

    void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }

    private void Spawn()
    {
        int itemIdx = Random.Range(0, _itemTypes.Count);
        ItemPool.Instance.SpawnItem(_itemTypes[itemIdx], new Vector2(Random.Range(-2, 2), Random.Range(-2, 2)));
    }
    
    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnCooldown);
            Spawn();
        }
    }
}