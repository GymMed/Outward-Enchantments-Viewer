using OutwardEnchantmentsViewer.Enchantments;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardEnchantmentsViewer.Utility.Helpers
{
    public class EnchantmentsHelper
    {
        public static string GetEnchantmentArmorIngrediantType(EnchantmentRecipe recipe)
        {
            foreach (EnchantmentRecipe.IngredientData data in recipe.CompatibleEquipments.CompatibleEquipments)
            {
                if(data.SpecificIngredient is Armor armor)
                {
                    return armor.EquipSlot.ToString();
                }
                else if(data.SpecificIngredient is Weapon weapon)
                {
                    return weapon.TypeDisplay.ToString();
                }
            }

            return "";
        }
        
        public static string GetEnchantmentDescription(EnchantmentRecipeItem enchantmentItem)
        {
            string output = $"{enchantmentItem.Name} \n"; ;
            //fill
            return output;
        }

        public static string GetEnchantmentsDescriptions(List<EnchantmentRecipeData> enchantmentRecipesDatas)
        {
            string output = "";

            foreach(EnchantmentRecipeData enchantmentRecipeData in enchantmentRecipesDatas)
            {
                output += GetEnchantmentDescription(enchantmentRecipeData.item);
            }

            return output;
        }

        public static string GetEnchantmentsDescriptions(List<EnchantmentRecipe> enchantmentRecipes)
        {
            string output = "";
            Enchantment enchantment;

            foreach(EnchantmentRecipe enchantmentRecipe in enchantmentRecipes)
            {
                enchantment = ResourcesPrefabManager.Instance.GetEnchantmentPrefab(enchantmentRecipe.RecipeID);
                output += $"Enchanting: {enchantment.Name} \n";
            }

            return output;
        }

        //includes pouch, backpack and equiped items
        public static List<EnchantmentRecipeData> GetAvailableEnchantmentRecipeDatasInInventory(Item item, CharacterInventory inventory)
        {
            List<Item> inventoryItems = ItemHelpers.GetUniqueItemsInInventory(inventory);
            List<EnchantmentRecipeItem> enchantments = ItemHelpers.GetAllItemsOfType<EnchantmentRecipeItem>(inventoryItems);
            List<EnchantmentRecipeData> haveEnchantments = new List<EnchantmentRecipeData>();

            foreach (EnchantmentRecipeItem enchantmentRecipeItem in enchantments)
            {
                foreach (EnchantmentRecipe enchantmentRecipe in enchantmentRecipeItem.Recipes)
                {
                    if (enchantmentRecipe.GetHasMatchingEquipment(item))
                    {
                        #if DEBUG
                        SL.Log($"{OutwardEnchantmentsViewer.prefix} equiment {item.Name} can be enchanted with {enchantmentRecipe.name} and {enchantmentRecipeItem.Name}, {enchantmentRecipeItem.name}");
                        #endif
                        haveEnchantments.Add(new EnchantmentRecipeData( enchantmentRecipeItem, enchantmentRecipe ));
                    }
                }
            }

            return haveEnchantments;
        }

        //includes pouch, backpack and equiped items
        public static List<EnchantmentRecipeItem> GetAvailableEnchantmentRecipeItemsInInventory(Item item, CharacterInventory inventory)
        {
            List<Item> inventoryItems = ItemHelpers.GetUniqueItemsInInventory(inventory);
            List<EnchantmentRecipeItem> enchantments = ItemHelpers.GetAllItemsOfType<EnchantmentRecipeItem>(inventoryItems);
            List<EnchantmentRecipeItem> haveEnchantments = new List<EnchantmentRecipeItem>();

            foreach (EnchantmentRecipeItem enchantmentRecipeItem in enchantments)
            {
                foreach (EnchantmentRecipe enchantmentRecipe in enchantmentRecipeItem.Recipes)
                {
                    if (enchantmentRecipe.GetHasMatchingEquipment(item))
                    {
                        #if DEBUG
                        SL.Log($"{OutwardEnchantmentsViewer.prefix} equiment {item.Name} can be enchanted with {enchantmentRecipe.name} and {enchantmentRecipeItem.Name}, {enchantmentRecipeItem.name}");
                        #endif
                        haveEnchantments.Add(enchantmentRecipeItem);
                    }
                }
            }

            return haveEnchantments;
        }

        public static List<EnchantmentRecipe> GetAvailableEnchantmentRecipies(Item item)
        {
            List<EnchantmentRecipe> enchantmentRecipes = RecipeManager.Instance.GetEnchantmentRecipes();
            List<EnchantmentRecipe> availableEnchantments = new List<EnchantmentRecipe>();

            foreach (EnchantmentRecipe enchantmentRecipe in enchantmentRecipes)
            {
                if (enchantmentRecipe.GetHasMatchingEquipment(item))
                {
                    #if DEBUG
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@SetEquipmentsEnchantmentsDescription equiment {item.Name} can be enchanted with {enchantmentRecipe.name}");
                    #endif
                    availableEnchantments.Add(enchantmentRecipe);
                }
            }

            return availableEnchantments;
        }

        public static List<EnchantmentRecipe> GetMissingEnchantments(List<EnchantmentRecipe> availableEnchantments, List<EnchantmentRecipe> haveEnchantments)
        {
            List<EnchantmentRecipe> missingEnchantments = new List<EnchantmentRecipe>();
            bool foundMissingRecipe = false;

            foreach (EnchantmentRecipe availableEnchantmentRecipe in availableEnchantments)
            {
                foreach (EnchantmentRecipe haveEnchantmentRecipe in haveEnchantments)
                {
                    if (availableEnchantmentRecipe.RecipeID == haveEnchantmentRecipe.RecipeID)
                    {
                        foundMissingRecipe = true;
                    }
                }

                if (foundMissingRecipe)
                    foundMissingRecipe = false;
                else
                {
                    missingEnchantments.Add(availableEnchantmentRecipe);
                }
            }

            return missingEnchantments;
        }

        // Fixes Game Developers Bug 
        public static void FixFilterRecipe()
        {
            Item filter = ResourcesPrefabManager.Instance.GetItemPrefab("5800047");

            if (filter == null)
                return;

            #if DEBUG
            SL.Log($"{OutwardEnchantmentsViewer.prefix} EnchantmentsHelper@FixFilterRecipe got item Filter!");
            #endif

            EnchantmentRecipeItem filterRecipeItem = filter as EnchantmentRecipeItem;
            if(!filterRecipeItem)
            {
                return;
            }

            EnchantmentRecipe[] filterRecipes = filterRecipeItem.Recipes;
            EnchantmentRecipe filterArmor = RecipeManager.Instance.GetEnchantmentRecipeForID(52);
            EnchantmentRecipe filterHelmet = RecipeManager.Instance.GetEnchantmentRecipeForID(53);
            EnchantmentRecipe filterBoots = RecipeManager.Instance.GetEnchantmentRecipeForID(54);

            if(filterArmor == null || filterHelmet == null || filterBoots == null)
            {
                return;
            }

            EnchantmentRecipe[] filterSet = new EnchantmentRecipe[] { filterArmor, filterHelmet, filterBoots };
            filterRecipeItem.Recipes = filterSet;

            #if DEBUG
            SL.Log($"{OutwardEnchantmentsViewer.prefix} EnchantmentsHelper@FixFilterRecipe fixed item Filter!");
            #endif
        }
    }
}
