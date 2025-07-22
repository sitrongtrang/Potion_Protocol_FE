using System.Collections.Generic;
using UnityEngine;

public class RequiredRecipeListUI : MonoBehaviour
{
    [SerializeField] private GameObject _recipeUIPrefab;

    void OnEnable()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnRequiredRecipeAdded += AddRecipe;
            LevelManager.Instance.OnRequiredRecipeRemoved += RemoveRecipe;
        }
        
    }

    void OnDisable()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnRequiredRecipeAdded -= AddRecipe;
            LevelManager.Instance.OnRequiredRecipeRemoved -= RemoveRecipe;
        }
    }

    public void Initialize(List<RecipeConfig> recipes)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < recipes.Count; i++)
        {
            if (recipes[i] == null) continue;
            AddRecipe(recipes[i]);
        }

        LevelManager.Instance.OnRequiredRecipeAdded += AddRecipe;
        LevelManager.Instance.OnRequiredRecipeRemoved += RemoveRecipe;
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
}