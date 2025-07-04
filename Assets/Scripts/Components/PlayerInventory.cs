using UnityEngine;

public class PlayerInventory
{
    private PlayerController _player;
    private IngredientConfig[] ingredients = new IngredientConfig[GameConstants.MaxSlot];
    private int _choosingSlot = 0;

    public IngredientConfig Get(int idx) => ingredients[idx];

    public void Initialize(PlayerController player)
    {
        _player = player;
    }

    public void Pickup(IngredientController ingredient)
    {
        bool isAdded = Add(ingredient.Config);
        if (isAdded)
        {
            IngredientPool.Instance.RemoveIngredient(ingredient);
        }
    }

    public void Drop()
    {
        if (_choosingSlot == -1 || ingredients[_choosingSlot] == null)
        {
            // Not choosing any ingredient
            Debug.Log("No ingredient to drop");
            return;
        }
        else
        {
            // Drop the ingredient into the world at player's position
            IngredientConfig configToDrop = ingredients[_choosingSlot];
            Vector3 dropPosition = _player.transform.position + _player.transform.forward; 

            IngredientPool.Instance.SpawnIngredient(configToDrop, dropPosition);

            // Remove from inventory
            ingredients[_choosingSlot] = null;
        }
    }

    public void TransferToStation(StationController station)
    {
        if (_choosingSlot == -1 || ingredients[_choosingSlot] == null)
        {
            // Not choosing any ingredient
            Debug.Log("No ingredient in slot to transfer");
        }
        else
        {
            // Transfer the ingredient to the station if the station requires
            if (station.RequireIngredient(ingredients[_choosingSlot]))
            {
                Debug.Log("Transferred item " + ingredients[_choosingSlot].Name + " in slot " + (_choosingSlot + 1).ToString() + " to station");
                // TODO: add ingredient to the station
                Remove(_choosingSlot);
            }
            else
            {
                Debug.Log("This station does not require this ingredient");
            }
        }
    }

    private bool Add(IngredientConfig ingredient)
    {
        int idx;
        // If choosing slot is empty, put the ingredient into that slot; else put in the first slot that is empty
        if (ingredients[_choosingSlot] == null) idx = _choosingSlot;
        else idx = FindSlot();

        // No empty slot, cannot add ingredient
        if (idx == -1)
        {
            Debug.Log("Inventory is full");
            return false;
        }

        // Found an empty slot, put ingredient into that slot
        _choosingSlot = idx;
        ingredients[idx] = ingredient;
        Debug.Log("Picked up item " + ingredient.Name + " to slot " + (idx + 1).ToString());
        return true;
    }

    private bool Remove(int idx)
    {
        // If the slot has ingredient in it, remove the ingredient
        if (ingredients[idx] == null) return false;
        ingredients[idx] = null;
        return true;
    }

    private int FindSlot()
    {
        // Find the first empty slot
        for (int i = 0; i < GameConstants.MaxSlot; i++)
        {
            if (ingredients[i] == null) return i;
        }

        return -1;
    }


}