using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StationController : MonoBehaviour
{
    private List<CraftConfig> _recipeList;
    private List<IngredientConfig> _ingredients;

    public void GetRecipe()
    {

        int recipeIndex = FindMatchingRecipe();
        if (recipeIndex != -1) StartCoroutine(WaitForCraft(_recipeList[recipeIndex]));
    }

    IEnumerator WaitForCraft(CraftConfig recipe)
    {
        yield return new WaitForSeconds(recipe.TimeCrafting);
        Instantiate(recipe.Item, transform.position, Quaternion.identity);
    }

    public bool MatchRecipe(CraftConfig recipe)
    {
        var stationSet = new HashSet<string>(_ingredients.Select(i => i.Id));
        var recipeSet = new HashSet<string>(recipe.Ingredients.Select(i => i.Id));
        return stationSet.SetEquals(recipeSet);
    }

    public int FindMatchingRecipe()
    {
        for (int i = 0; i < _recipeList.Count; i++)
        {
            if (MatchRecipe(_recipeList[i]))
            {
                return i;
            }
        }
        return -1;
    }

    public void GetIngredient(IngredientConfig config)
    {
        _ingredients.Add(config);
    }

    public bool RequireIngredient(IngredientConfig config)
    {
        return true;
    }
}