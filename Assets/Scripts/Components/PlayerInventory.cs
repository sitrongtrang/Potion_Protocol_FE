using System;
using UnityEngine;

public class PlayerInventory : IComponent
{
    private PlayerController _player;
    private ItemConfig[] items = new ItemConfig[GameConstants.MaxSlot];
    private int _choosingSlot = 0;
    public int ChoosingSlot
    {
        get => _choosingSlot;
        set
        {
            int oldSlot = _choosingSlot;
            _choosingSlot = value;
            if (oldSlot != value)
            {
                OnChoosingSlotChanged?.Invoke(oldSlot, value); // Highlight the new slot in UI
            } 
        }
    }
    private bool _isAutoFocus;
    public event Action<int, int> OnChoosingSlotChanged;
    public event Action<int, Sprite> OnSlotUpdated;
    public ItemConfig Get(int idx) => items[idx];

    public void Initialize(PlayerController player, PlayerInputManager inputManager)
    {
        ChoosingSlot = 0;
        _player = player;
        _isAutoFocus = true;
    }

    public ItemConfig Pickup(ItemController item)
    {
        bool isAdded = Add(item.Config);
        if (isAdded)
        {
            ItemPool.Instance.RemoveItem(item);
        }
        return isAdded ? item.Config : null;
    }

    public ItemConfig Drop()
    {
        if (_choosingSlot == -1 || items[_choosingSlot] == null)
        {
            // Not choosing any item
            return null;
        }
        else
        {
            // Drop the item into the world at player's position
            ItemConfig itemToDrop = items[_choosingSlot];
            Vector2 playerPos = _player.transform.position;
            Vector2 dropPosition = playerPos + 0.5f * Vector2.down;

            ItemPool.Instance.SpawnItem(itemToDrop, dropPosition);

            // Remove from inventory
            items[_choosingSlot] = null;
            OnSlotUpdated?.Invoke(_choosingSlot, null); // Update inventory UI
            return itemToDrop;
        }
    }

    public ItemConfig TransferToStation(StationController station)
    {
        if (_choosingSlot == -1 || items[_choosingSlot] == null)
        {
            // Not choosing any item
            return null;
        }
        else
        {
            // Transfer the item to the station if the station 
            ItemConfig itemToTransfer = items[_choosingSlot];
            station.AddItem(itemToTransfer);
            Remove(_choosingSlot);
            return itemToTransfer;
        }
    }

    public FinalProductConfig Submit()
    {
        if (_choosingSlot == -1 || items[_choosingSlot] == null)
        {
            // Not choosing any item
            return null;
        }
        else
        {
            if (items[_choosingSlot] is FinalProductConfig product)
            {
                // Item is submissible
                bool submitted = LevelManager.Instance.OnProductSubmitted(product); // check if the product can be submitted
                if (submitted)
                {
                    Remove(_choosingSlot);
                    return product;
                }
                
            }
            return null;
        }
    }

    private bool Add(ItemConfig item)
    {
        int idx;
        // If choosing slot is empty, put the item into that slot; else put in the first slot that is empty
        if (items[_choosingSlot] == null) idx = _choosingSlot;
        else idx = FindSlot();

        // No empty slot, cannot add item
        if (idx == -1)
        {
            return false;
        }
        OnSlotUpdated?.Invoke(idx, item.Prefab.GetComponent<SpriteRenderer>().sprite); // Update inventory UI with the new item sprite
        // Found an empty slot, put item into that slot
        if (_isAutoFocus) ChoosingSlot = idx; // choose the current slot if is in auto focus mode
        items[idx] = item;
        return true;
    }

    private bool Remove(int idx)
    {
        // If the slot has item in it, remove the item
        if (items[idx] == null) return false;
        items[idx] = null;
        OnSlotUpdated?.Invoke(idx, null); // Update inventory UI to remove the item sprite
        return true;
    }

    private int FindSlot()
    {
        // Find the first empty slot
        for (int i = 0; i < GameConstants.MaxSlot; i++)
        {
            if (items[i] == null) return i;
        }

        return -1;
    } 
}