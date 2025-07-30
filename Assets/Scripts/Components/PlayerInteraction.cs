using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction
{
    private PlayerController _player;
    private PlayerInventory _inventory;
    private PlayerInputManager _inputManager;
    private List<GameObject> _itemsInCollision = new List<GameObject>();
    private StationController _nearStation;
    private bool _isNearStation = false;
    private bool _isNearSubmissionPoint = false;
    private bool _isNearCraftPoint = false;
    private float _multiplier = 1;
    public void SetPointMultiplier(float multiplier)
    {
        _multiplier = multiplier;
    }
    public void Initialize(PlayerController player, PlayerInputManager inputManager)
    {
        _inventory = player.Inventory;
        _player = player;
        _inputManager = inputManager;

        _inputManager.controls.Player.Submit.performed += ctx => Submit();
        _inputManager.controls.Player.Transfer.performed += ctx => TransferToStation();
        _inputManager.controls.Player.Attack.performed += ctx =>
        {
            if (!_isNearSubmissionPoint && !_isNearStation) _player.StartCoroutine(_player.Attack.Attack());
        };
        _inputManager.controls.Player.Exploit.performed += ctx => { };
        _inputManager.controls.Player.Combine.performed += ctx =>
        {
            // combine & craft item
            if (_isNearCraftPoint) _nearStation.StartCrafting();
        };
        inputManager.controls.Player.Drop.performed += ctx => DropItem();
        inputManager.controls.Player.Pickup.performed += ctx => PickUpItem();
    }

    void PickUpItem()
    {
        if (_itemsInCollision.Count <= 0)
        {
            Debug.Log("No items nearby to pick up");
            return;
        }

        // Find nearest item
        float minDistance = Mathf.Infinity;
        GameObject nearestItem = _itemsInCollision[0];
        for (int i = 0; i < _itemsInCollision.Count; i++)
        {
            Vector2 distanceVector = _player.gameObject.transform.position - _itemsInCollision[i].transform.position;
            Debug.Log(distanceVector);
            float distance = (float)Math.Sqrt(distanceVector.x * distanceVector.x + distanceVector.y * distanceVector.y);
            if (distance < minDistance)
            {
                nearestItem = _itemsInCollision[i];
                minDistance = distance;
            }
        }

        ItemConfig pickedUpItem = _inventory.Add(nearestItem.GetComponent<ItemController>().Config);
        if (pickedUpItem)
        {
            ItemPool.Instance.RemoveItem(nearestItem.GetComponent<ItemController>());
            Debug.Log($"Picked up item: {pickedUpItem.Name}");
        }
        else
        {
            Debug.Log("Inventory is full");
        }
    }

    void TransferToStation()
    {
        if (!_isNearStation || _nearStation == null)
        {
            Debug.Log("No station nearby to transfer item");
            return;
        }

        ItemConfig transferredItem = _inventory.Remove();
        if (transferredItem)
        {
            _nearStation.AddItem(transferredItem);
            Debug.Log($"Transferred {transferredItem.Name} in slot {_inventory.ChoosingSlot + 1} to station");
        }
        else
        {
            Debug.Log("No item in slot to transfer");
        }
    }

    void Submit()
    {
        if (!_isNearSubmissionPoint)
        {
            Debug.Log("No submission point nearby to submit item");
            return;
        }
        
        ItemConfig submittedItem = _inventory.Get(_inventory.ChoosingSlot);
        if (submittedItem.Type == ItemType.Potion)
        {
            bool submitted = LevelManager.Instance.OnProductSubmitted(submittedItem, _multiplier);  
            if (submitted) _inventory.Remove();
            // Handle submission logic, e.g., update score, etc.  
            Debug.Log($"Submitted {submittedItem.Name} in slot {_inventory.ChoosingSlot + 1}");
        }
        else
        {
            Debug.Log("No item in slot to submit");
        }
        
    }

    void DropItem()
    {
        ItemConfig droppedItem = _inventory.Remove();
        if (droppedItem)
        {
            ItemPool.Instance.SpawnItem(droppedItem, _player.transform.position);
            Debug.Log($"Drop {droppedItem.Name} in slot {_inventory.ChoosingSlot + 1}");
        }
        else
        {
            Debug.Log("No item to drop");
        }        
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Item")
        {
            _itemsInCollision.Add(collider.gameObject);
            if (_itemsInCollision.Count == 1)
            {
                // Trigger display UI to inform player to pick item (K)
            }
        }
        if (collider.gameObject.tag == "SubmissionPoint")
        {
            _isNearSubmissionPoint = true;
        }
        if (collider.gameObject.tag == "Station")
        {
            _isNearStation = true;
            _nearStation = collider.gameObject.GetComponentInParent<StationController>();
            // display UI to inform player to transfer item
        }
        if (collider.gameObject.tag == "CraftPoint")
        {
            _isNearCraftPoint = true;
            _nearStation = collider.gameObject.GetComponentInParent<StationController>();
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Item")
        {
            _itemsInCollision.Remove(collider.gameObject);
            if (_itemsInCollision.Count == 0)
            {
                // set UI inform player to pick item into inActive
            }
        }
        if (collider.gameObject.tag == "Station")
        {
            _isNearStation = false;
            // disable "inform player to transfer item"
        }
        if (collider.gameObject.tag == "SubmissionPoint")
        {
            _isNearSubmissionPoint = false;
        }
        if (collider.gameObject.tag == "CraftPoint")
        {
            _isNearCraftPoint = false;
        }
    }
}
