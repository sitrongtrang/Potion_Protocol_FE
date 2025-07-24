using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory
{
    private PlayerController _player;
    private PlayerInputManager _inputManager;
    private InputAction[] _inputAction;
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
                OnChoosingSlotChanged?.Invoke(oldSlot, value);
            } 
        }
    }
    private bool _isAutoFocus;
    public event Action<int, int> OnChoosingSlotChanged;
    public event Action<int, Sprite> OnSlotUpdated;

    public void Initialize(PlayerController player, PlayerInputManager inputManager)
    {
        ChoosingSlot = 0;
        _player = player;
        _inputManager = inputManager;

        _inputAction = new InputAction[GameConstants.MaxSlot] {
            _inputManager.controls.Player.ChooseSlot1,
            _inputManager.controls.Player.ChooseSlot2,
            _inputManager.controls.Player.ChooseSlot3,
            _inputManager.controls.Player.ChooseSlot4,
            _inputManager.controls.Player.ChooseSlot5
        };

        for (int i = 0; i < GameConstants.MaxSlot; i++)
        {
            int index = i;
            _inputAction[i].performed += ctx => ChooseSlot(index);
        }

        _inputManager.controls.Player.Nextslot.performed += ctx => NextSlot();

        _isAutoFocus = PlayerPrefs.GetInt("IsAutoFocus") == 1;
    }

    private void ChooseSlot(int index)
    {
        if (index < 0 || index >= GameConstants.MaxSlot) return;
        ChoosingSlot = index;
        Debug.Log($"Slot {index + 1} chosen.");
    }

    private void NextSlot()
    {
        ChoosingSlot = (_choosingSlot + 1) % GameConstants.MaxSlot;
        Debug.Log($"Next slot chosen: {_choosingSlot + 1}");
    }

    public ItemConfig Add(ItemConfig item)
    {
        int slotIndex = GetAddSlot();
        if (slotIndex < 0 || slotIndex >= GameConstants.MaxSlot)
        {
            Debug.LogWarning("Invalid slot index to add.");
            return null;
        }

        if (item == null)
        {
            Debug.LogWarning($"Item not found.");
            return null;
        }

        if (items[slotIndex] != null)
        {
            Debug.LogWarning($"Slot {slotIndex + 1} is already occupied. Cannot add item {item.Name}.");
            return null;
        }

        items[slotIndex] = item;
        OnSlotUpdated?.Invoke(slotIndex, item.Icon);
        if (_isAutoFocus) ChoosingSlot = slotIndex;
        Debug.Log($"Added up item {item.Name} into slot {slotIndex + 1}");
        return item;
    }

    public ItemConfig Remove()
    {
        if (_choosingSlot < 0 || _choosingSlot >= GameConstants.MaxSlot || items[_choosingSlot] == null)
        {
            Debug.LogWarning("Invalid slot for drop.");
            return null;
        }

        ItemConfig item = items[_choosingSlot];
        items[_choosingSlot] = null; 
        OnSlotUpdated?.Invoke(_choosingSlot, null);
        Debug.Log($"Removed up item {item.Name} in slot {_choosingSlot + 1}");
        return item;
    }

    private int GetAddSlot()
    {
        int idx = -1;
        // If choosing slot is empty, choose that slot to add; else find another empty slot
        if (items[_choosingSlot] == null) idx = _choosingSlot;
        else idx = FindEmptySlot();
        return idx;
    }

    private int FindEmptySlot()
    {
        // Find the first empty slot
        for (int i = 0; i < GameConstants.MaxSlot; i++)
        {
            if (items[i] == null) return i;
        }

        return -1;
    } 
}