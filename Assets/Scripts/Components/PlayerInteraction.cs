using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInteraction : IComponent, IUpdatableComponent
{
    private PlayerInputManager _inputManager;
    private PlayerController _player;
    [SerializeField] private List<GameObject> _objectInCollision = new List<GameObject>();
    [SerializeField] private bool _isNearStation = false;
    private PlayerInventory _inventory;
    private StationController _nearStation;
    private InputAction[] _inputAction;
    [SerializeField] private bool _isNearSubmissionPoint = false;
    private bool _isNearCraftPoint = false;

    public void Initialize(PlayerController player, PlayerInputManager inputManager)
    {
        _inventory = player.Inventory;
        _player = player;
        _inputManager = inputManager;
        // Choose slot
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
        _inputManager.controls.Player.Submit.performed += ctx =>
        {
            if (_isNearSubmissionPoint)
            {
                Submit();
            }
        };
        _inputManager.controls.Player.Transfer.performed += ctx =>
        {
            if (_isNearStation)
            {
                TransferToStation();
            }
        };
        _inputManager.controls.Player.Attack.performed += ctx =>
        {
            if (!_isNearSubmissionPoint && !_isNearStation) _player.StartCoroutine(_player.Attack.Attack());
        };
        _inputManager.controls.Player.Exploit.performed += ctx =>
        {
            // exploit
        };
        _inputManager.controls.Player.Combine.performed += ctx =>
        {
            // combine & craft item
            if (_isNearCraftPoint) EventBus.Craft();
        };
        inputManager.controls.Player.Drop.performed += ctx => DropItem();
        inputManager.controls.Player.Pickup.performed += ctx =>
        {
            if (_objectInCollision.Count > 0) PickUpItem();
            else
            {
                Debug.Log("Nothing to pick");
            }
        };
    }
    
    public void MyUpdate()
    {

    }

    void ChooseSlot(int slot)
    {
        _inventory.ChoosingSlot = slot;
        Debug.Log($"Choosing slot{_inventory.ChoosingSlot + 1}");
    }

    void NextSlot()
    {
        _inventory.ChoosingSlot = (_inventory.ChoosingSlot + 1) % GameConstants.MaxSlot;
        Debug.Log($"Choosing slot{_inventory.ChoosingSlot + 1}");
    }

    void PickUpItem()
    {
        // pick up logic
        float minDistance = Mathf.Infinity;
        // find nearest object in list collision objects
        GameObject nearestItem = _objectInCollision[0];
        for (int i = 0; i < _objectInCollision.Count; i++)
        {
            Vector2 distanceVector = _player.gameObject.transform.position - _objectInCollision[i].transform.position;
            Debug.Log(distanceVector);
            float distance = (float)Math.Sqrt(distanceVector.x * distanceVector.x + distanceVector.y * distanceVector.y);
            if (distance < minDistance)
            {
                nearestItem = _objectInCollision[i];
            }
        }

        ItemConfig pickedUpItem = _inventory.Pickup(nearestItem.GetComponent<ItemController>());
        if (pickedUpItem)
        {
            Debug.Log($"Picked up item: {pickedUpItem.Name}");
        }
        else
        {
            Debug.Log("Inventory is full");
        }
    }

    void TransferToStation()
    {
        ItemConfig transferredItem = _inventory.TransferToStation(_nearStation);
        if (transferredItem)
        {
            Debug.Log($"Transferred {transferredItem.Name} in slot {_inventory.ChoosingSlot + 1} to station");
        }
        else
        {
            Debug.Log("No item in slot to transfer");
        }
        
    }
    void Submit()
    {
        FinalProductConfig submittedProduct = _inventory.Submit();
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
        ItemConfig droppedItem = _inventory.Drop();
        if (droppedItem)
        {
            Debug.Log($"Drop {droppedItem.Name} in slot {_inventory.ChoosingSlot + 1}");
        }
        else
        {
            Debug.Log("No item to drop");
        }        
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider);
        if (collider.gameObject.tag == "Item")
        {
            _objectInCollision.Add(collider.gameObject);
            if (_objectInCollision.Count == 1)
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
            _nearStation = collider.gameObject.GetComponent<StationController>();
            // display UI to inform player to transfer item
        }
        if (collider.gameObject.tag == "CraftPoint")
        {
            _isNearCraftPoint = true;
        }
    }
    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Item")
        {
            _objectInCollision.Remove(collider.gameObject);
            if (_objectInCollision.Count == 0)
            {
                // set UI inform player to pick item into inActive
            }
        }
        if (collider.gameObject.tag == "Station")
        {
            _isNearStation = false;
            // disable "inform player to transfer item"
        }
    }
}
