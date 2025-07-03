using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    // [SerializeField] private GameObject[] _inventory = new GameObject[5];
    // int _slotEmptyRemaining = 5;
    int _slotChoosing = 1; // default value of choosing slot is 1 (first slot)
    [SerializeField] private List<GameObject> _objectInCollision = new List<GameObject>();

    [SerializeField] bool _isNearStation = false;
    bool _isPickupIngredient = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // choosing slot logic
        for (int i = 0; i < 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                _slotChoosing = i + 1;
                Debug.Log($"Choosing slot{_slotChoosing}");
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _slotChoosing = _slotChoosing % 5 + 1;
            Debug.Log($"Choosing slot{_slotChoosing}");
        }
        // Transfer ingredient to station logic
        if (_isNearStation && Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log($"Transferred ingredient slot {_slotChoosing} to station");
            // if (_inventory[_slotChoosing - 1] != null)
            // {
            //     // Trigger station logic
            //     _inventory[_slotChoosing - 1] = null;
            // }
        }

        // Throw/pickup ingredient logic
        if (_objectInCollision.Count > 0 && Input.GetKeyDown(KeyCode.K))
        {
            _isPickupIngredient = !_isPickupIngredient;
            if (_isPickupIngredient)
            {
                Debug.Log("Picked up ingredient");
            }
            else
            {
                Debug.Log("Drop ingredient");
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collider)
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
            // display UI to inform player to pick item
        }
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Ingredient")
        {
            _objectInCollision.Remove(collider.gameObject);
            if (_objectInCollision.Count == 0)
            {
                // set UI inActive
            }
        }
        if (collider.gameObject.tag == "Station")
        {
            _isNearStation = false;
        }
    }
}
