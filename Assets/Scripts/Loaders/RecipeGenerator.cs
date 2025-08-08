using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeGenerator : MonoBehaviour
{
    public static RecipeGenerator Instance { get; private set; }
    [SerializeField] private RequiredRecipeListUI _requiredRecipeListUI;
    private float _generateInterval;
    private List<RecipeConfig> _finalRecipes = new();
    private List<RecipeInstance> _requiredRecipes = new();
    private Dictionary<RecipeInstance, Coroutine> _recipeExpirations = new();

    public List<RecipeInstance> RequiredRecipes => _requiredRecipes;
    public event Action<RecipeConfig> OnRequiredRecipeAdded;
    public event Action<int> OnRequiredRecipeRemoved;

    public class RecipeInstance
    {
        public RecipeConfig Config;
        public RecipeInstance(RecipeConfig recipe)
        {
            Config = recipe;
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Initialize(LevelConfig levelConfig)
    {
        _generateInterval = levelConfig.RecipeAddInterval;
        _finalRecipes = levelConfig.FinalRecipes;

        _requiredRecipes.Clear();
        _requiredRecipeListUI.Initialize();
        StartCoroutine(AddNewRequiredRecipe());
    }

    private RecipeConfig GetNewRequiredRecipe()
    {
        int idx = _finalRecipes.Count > 0 ? UnityEngine.Random.Range(0, _finalRecipes.Count) : -1;
        if (idx == -1) return null;
        return _finalRecipes[idx];
    }

    private IEnumerator AddNewRequiredRecipe()
    {
        while (true)
        {
            RecipeConfig newRecipe = GetNewRequiredRecipe();
            if (newRecipe != null)
            {
                var instance = new RecipeInstance(newRecipe);
                _requiredRecipes.Add(instance);
                OnRequiredRecipeAdded?.Invoke(instance.Config);
                Coroutine coroutine = StartCoroutine(RecipeExpire(instance));
                _recipeExpirations[instance] = coroutine;
                yield return new WaitForSeconds(_generateInterval);
                yield return new WaitUntil(() => _requiredRecipes.Count < GameConstants.MaxRequiredRecipes);
            }
        }
    }

    public void RemoveRecipe(RecipeInstance instance)
    {
        int index = _requiredRecipes.IndexOf(instance);
        if (index == -1) return;

        _requiredRecipes.RemoveAt(index);

        if (_recipeExpirations.TryGetValue(instance, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
            _recipeExpirations.Remove(instance);
        }

        OnRequiredRecipeRemoved?.Invoke(index);
    }

    private IEnumerator RecipeExpire(RecipeInstance recipe)
    {
        yield return new WaitForSeconds(recipe.Config.ExpireTime);
        RemoveRecipe(recipe);
    }
}