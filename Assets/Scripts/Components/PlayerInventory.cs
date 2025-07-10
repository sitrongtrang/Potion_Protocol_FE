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
            if (oldSlot != value) OnSlotChanged?.Invoke();
        }
    }
    private bool _isAutoFocus;
    public event Action OnSlotChanged;
    public ItemConfig Get(int idx) => items[idx];

    public void Initialize(PlayerController player, PlayerInputManager inputManager)
    {
        _player = player;
        _isAutoFocus = GameManager.Instance.IsAutoFocus;
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
            Vector3 dropPosition = _player.transform.position + _player.transform.forward;

            ItemPool.Instance.SpawnItem(itemToDrop, dropPosition);

            // Remove from inventory
            items[_choosingSlot] = null;
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
                // item is submissible
                Remove(_choosingSlot);
                return product;
            }
            else
            {
                return null;
            }
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