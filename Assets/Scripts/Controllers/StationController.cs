using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StationController : MonoBehaviour
{
    private List<RecipeConfig> _recipeList;
    private List<ItemConfig> _items;

    public void GetRecipe()
    {

        int recipeIndex = FindMatchingRecipe();
        if (recipeIndex != -1) StartCoroutine(WaitForCraft(_recipeList[recipeIndex]));
    }

    IEnumerator WaitForCraft(RecipeConfig recipe)
    {
        yield return new WaitForSeconds(recipe.TimeCrafting);
        Instantiate(recipe.Item, transform.position, Quaternion.identity);
    }

    public bool MatchRecipe(RecipeConfig recipe)
    {
        var stationSet = new HashSet<string>(_items.Select(i => i.Id));
        var recipeSet = new HashSet<string>(recipe.Items.Select(i => i.Id));
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

    public void GetItem(ItemConfig config)
    {
        _items.Add(config);
    }
}