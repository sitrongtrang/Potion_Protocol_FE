using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
class FinalRecipe
{
    // 
    public GameObject _recipeImagePrefab;
    public List<GameObject> _listIngredientsPrefab;
    public FinalRecipe(GameObject imageObj, List<GameObject> ingredients)
    {
        _recipeImagePrefab = imageObj;
        _listIngredientsPrefab = ingredients;
    }
}
public class FinalItemsController : MonoBehaviour
{
    [SerializeField] LevelConfig _levelConfig;
    [SerializeField] private List<RecipeConfig> _recipeConfigs = new List<RecipeConfig>(); // quản lý giống stack
    [SerializeField] private GameObject _finalRecipeUIPrefab;
    private List<FinalRecipe> _finalRecipe = new List<FinalRecipe>();
    [SerializeField] private GameObject _recipeImagePrefab;
    [SerializeField] private GameObject _ingredientImagePrefab;
    [SerializeField] private float slideDuration = 0.3f;
    [SerializeField] private float itemSpacing = 0f; // chiều ngang spacing
    private bool isDeleting = false;
    private int maxCount = 5;


    void Start()
    {
        for (int i = 0; i < maxCount; i++)
        {
            AddNewItem();
        }
        RefreshDisplay();
    }

    void AddNewItem()
    {
        // Random potion
        int randomItem = Random.Range(0, _levelConfig.FinalRecipes.Count);
        // final recipe item type: Recipe Config
        RecipeConfig recipe = _levelConfig.FinalRecipes[randomItem];
        _recipeConfigs.Add(recipe);
        List<GameObject> ingredients = new List<GameObject>();
        GameObject _finalRecipeUI = Instantiate(_finalRecipeUIPrefab, transform);
        GameObject imageObj = Instantiate(_recipeImagePrefab, _finalRecipeUI.transform);
        // Đặt sprite cho hình ảnh chính
        Image mainImage = imageObj.GetComponent<Image>();
        if (mainImage != null)
        {
            mainImage.sprite = recipe.Product.Prefab.GetComponent<SpriteRenderer>().sprite;
        }
        for (int i = 0; i < recipe.Inputs.Count; i++)
        {
            GameObject ingredient = Instantiate(_ingredientImagePrefab, _finalRecipeUI.transform.GetChild(0));
            ingredient.GetComponent<Image>().sprite = recipe.Inputs[i].Prefab.gameObject.GetComponent<SpriteRenderer>().sprite;
            ingredients.Add(ingredient);
        }
        
        
        _finalRecipe.Add(new FinalRecipe(imageObj, ingredients));
    }

    void RefreshDisplay()
    {
        for (int i = 0; i < _finalRecipe.Count; i++)
        {
            var recipe = _finalRecipe[i];
            RectTransform rt = recipe._recipeImagePrefab.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(i * itemSpacing, 0);
        }
    }

    void RemoveItem()
    {
        if (_recipeConfigs.Count == 0 || isDeleting) return;

        isDeleting = true;

        // Xoá phần tử đầu
        _recipeConfigs.RemoveAt(0);

        FinalRecipe removed = _finalRecipe[0];
        _finalRecipe.RemoveAt(0);

        // Huỷ các object liên quan
        Destroy(removed._recipeImagePrefab.transform.parent.gameObject); // huỷ toàn bộ UI cha chứa cả nguyên liệu

        StartCoroutine(SlideAndAdd());
    }

    IEnumerator SlideAndAdd()
    {
        float elapsed = 0f;

        // Lưu vị trí ban đầu
        List<Vector2> startPositions = new List<Vector2>();
        foreach (var recipe in _finalRecipe)
        {
            RectTransform rt = recipe._recipeImagePrefab.GetComponent<RectTransform>();
            startPositions.Add(rt.anchoredPosition);
        }

        // Animate trượt trái
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);

            for (int i = 0; i < _finalRecipe.Count; i++)
            {
                RectTransform rt = _finalRecipe[i]._recipeImagePrefab.GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.Lerp(startPositions[i], startPositions[i] - new Vector2(itemSpacing, 0), t);
            }

            yield return null;
        }

        // Thêm item mới vào cuối
        AddNewItem();
        RefreshDisplay();

        yield return new WaitForSeconds(1f);
        isDeleting = false;
    }

    
    void Update()
    {
        if (!isDeleting)
        {
            RemoveItem();
        }
    }
}
