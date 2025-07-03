using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    private IngredientConfig[] ingredients = new IngredientConfig[GameConstants.MaxSlot];

    public IngredientConfig Get(int idx) => ingredients[idx];

    public PlayerInventory()
    {
        return;
    }

    public bool Add(IngredientConfig ingredient)
    {
        int idx = FindSlot();
        if (idx == -1)
        {
            Debug.Log("Inventory is full");
            return false;
        }
        ingredients[idx] = ingredient;
        Debug.Log("Picked up item " + ingredient.Name + " to slot " + (idx+1).ToString());
        return true;
    }

    public bool Remove(int idx)
    {
        if (ingredients[idx] == null) return false;
        ingredients[idx] = null;
        return true;
    }

    private int FindSlot()
    {
        for (int i = 0; i < GameConstants.MaxSlot; i++)
        {
            if (ingredients[i] == null) return i;
        }

        return -1;
    }


}