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
            mainCanvas = FindFirstObjectByType<Canvas>();
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
            _progressBarInstance = Instantiate(progressBarPrefab, mainCanvas.transform);
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

        var slider = _progressBarInstance;
        slider.maxValue = 1f;
        slider.value = 0f;
        var rt = slider.GetComponent<RectTransform>();
        Vector3 offset = Vector3.down * 0.8f
        + (_config.Type == StationType.AlchemyStation ? Vector3.right * 0.4f : Vector3.zero);

        float elapsed = 0f;
        float total = recipe.CraftingTime;
        while (elapsed < total)
        {
            elapsed += Time.deltaTime;
            slider.value = Mathf.Clamp01(elapsed / total);
            Vector3 worldPos = transform.position + offset;
            rt.position = Camera.main.WorldToScreenPoint(worldPos);
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