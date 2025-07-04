using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerConfig _config;
    private List<IUpdatableComponent> _updatableComponents = new();

    public PlayerConfig Config => _config;
    public PlayerInventory Inventory { get; private set; }
    public PlayerAttack Attack { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    public PlayerMovement Movement { get; private set; }

    void Awake()
    {
        Inventory = RegisterComponent(new PlayerInventory());
        Attack = RegisterComponent(new PlayerAttack());
        Interaction = RegisterComponent(new PlayerInteraction());
        Movement = RegisterComponent(new PlayerMovement());
    }

    void Update()
    {
        for (int i = 0; i < _updatableComponents.Count; i++)
        {
            _updatableComponents[i].MyUpdate();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Interaction.OnTriggerEnter2D(collider);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        Interaction.OnTriggerExit2D(collider);
    }

    private T RegisterComponent<T>(T component) where T : class
    {
        if (component is IComponent playerComponent)
        {
            playerComponent.Initialize(this);
        }

        if (component is IUpdatableComponent updatable)
        {
            _updatableComponents.Add(updatable);
        }

        return component;
    }
}