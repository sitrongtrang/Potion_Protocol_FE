using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    [SerializeField] private GameObject _ingredientList;
    [SerializeField] private Image _productImage;
    [SerializeField] private GameObject _ingredientImagePrefab;

    public void Initialize(RecipeConfig recipe)
    {
        if (recipe == null) return;

        _productImage.sprite = recipe.Product.Icon;

        foreach (Transform child in _ingredientList.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < recipe.Inputs.Count; i++)
        {
            ItemConfig ingredient = recipe.Inputs[i];
            GameObject ingredientImageObj = Instantiate(_ingredientImagePrefab, _ingredientList.transform);
            Image ingredientImage = ingredientImageObj.GetComponent<Image>();
            ingredientImage.sprite = ingredient.Icon;
            ingredientImageObj.transform.SetParent(_ingredientList.transform);
        }
    }

    //public void SetRecipe(RecipeConfig recipe)
    //{
    //    _productImage.sprite = recipe.Product.Icon;
    //    for (int i = 0; i < recipe.Inputs.Count; i++)
    //    {
    //        ItemConfig ingredient = recipe.Inputs[i];
    //        if (i >= _ingredientList.transform.childCount)
    //        {
    //            GameObject ingredientImageObj = Instantiate(_ingredientImagePrefab, _ingredientList.transform);
    //            Image ingredientImage = ingredientImageObj.GetComponent<Image>();
    //            ingredientImage.sprite = ingredient.Icon;
    //            ingredientImageObj.transform.SetParent(_ingredientList.transform);
    //        } 
    //        else
    //        {
    //            Image ingredientImage = _ingredientList.transform.GetChild(i).GetComponent<Image>();
    //            ingredientImage.sprite = ingredient.Icon;
    //        }
    //    }
    //}
}