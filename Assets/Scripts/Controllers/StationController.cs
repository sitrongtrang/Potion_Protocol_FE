using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StationController : MonoBehaviour
{
    private List<RecipeConfig> _recipeList;
    private List<ItemConfig> _items;
    void OnEnable()
    {
        EventBus.CraftItem += StartCrafting;
    }

    public void AddItem(ItemConfig config)
    {
        _items.Add(config);
    }

    public void StartCrafting()
    {

        int recipeIndex = FindMatchingRecipe();
        if (recipeIndex != -1) StartCoroutine(WaitForCraft(_recipeList[recipeIndex]));
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
        yield return new WaitForSeconds(recipe.TimeCrafting);
        Vector3 dropPosition = transform.position + transform.forward;
        DropItem(recipe.Item, dropPosition);
    }

    private bool MatchRecipe(RecipeConfig recipe)
    {
        var stationSet = new HashSet<string>(_items.Select(i => i.Id));
        var recipeSet = new HashSet<string>(recipe.Items.Select(i => i.Id));
        return stationSet.SetEquals(recipeSet);
    }

    private int FindMatchingRecipe()
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
}