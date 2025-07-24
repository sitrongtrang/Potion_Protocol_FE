using UnityEngine;

public static class FormulaeCalculator
{
    public static int CalculateScore(RecipeConfig recipe)
    {
        return Mathf.CeilToInt(recipe.CraftingTime * recipe.Inputs.Count / 5f) * 5;
    }
}