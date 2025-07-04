using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientManagerTest : MonoBehaviour
{
    [SerializeField] private List<IngredientConfig> _ingredientTypes;
    [SerializeField] private PlayerController _player;

    private List<IngredientController> ingredients = new();
    private float _spawnCooldown = 5;

    void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }

    private void Spawn()
    {
        int ingredientIdx = Random.Range(0, _ingredientTypes.Count);
        IngredientPool.Instance.SpawnIngredient(_ingredientTypes[ingredientIdx], new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0));
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