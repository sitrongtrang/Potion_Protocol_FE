using UnityEngine;

public static class FormulaeCalculator
{
    public static int CalculateScore(RecipeConfig recipe, float multiplier)
    {
        return Mathf.CeilToInt(multiplier * recipe.CraftingTime * recipe.Inputs.Count / 5f) * 5;
    }
}