using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInteraction : IComponent, IUpdatableComponent
{
    private PlayerController _player;
    [SerializeField] private List<GameObject> _objectInCollision = new List<GameObject>();

    [SerializeField] bool _isNearStation = false;
    PlayerInventory _inventory;
    StationController _nearStation;
    public void Initialize(PlayerController player)
    {
        _inventory = player.Inventory;
        _player = player;
    }
    
    public void MyUpdate()
    {
        // choosing slot logic
        for (int i = 0; i < 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                _inventory.ChoosingSlot = i;
                Debug.Log($"Choosing slot{_inventory.ChoosingSlot + 1}");
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _inventory.ChoosingSlot = (_inventory.ChoosingSlot + 1) % 5;
            Debug.Log($"Choosing slot{_inventory.ChoosingSlot}");
        }
        // Action priority: Pickup > Transfer ingredient > Attack
        if (Input.GetKeyDown(KeyCode.J))
        {
            // Pickup ingredient logic
            if (_objectInCollision.Count > 0)
            {
                // pick up logic
                float minDistance = 99999999f;
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
                Debug.Log($"Picked up ingredient: {nearestIngredient.name}");
                _inventory.Pickup(nearestIngredient.GetComponent<IngredientController>());
            }
            // Transfer ingredient to station logic
            else if (_isNearStation)
            {
                Debug.Log($"Transferred ingredient slot {_inventory.ChoosingSlot} to station");
                _inventory.TransferToStation(_nearStation);
            }
            // Attack
            else
            {
                _player.StartCoroutine(_player.Attack.Attack());
            }
        }

        // drop logic
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log($"Drop item number {_inventory.ChoosingSlot + 1} is {_inventory.Get(_inventory.ChoosingSlot)}");
            // Remove item at _ingredient.ChoosingSlot: 
            _inventory.Drop();
            
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
