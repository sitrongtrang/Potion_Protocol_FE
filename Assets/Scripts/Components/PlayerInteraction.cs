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

    #region Initialization
    public void Initialize(PlayerController player, PlayerInputManager inputManager)
    {
        _player = player;
        _inventory = player.Inventory;
        _interactDistance = player.Config.InteractDistance;
        _inputManager = inputManager;

        _inputManager.controls.Player.Submit.performed += ctx => TrySubmit();
        _inputManager.controls.Player.Transfer.performed += ctx => TryTransfer();
        _inputManager.controls.Player.Combine.performed += ctx => TryCraft();
        inputManager.controls.Player.Drop.performed += ctx => TryDrop();
        inputManager.controls.Player.Pickup.performed += ctx => TryPickup();
    }
    #endregion

    #region Interaction
    void TryPickup()
    {
        ItemController[] activeItems = ItemPool.Instance.ActiveItems;
        if (activeItems.Length <= 0)
        {
            Debug.Log("No item nearby to pick up");
            return;
        }
        // Find nearest item
        float minDistance = Mathf.Infinity;
        ItemController nearestItem = activeItems[0];
        for (int i = 0; i < activeItems.Length; i++)
        {
            Vector2 distanceVector = _player.gameObject.transform.position - activeItems[i].transform.position;
            float distance = (float)Math.Sqrt(distanceVector.x * distanceVector.x + distanceVector.y * distanceVector.y);
            if (distance < minDistance)
            {
                nearestItem = activeItems[i];
                minDistance = distance;
            }
        }

        if (minDistance > _interactDistance || nearestItem == null)
        {
            Debug.Log("No item nearby to pick up");
            return;
        }

        // Simulate pickup logic
        SimulatePickup(nearestItem.GetComponent<ItemController>());
    }

    private void SimulatePickup(ItemController item)
    {
        ItemConfig pickedUpItem = _inventory.TryAdd(item.Config);
        if (pickedUpItem)
        {
            ItemPool.Instance.RemoveItem(item);
            Debug.Log($"Picked up item: {pickedUpItem.Name}");
        }
        else
        {
            Debug.Log("Could not pickup item.");
        }
    }

    private void TryTransfer()
    {
        // Find nearest station
        float minDistance = Mathf.Infinity;
        StationController nearStation = null;
        for (int i = 0; i < LevelManager.Instance.Stations.Count; i++)
        {
            Vector2 position = _player.gameObject.transform.position;
            Vector2 distanceVector = position - LevelManager.Instance.Stations[i].GetTransferZone();
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

        // Simulate transfer logic
        ItemConfig transferredItem = _inventory.TryRemove();
        SimulateTransfer(nearStation, transferredItem);
    }

    private void SimulateTransfer(StationController station, ItemConfig item)
    {
        if (item)
        {
            station.AddItem(item);
            Debug.Log($"Transferred {item.Name} in slot {_inventory.ChoosingSlot + 1} to station");
        }
        else
        {
            Debug.Log("Could not transfer to station.");
        }
    }

    private void TryCraft()
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
            _player.Attack.TryAttack(); // Trigger attack animation as feedback
            return;
        }

        // Simulate craft logic
        SimulateCraft(nearStation);
    }

    private void SimulateCraft(StationController station)
    {
        station.StartCrafting();
    }

    private void TrySubmit()
    {
        // Check for nearby submission point
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

        // Simulate submit logic
        ItemConfig submittedItem = _inventory[_inventory.ChoosingSlot];
        SimulateSubmit(submittedItem);
    }

    private void SimulateSubmit(ItemConfig item)
    {
        if (item != null && item.Type == ItemType.Potion)
        {
            bool submitted = LevelManager.Instance.OnProductSubmitted(item, _scoreMultiplier);
            if (submitted) _inventory.TryRemove();
            Debug.Log($"Submitted {item.Name} in slot {_inventory.ChoosingSlot + 1}");
        }
        else
        {
            Debug.Log("Could not submit.");
        }
    }

    private void TryDrop()
    {
        ItemConfig droppedItem = _inventory.TryRemove();
        SimulateDrop(droppedItem);
    }

    private void SimulateDrop(ItemConfig item)
    {
        if (item)
        {
            ItemPool.Instance.SpawnItem(item, _player.transform.position);
            Debug.Log($"Drop {item.Name} in slot {_inventory.ChoosingSlot + 1}");
        }
        else
        {
            Debug.Log("Could not drop");
        }
    }
    #endregion
}
