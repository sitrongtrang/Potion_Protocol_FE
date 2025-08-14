using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeInfoPage : InfoPage
{
    [SerializeField] private TextMeshProUGUI _inputText;
    [SerializeField] private HorizontalLayoutGroup inputList;
    [SerializeField] private TextMeshProUGUI _cratfTimeText;
    [SerializeField] private TextMeshProUGUI _outputText;
    [SerializeField] private Image _outputImage;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    public override void DisplayInfo(EntityConfig entityConfig)
    {
        base.DisplayInfo(entityConfig);
        if (entityConfig is RecipeConfig recipeConfig)
        {
            if (!string.IsNullOrEmpty(recipeConfig.Description))
                _descriptionText.text = recipeConfig.Description;
            else
                _descriptionText.text = "...";
            _cratfTimeText.text = $"Craft time: {recipeConfig.CraftingTime}s";
            if (recipeConfig.Inputs != null && recipeConfig.Inputs.Count > 0)
            {
                _inputText.text = "Inputs:";
                inputList.gameObject.SetActive(true);
                for (int i = 0; i < inputList.transform.childCount; i++)
                {
                    var child = inputList.transform.GetChild(i);
                    if (i < recipeConfig.Inputs.Count)
                    {
                        var itemConfig = recipeConfig.Inputs[i];
                        child.GetComponent<Image>().sprite = itemConfig.Icon;
                        child.gameObject.SetActive(true);
                    }
                    else
                    {
                        child.GetComponent<Image>().sprite = null;
                        child.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                _inputText.text = "Inputs: ???";
                inputList.gameObject.SetActive(false);
            }
            if (recipeConfig.Product != null)
            {
                _outputText.text = "Output:";
                _outputImage.sprite = recipeConfig.Product.Icon;
                _outputImage.gameObject.SetActive(true);
            }
            else
            {
                _outputText.text = "Output: ???";
                _outputImage.sprite = null;
                _outputImage.gameObject.SetActive(false);
            }
        }
        else
        {
            _inputText.text = "Inputs: ???";
            inputList.gameObject.SetActive(false);
            _cratfTimeText.text = "Craft time: ???";
            _outputText.text = "Output: ???";
            _outputImage.sprite = null;
            _outputImage.gameObject.SetActive(false);
            _descriptionText.text = "...";
        }
    }
}