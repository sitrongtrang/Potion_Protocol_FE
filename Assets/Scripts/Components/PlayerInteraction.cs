using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction
{
    private PlayerController _player;
    private PlayerInventory _inventory;
    private PlayerInputManager _inputManager;
    private float _interactDistance;
    private float _scoreMultiplier = 1;

    public float ScoreMultiplier
    {
        get => _scoreMultiplier;
        set => _scoreMultiplier = value;
    }

    public void Initialize(PlayerController player, PlayerInputManager inputManager)
    {
        _player = player;
        _inventory = player.Inventory;
        _interactDistance = player.InteractionDistance;
        _inputManager = inputManager;

        _inputManager.controls.Player.Submit.performed += ctx => Submit();
        _inputManager.controls.Player.Transfer.performed += ctx => Transfer();
        _inputManager.controls.Player.Combine.performed += ctx => Craft();
        inputManager.controls.Player.Drop.performed += ctx => DropItem();
        inputManager.controls.Player.Pickup.performed += ctx => PickUpItem();
    }

    void PickUpItem()
    {
        ItemController[] itemsInCollision = ItemPool.Instance.ActiveItems;
        // Find nearest item
        float minDistance = Mathf.Infinity;
        ItemController nearestItem = itemsInCollision[0];
        for (int i = 0; i < itemsInCollision.Length; i++)
        {
            Vector2 distanceVector = _player.gameObject.transform.position - itemsInCollision[i].transform.position;
            float distance = (float)Math.Sqrt(distanceVector.x * distanceVector.x + distanceVector.y * distanceVector.y);
            if (distance < minDistance)
            {
                nearestItem = itemsInCollision[i];
                minDistance = distance;
            }
        }

        if (minDistance > _interactDistance || nearestItem == null)
        {
            Debug.Log("No item nearby to pick up");
            return;
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

    void Transfer()
    {
        // Find nearest station
        float minDistance = Mathf.Infinity;
        StationController nearStation = null;
        for (int i = 0; i < LevelManager.Instance.Stations.Count; i++)
        {
            Vector2 distanceVector = _player.gameObject.transform.position - LevelManager.Instance.Stations[i].transform.position;
            float distance = (float)Math.Sqrt(distanceVector.x * distanceVector.x + distanceVector.y * distanceVector.y);
            if (distance < minDistance)
            {
                nearStation = LevelManager.Instance.Stations[i];
                minDistance = distance;
            }
        }

        if (minDistance > _interactDistance || nearStation == null)
        {
            Debug.Log("No station nearby to transfer item");
            return;
        }

        ItemConfig transferredItem = _inventory.Remove();
        if (transferredItem)
        {
            nearStation.AddItem(transferredItem);
            Debug.Log($"Transferred {transferredItem.Name} in slot {_inventory.ChoosingSlot + 1} to station");
        }
        else
        {
            Debug.Log("No item in slot to transfer");
        }
    }

    void Craft()
    {
        // Find nearest alchemy station
        float minDistance = Mathf.Infinity;
        StationController nearStation = null;
        for (int i = 0; i < LevelManager.Instance.Stations.Count; i++)
        {
            if (LevelManager.Instance.Stations[i].Config.Type != StationType.AlchemyStation) continue;
            Vector2 distanceVector = _player.gameObject.transform.position - LevelManager.Instance.Stations[i].transform.position;
            float distance = (float)Math.Sqrt(distanceVector.x * distanceVector.x + distanceVector.y * distanceVector.y);
            if (distance < minDistance)
            {
                nearStation = LevelManager.Instance.Stations[i];
                minDistance = distance;
            }
        }
        if (minDistance > _interactDistance || nearStation == null)
        {
            Debug.Log("No alchemy station nearby to craft item");
            _player.StartCoroutine(_player.Attack.Attack()); // Trigger attack animation as feedback
            return;
        }

        nearStation.StartCrafting();
    }

    void Submit()
    {
        GameObject submissionPoint = GameObject.FindGameObjectWithTag("SubmissionPoint");
        if (submissionPoint == null)
        {
            Debug.Log("No submission point found");
            return;
        }
        else
        {
            Vector2 distanceVector = _player.gameObject.transform.position - submissionPoint.transform.position;
            float distance = (float)Math.Sqrt(distanceVector.x * distanceVector.x + distanceVector.y * distanceVector.y);
            if (distance > _interactDistance)
            {
                Debug.Log("No submission point nearby to submit item");
                return;
            }
        }

        ItemConfig submittedItem = _inventory.Get(_inventory.ChoosingSlot);
        if (submittedItem.Type == ItemType.Potion)
        {
            bool submitted = LevelManager.Instance.OnProductSubmitted(submittedItem, _scoreMultiplier);
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
}
