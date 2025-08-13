using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private PlayerInventory _playerInventory;
    [SerializeField] private GameStateHandler _gameStateHandler;
    [SerializeField] private StartGameHandler _startGameHandler;
    [SerializeField] private GameObject[] _inventoryItemsUI;
    [SerializeField] private GameObject[] _inventorySlots;
    [SerializeField] Sprite _unChoosingSlotImg;
    [SerializeField] Sprite _choosingSlotImg;

    public void Initialize(PlayerInventory playerInventory)
    {
        _playerInventory = playerInventory;
        
        for (int i = 0; i < GameConstants.MaxSlot; i++)
        {
            _inventoryItemsUI[i].GetComponent<Image>().sprite = null;
            _inventoryItemsUI[i].SetActive(false);
            if (i != playerInventory.ChoosingSlot)
                _inventorySlots[i].GetComponent<Image>().sprite = _unChoosingSlotImg;
            else
                _inventorySlots[i].GetComponent<Image>().sprite = _choosingSlotImg;
        }
    }

    void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            _playerInventory.OnSlotUpdated += UpdateInventoryUI;
        }
        else if (SceneManager.GetActiveScene().name == "OnlineGameScene")
        {
            _gameStateHandler.OnInventorySynced += SyncInventory;
        }

        _playerInventory.OnChoosingSlotChanged += UpdateChoosingSlotUI;
    }

    void OnDisable()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            _playerInventory.OnSlotUpdated -= UpdateInventoryUI;
        }
        else if (SceneManager.GetActiveScene().name == "OnlineGameScene")
        {
            _gameStateHandler.OnInventorySynced -= SyncInventory;
        }

        _playerInventory.OnChoosingSlotChanged -= UpdateChoosingSlotUI;
    }

    private void UpdateChoosingSlotUI(int oldSlotIndex, int newSlotIndex)
    {
        if (oldSlotIndex >= 0 && oldSlotIndex < GameConstants.MaxSlot)
            _inventorySlots[oldSlotIndex].GetComponent<Image>().sprite = _unChoosingSlotImg;
        _inventorySlots[newSlotIndex].GetComponent<Image>().sprite = _choosingSlotImg;
    }

    private void UpdateInventoryUI(int slotIndex, Sprite sprite = null)
    {
        _inventoryItemsUI[slotIndex].SetActive(sprite != null);
        _inventoryItemsUI[slotIndex].GetComponent<Image>().sprite = sprite;
    }

    private void SyncInventory(string[] itemTypeIds, int[] indicies)
    {
        HashSet<int> invInd = new HashSet<int>();
        for (int i = 0; i < itemTypeIds.Length; i++)
        {
            int index = indicies[i];
            invInd.Add(index);
            ScriptableObject scriptableObject = _gameStateHandler.PrefabsMap.GetSO(itemTypeIds[i]);
            if (scriptableObject is ItemConfig itemConfig)
            {
                UpdateInventoryUI(index, itemConfig.Icon);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            if (!invInd.Contains(i)) UpdateInventoryUI(i);
        }
    }
}