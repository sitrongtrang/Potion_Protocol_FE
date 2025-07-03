using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    private IngredientConfig[] ingredients = new IngredientConfig[GameConstants.MaxSlot];
    public bool Add(IngredientConfig ingredient)
    {
        return false;
    }
}