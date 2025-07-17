using UnityEngine.UI;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] int _id;
    [SerializeField] GameObject _item;
    [SerializeField] GameObject _itemImagePrefab;
    [SerializeField] Sprite _choosingSlotImg;
    [SerializeField] Sprite _unChoosingSlotImg;
    void OnEnable()
    {
        _item = transform.GetChild(0).gameObject;
        EventBus.UpdateItem += UpdateItem;
        EventBus.UpdateChoosingSlot += UpdateChoosingSlot;
    }
    void OnDisable()
    {
        EventBus.UpdateItem -= UpdateItem;
        EventBus.UpdateChoosingSlot -= UpdateChoosingSlot;
    }
    void UpdateItem(int id, GameObject item)
    {
        if (_id == id)
        {
            
            if (item != null && _item.GetComponent<Image>().sprite == null)
            {

                _item.SetActive(true);
                _item.GetComponent<Image>().sprite = item.GetComponent<SpriteRenderer>().sprite;
            }
            else if (item == null && _item.GetComponent<Image>().sprite != null)
            {
                _item.GetComponent<Image>().sprite = null;
                _item.SetActive(false);
            }
            // Do nothing in others case
        }
    }

    void UpdateChoosingSlot(int idx)
    {
        if (_id == idx)
        {
            GetComponent<Image>().sprite = _choosingSlotImg;
        }
        else
        {
            GetComponent<Image>().sprite = _unChoosingSlotImg;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
