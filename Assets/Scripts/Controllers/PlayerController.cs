using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerConfig _config;
    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private List<IPlayerAction> _playerComponents = new List<IPlayerAction>();
    public List<IPlayerAction> PlayerComponents => _playerComponents;
    public PlayerConfig Config => _config;
    public PlayerInventory Inventory => _inventory;

    void Awake()
    {
        _inventory = new PlayerInventory();
        _playerComponents.Add(new PlayerMovement());
        _playerComponents.Add(new PlayerAttack());
        _playerComponents.Add(new PlayerInteraction());
        for (int i = 0; i < _playerComponents.Count; i++)
        {
            _playerComponents[i].Initialize(this);
        }
        Debug.Log($"Số lượng collider trên Player: {GetComponents<Collider2D>().Length}");
    }

    void Update()
    {
        for (int i = 0; i < _playerComponents.Count; i++)
        {
            _playerComponents[i].MyUpdate();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (_playerComponents[2] is PlayerInteraction interaction)
        {
            interaction.OnTriggerEnter2D(collider);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (_playerComponents[2] is PlayerInteraction interaction)
        {
            interaction.OnTriggerExit2D(collider);
        }
    }
}