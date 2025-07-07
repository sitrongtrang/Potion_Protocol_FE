using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour
{
    private List<CraftConfig> _recipeList;
    private List<IngredientConfig> _ingredients;

    public void GetRecipe()
    {

        CraftConfig recipe = FindMatchingRecipe();
        if (recipe != null) StartCoroutine(WaitForCraft(recipe));
    }

    IEnumerator WaitForCraft(CraftConfig recipe)
    {
        yield return new WaitForSeconds(recipe.TimeCrafting);
        Instantiate(recipe.Item, transform.position, Quaternion.identity);
    }

    public bool MatchRecipeWithCounts(CraftConfig recipe)
    {
        if (_ingredients.Count != recipe.Ingredients.Count)
            return false;

        var stationDict = new Dictionary<string, int>();
        var recipeDict = new Dictionary<string, int>();

        foreach (var ing in _ingredients)
        {
            if (stationDict.ContainsKey(ing.Id))
                stationDict[ing.Id]++;
            else
                stationDict[ing.Id] = 1;
        }

        foreach (var ing in recipe.Ingredients)
        {
            if (recipeDict.ContainsKey(ing.Id))
                recipeDict[ing.Id]++;
            else
                recipeDict[ing.Id] = 1;
        }

        if (stationDict.Count != recipeDict.Count)
            return false;

        foreach (var kvp in stationDict)
        {
            if (!recipeDict.TryGetValue(kvp.Key, out int count) || count != kvp.Value)
                return false;
        }

        return true;
    }

    public CraftConfig FindMatchingRecipe()
    {
        for (int i = 0; i < _recipeList.Count; i++)
        {
            if (MatchRecipeWithCounts(_recipeList[i]))
            {
                return _recipeList[i];
            }
        }
        return null;
    }


    public bool RequireIngredient(IngredientConfig config)
    {
        return true;
    }
}