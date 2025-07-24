using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction
{
    private PlayerInputManager _inputManager;
    private PlayerController _player;
    private PlayerInventory _inventory;
    [SerializeField] private List<GameObject> _itemsInCollision = new List<GameObject>();
    private StationController _nearStation;
    private GameObject _nearSubmissionPoint;
    private GameObject _nearCraftPoint;

    public void Initialize(PlayerController player, PlayerInputManager inputManager)
    {
        _inventory = player.Inventory;
        _player = player;
        _inputManager = inputManager;

        _inputManager.controls.Player.Submit.performed += ctx => Submit();
        _inputManager.controls.Player.Transfer.performed += ctx => TransferToStation();
        _inputManager.controls.Player.Attack.performed += ctx =>
        {
            if (_nearSubmissionPoint == null && _nearStation == null) _player.StartCoroutine(_player.Attack.Attack());
        };
        _inputManager.controls.Player.Exploit.performed += ctx => { };
        _inputManager.controls.Player.Combine.performed += ctx =>
        {
            // combine & craft item
            if (_nearCraftPoint) _nearStation.StartCrafting();
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
        if (_nearStation == null)
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
        if (_nearSubmissionPoint == null)
        {
            Debug.Log("No submission point nearby to submit item");
            return;
        }
        
        FinalProductConfig submittedProduct = _inventory.Remove() as FinalProductConfig;
        if (submittedProduct)
        {
            Debug.Log($"Submitted {submittedProduct.Name} in slot {_inventory.ChoosingSlot + 1}");
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
            _nearSubmissionPoint = collider.gameObject;
        }
        if (collider.gameObject.tag == "Station")
        {
            _nearStation = collider.gameObject.GetComponentInParent<StationController>();
            // display UI to inform player to transfer item
        }
        if (collider.gameObject.tag == "CraftPoint")
        {
            _nearCraftPoint = collider.gameObject;
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
            _nearStation = null;
            // disable "inform player to transfer item"
        }
        if (collider.gameObject.tag == "SubmissionPoint")
        {
            _nearSubmissionPoint = null;
        }
        if (collider.gameObject.tag == "CraftPoint")
        {
            _nearCraftPoint = null;
        }
    }
}
