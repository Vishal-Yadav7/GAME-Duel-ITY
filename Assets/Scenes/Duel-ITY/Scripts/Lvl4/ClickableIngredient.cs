using UnityEngine;

public class ClickableIngredient : MonoBehaviour
{
    public enum IngredientType
    {
        EggTray,
        SingleEgg,
        BrokenEgg,

        Water,
        Milk,
        TeaLeaves,

        MaggiePacket,

        Cook
    }

    [Header("Type")]
    public IngredientType ingredientType;

    [Header("Recipe Manager")]
    public CookingRecipeManager recipeManager;

    void OnMouseDown()
    {
        if (recipeManager == null)
        {
            Debug.LogWarning(
                "CookingRecipeManager Missing!"
            );
            return;
        }

        switch (ingredientType)
        {
            case IngredientType.EggTray:

                recipeManager.TapEggTray();

                break;

            case IngredientType.SingleEgg:

                recipeManager.TapSingleEgg();

                break;

            case IngredientType.BrokenEgg:

                recipeManager.TapBrokenEgg();

                break;

            case IngredientType.Water:

                recipeManager.TapWater();

                break;

            case IngredientType.Milk:

                recipeManager.TapMilk();

                break;

            case IngredientType.TeaLeaves:

                recipeManager.TapTeaLeaves();

                break;

            case IngredientType.MaggiePacket:

                recipeManager.TapMaggiePacket();

                break;

            case IngredientType.Cook:

                break;
        }
    }
}