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
        _inputManager.controls.Player.Interact.performed += ctx =>
        {
            // Action priority: Pickup > Transfer ingredient > Attack
            // Pickup ingredient logic

            // Transfer ingredient to station logic
            if (_isNearStation)
            {
                TransferToStation();
            }
            // Attack
            else
            {
                _player.StartCoroutine(_player.Attack.Attack());
            }
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
        Debug.Log($"Choosing slot{_inventory.ChoosingSlot}");
    }

    void PickUpItem()
    {
        // pick up logic
        float minDistance = Mathf.Infinity;
        // find nearest object in list collision objects
        GameObject nearestIngredient = _objectInCollision[0];
        for (int i = 0; i < _objectInCollision.Count; i++)
        {
            Vector2 distanceVector = _player.gameObject.transform.position - _objectInCollision[i].transform.position;
            Debug.Log(distanceVector);
            float distance = (float)Math.Sqrt(distanceVector.x * distanceVector.x + distanceVector.y * distanceVector.y);
            if (distance < minDistance)
            {
                nearestIngredient = _objectInCollision[i];
            }
        }

        bool pickedUp = _inventory.Pickup(nearestIngredient.GetComponent<IngredientController>());
        if (pickedUp)
        {
            Debug.Log($"Picked up ingredient: {nearestIngredient.name}");
        }
        else
        {
            Debug.Log("Inventory is full");
        }
    }

    void TransferToStation()
    {
        bool transferred = _inventory.TransferToStation(_nearStation);
        if (transferred)
        {
            Debug.Log($"Transferred {_inventory.Get(_inventory.ChoosingSlot).Name} in slot {_inventory.ChoosingSlot + 1} to station");
        }
        else
        {
            Debug.Log("No ingredient in slot to transfer");
        }
        
    }

    void DropItem()
    {
        bool dropped = _inventory.Drop();
        if (dropped)
        {
            Debug.Log($"Drop ingredient in slot {_inventory.ChoosingSlot + 1}");
        }
        else
        {
            Debug.Log("No ingredient to drop");
        }        
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider);
        if (collider.gameObject.tag == "Ingredient")
        {
            _objectInCollision.Add(collider.gameObject);
            if (_objectInCollision.Count == 1)
            {
                // Trigger display UI to inform player to pick item (K)
            }

        }
        if (collider.gameObject.tag == "Station")
        {
            _isNearStation = true;
            _nearStation = collider.gameObject.GetComponent<StationController>();
            // display UI to inform player to transfer item
        }
    }
    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Ingredient")
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
