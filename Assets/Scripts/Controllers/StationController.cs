using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StationController : MonoBehaviour
{
    private StationConfig _config;
    private List<RecipeConfig> _recipes;
    private List<ItemConfig> _items;

    [SerializeField] private Slider progressBarPrefab;
    private Canvas mainCanvas;
    private Slider _progressBarInstance;

    private void Awake()
    {
        if (mainCanvas == null)
#pragma warning disable CS0618 // Type or member is obsolete
            mainCanvas = FindObjectOfType<Canvas>();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    public void Initialize(StationConfig config, List<RecipeConfig> recipes)
    {
        _config = config;
        _recipes = recipes;
        _items = new();
    }

    public void AddItem(ItemConfig config)
    {
        _items.Add(config);
        if (_config.Type == StationType.Furnace)
        {
            StartCrafting();
        }
    }

    public void StartCrafting()
    {

        int recipeIndex = FindMatchingRecipe();
        if (recipeIndex != -1)
        {
            _items.Clear();
            StartCoroutine(WaitForCraft(_recipes[recipeIndex]));
        }
        else
        {
            for (int i = 0; i < _items.Count; i++)
            {
                Vector2 stationPos = transform.position;
                Vector2 dropPosition = stationPos + 0.5f * (i + 1) * Vector2.down;
                DropItem(_items[i], dropPosition);
                _items.Remove(_items[i]);
            }
        }
    }

    public void DropItem(ItemConfig item, Vector2 dropPosition)
    {
        ItemPool.Instance.SpawnItem(item, dropPosition);
    }

    IEnumerator WaitForCraft(RecipeConfig recipe)
    {
        //yield return new WaitForSeconds(recipe.CraftingTime);

        if (progressBarPrefab != null)
        {
            _progressBarInstance = Instantiate(
                progressBarPrefab,
                mainCanvas.transform
            );

            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.down * 0.8f);
            _progressBarInstance.GetComponent<RectTransform>().position = screenPos;

            Debug.Log("cook");
            _progressBarInstance.maxValue = 1f;
            _progressBarInstance.value = 0f;
        }

        float elapsed = 0f;
        float total = recipe.CraftingTime;
        while (elapsed < total)
        {
            elapsed += Time.deltaTime;
            if (_progressBarInstance != null)
                _progressBarInstance.value = Mathf.Clamp01(elapsed / total);
            yield return null;
        }

        if (_progressBarInstance != null)
            Destroy(_progressBarInstance.gameObject);

        Vector2 stationPos = transform.position;
        Vector2 dropPosition = stationPos + 0.5f * Vector2.down;
        Debug.Log(recipe);
        DropItem(recipe.Product, dropPosition);
    }

    private bool MatchRecipe(RecipeConfig recipe)
    {
        var stationSet = new HashSet<string>(_items.Select(i => i.Id));
        var recipeSet = new HashSet<string>(recipe.Inputs.Select(i => i.Id));
        return stationSet.SetEquals(recipeSet);
    }

    private int FindMatchingRecipe()
    {
        for (int i = 0; i < _recipes.Count; i++)
        {
            if (MatchRecipe(_recipes[i]))
            {
                return i;
            }
        }
        return -1;
    }
}