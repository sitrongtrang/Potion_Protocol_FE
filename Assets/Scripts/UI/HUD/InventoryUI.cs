using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private PlayerInventory _playerInventory;
    [SerializeField] private GameObject[] _inventoryItemsUI;
    [SerializeField] private GameObject[] _inventorySlots;
    [SerializeField] Sprite _unChoosingSlotImg;
    [SerializeField] Sprite _choosingSlotImg;

    public void Initialize(PlayerController player)
    {
        _playerInventory = player.Inventory;

        // Initialize inventory UI
        for (int i = 0; i < GameConstants.MaxSlot; i++)
        {
            _inventoryItemsUI[i].GetComponent<Image>().sprite = null;
            _inventoryItemsUI[i].SetActive(false);
            if (i != player.Inventory.ChoosingSlot)
                _inventorySlots[i].GetComponent<Image>().sprite = _unChoosingSlotImg;
            else
                _inventorySlots[i].GetComponent<Image>().sprite = _choosingSlotImg;
        }

        _playerInventory.OnSlotUpdated += UpdateInventoryUI;
        _playerInventory.OnChoosingSlotChanged += UpdateChoosingSlotUI;
    }

    private void OnEnable()
    {
        if (_playerInventory == null) return;
        _playerInventory.OnSlotUpdated += UpdateInventoryUI;
        _playerInventory.OnChoosingSlotChanged += UpdateChoosingSlotUI;
    }

    private void OnDisable()
    {
        if (_playerInventory == null) return;
        _playerInventory.OnSlotUpdated -= UpdateInventoryUI;
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
}