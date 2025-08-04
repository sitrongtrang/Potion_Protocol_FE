using System.Collections.Generic;
using UnityEngine;

public class AlchemyStationController : StationController
{
    [SerializeField] private GameObject _table;
    [SerializeField] private GameObject[] _itemsOnTable;

    public override void Initialize(List<RecipeConfig> recipes)
    {
        base.Initialize(recipes);
        for (int i = 0; i < _itemsOnTable.Length; i++)
        {
            _itemsOnTable[i].SetActive(false);
        }
    }

    public override bool AddItem(ItemConfig config)
    {
        if (_items.Count < GameConstants.MaxItemsInAlchemyStation)
        {
            bool added = base.AddItem(config);
            if (!added) return false;
            _itemsOnTable[_items.Count - 1].SetActive(true);
            _itemsOnTable[_items.Count - 1].GetComponent<SpriteRenderer>().sprite = config.Icon;
            return true;
        }
        else
        {
            DropItem(config);
            Debug.LogWarning("Cannot add more items to the Alchemy Station.");
            return false;
        }
    }

    public override void RemoveItem(int idx, bool drop = true)
    {
        base.RemoveItem(idx, drop);
        _itemsOnTable[_items.Count].SetActive(false);
        _itemsOnTable[_items.Count].GetComponent<SpriteRenderer>().sprite = null;
    }

    public override Vector2 GetTransferZone()
    {
        return _table.transform.position;
    }
}