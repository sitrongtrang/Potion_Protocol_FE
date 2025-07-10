using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;

public class StationController : MonoBehaviour
{
    private StationConfig _config;
    private List<RecipeConfig> _recipes;
    private List<ItemConfig> _items;
    void OnEnable()
    {
        EventBus.CraftItem += StartCrafting;
    }

    public void Initialize(StationConfig config, List<RecipeConfig> recipes)
    {
        _config = config;
        _recipes = recipes;
        _items = new();
    }

    public void AddItem(ItemConfig config)
    {
        _items.Add(config);
    }

    public void StartCrafting()
    {

        int recipeIndex = FindMatchingRecipe();
        if (recipeIndex != -1) StartCoroutine(WaitForCraft(_recipes[recipeIndex]));
        else
        {
            for (int i = 0; i < _items.Count; i++)
            {
                Vector3 dropPosition = transform.position + (i + 1) * transform.forward;
                DropItem(_items[i], dropPosition);
            }
        }
    }

    public void DropItem(ItemConfig item, Vector3 dropPosition)
    {
        ItemPool.Instance.SpawnItem(item, dropPosition);
    }

    IEnumerator WaitForCraft(RecipeConfig recipe)
    {
        yield return new WaitForSeconds(recipe.CraftingTime);
        Vector3 dropPosition = transform.position + transform.forward;
        DropItem(recipe.Product, dropPosition);
    }

    private bool MatchRecipe(RecipeConfig recipe)
    {
        var stationSet = new HashSet<string>(_items.Select(i => i.Id));
        var recipeSet = new HashSet<string>(recipe.Inputs.Select(i => i.Id));
        return stationSet.SetEquals(recipeSet);
    }

    private int FindMatchingRecipe()
    {
        for (int i = 0; i < _recipes.Count; i++)
        {
            if (MatchRecipe(_recipes[i]))
            {
                return i;
            }
        }
        return -1;
    }
}