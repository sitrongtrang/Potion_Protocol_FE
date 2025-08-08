using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RequiredRecipeListUI : MonoBehaviour
{
    [SerializeField] private GameObject _recipeUIPrefab;
    private GameStateHandler _gameStateHandler;

    void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            RecipeGenerator.Instance.OnRequiredRecipeAdded += AddRecipe;
            RecipeGenerator.Instance.OnRequiredRecipeRemoved += RemoveRecipe;
        } 
        else if (SceneManager.GetActiveScene().name == "OnlineGameScene")
        {
            _gameStateHandler = FindFirstObjectByType<GameStateHandler>();
            if (_gameStateHandler != null) 
            {
                _gameStateHandler.OnRecipesSynced += SyncRecipe; 
            }
        }
    }

    void OnDisable()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            RecipeGenerator.Instance.OnRequiredRecipeAdded -= AddRecipe;
            RecipeGenerator.Instance.OnRequiredRecipeRemoved -= RemoveRecipe;
        }
        else if (SceneManager.GetActiveScene().name == "OnlineGameScene")
        {
            if (_gameStateHandler != null)
            {
                _gameStateHandler.OnRecipesSynced -= SyncRecipe;
            }
        }
    }

    public void Initialize()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddRecipe(RecipeConfig recipe)
    {
        GameObject recipeUIObj = Instantiate(_recipeUIPrefab, transform);
        RecipeUI recipeUI = recipeUIObj.GetComponent<RecipeUI>();
        recipeUI.Initialize(recipe);
        recipeUIObj.transform.SetParent(transform);
    }

    public void RemoveRecipe(int idx)
    {
        Destroy(transform.GetChild(idx).gameObject);
    }

    public void SyncRecipe(List<RecipeConfig> recipes)
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            if (i >= _recipeUIPrefab.transform.childCount)
            {
                AddRecipe(recipes[i]);
            } 
            else
            {
                Transform recipeUIObj = _recipeUIPrefab.transform.GetChild(i);
                RecipeUI recipeUI = recipeUIObj.GetComponent<RecipeUI>();
                recipeUI.Initialize(recipes[i]);
            }
        }
    }
}