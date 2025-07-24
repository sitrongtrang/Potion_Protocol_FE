using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Scriptable Objects/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private GameObject _mapPrefab;
    public GameObject MapPrefab => _mapPrefab;

    [SerializeField] private List<EnemyController> _enemies;
    public List<EnemyController> Enemies => _enemies;

    [SerializeField] private float _recipeAddInterval;
    public float RecipeAddInterval => _recipeAddInterval;

    [Serializable]
    public class ItemSourceSetting
    {
        public ItemSourceController Prefab;
        public int MaxCapacity;
    }
    [SerializeField] private List<ItemSourceSetting> _itemSources;
    public List<ItemSourceSetting> ItemSources => _itemSources;

    [SerializeField] private List<RecipeConfig> _ingotRecipes;
    public List<RecipeConfig> IngotRecipes => _ingotRecipes;

    [SerializeField] private List<RecipeConfig> _finalRecipes; // Recipes for final products to submit
    public List<RecipeConfig> FinalRecipes => _finalRecipes;

    [SerializeField] private float _levelTime;
    public float LevelTime => _levelTime;

    [SerializeField] private int[] _scoreThresholds; // Thresholds for evaluating 1, 2, or 3 stars
    public int[] ScoreThresholds => _scoreThresholds;
}