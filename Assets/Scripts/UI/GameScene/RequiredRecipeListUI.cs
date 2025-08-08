using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RequiredRecipeListUI : MonoBehaviour
{
    [SerializeField] private GameObject _recipeUIPrefab;

    void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            RecipeGenerator.Instance.OnRequiredRecipeAdded += AddRecipe;
            RecipeGenerator.Instance.OnRequiredRecipeRemoved += RemoveRecipe;
        }
    }

    void OnDisable()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            RecipeGenerator.Instance.OnRequiredRecipeAdded -= AddRecipe;
            RecipeGenerator.Instance.OnRequiredRecipeRemoved -= RemoveRecipe;
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
}