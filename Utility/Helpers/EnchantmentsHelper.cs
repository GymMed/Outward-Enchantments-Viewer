using OutwardEnchantmentsViewer.Enchantments;
using OutwardEnchantmentsViewer.Utility.Enums;
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
                    return EquipmentSlotIDsExtensions.GetArmorTypeName(armor.EquipSlot);
                }
                else if(data.SpecificIngredient is Weapon weapon)
                {
                    return WeaponTypeExtensions.GetWeaponTypeName(weapon.Type);
                }
                else
                {
                    if (data.SpecificIngredient == null)
                        return "";

                    return data.SpecificIngredient.Name;
                }
            }

            return "";
        }
        
        public static string GetDetailedEnchantmentDescription(EnchantmentRecipeItem enchantmentItem, Equipment equipment, int count = 1)
        {
            string output = $"{enchantmentItem.Name}";

            if (count > 1)
                output += $" ({count})";

            output += "\n";
            Enchantment enchantment = null;

            foreach (EnchantmentRecipe recipe in enchantmentItem.Recipes)
            {
                if(recipe.GetHasMatchingEquipment(equipment))
                {
                    enchantment = ResourcesPrefabManager.Instance.GetEnchantmentPrefab(recipe.RecipeID);
                    break;
                }
            }

            if (enchantment == null)
                return output;

            output += ItemEnchantmentInformationHelper.BuildDescriptions(enchantment, equipment);

            return output;
        }

        public static string GetDetailedEnchantmentsDescriptions(List<EnchantmentRecipeDetailedData> enchantmentRecipesDatas, Equipment equipment)
        {
            string output = "";
            int enchantmentRecipesDatasMinusOne = enchantmentRecipesDatas.Count - 1;

            for(int currentRecipeData = 0; currentRecipeData < enchantmentRecipesDatasMinusOne; currentRecipeData++)
            {
                output += GetDetailedEnchantmentDescription(enchantmentRecipesDatas[currentRecipeData].Data.item, equipment, enchantmentRecipesDatas[currentRecipeData].Count);
                output += "\n";
            }

            if(enchantmentRecipesDatas.Count > 0)
            {
                output += GetDetailedEnchantmentDescription(enchantmentRecipesDatas[enchantmentRecipesDatasMinusOne].Data.item, equipment, enchantmentRecipesDatas[enchantmentRecipesDatasMinusOne].Count);
            }

            return output;
        }

        public static string GetEnchantmentDescription(EnchantmentRecipeItem enchantmentItem, int count)
        {
            string output = $"{enchantmentItem.Name}";

            if (count > 1)
                output += $" ({count})";

            output += "\n";

            return output;
        }

        public static string GetEnchantmentsDescriptions(List<EnchantmentRecipeDetailedData> enchantmentRecipesDatas)
        {
            string output = "";

            foreach(EnchantmentRecipeDetailedData enchantmentRecipeData in enchantmentRecipesDatas)
            {
                output += GetEnchantmentDescription(enchantmentRecipeData.Data.item, enchantmentRecipeData.Count);
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

        public static string GetDetailedEnchantmentsDescriptions(List<EnchantmentRecipe> enchantmentRecipes, Equipment equipment)
        {
            string output = "";
            
            int enchantmentRecipesMinusOne = enchantmentRecipes.Count - 1;

            for(int currentRecipe = 0; currentRecipe < enchantmentRecipesMinusOne; currentRecipe++)
            {
                output += GetDetailedEnchantmentDescription(enchantmentRecipes[currentRecipe], equipment);
                output += "\n";
            }

            if(enchantmentRecipes.Count > 0)
            {
                output += GetDetailedEnchantmentDescription(enchantmentRecipes[enchantmentRecipesMinusOne], equipment);
            }


            return output;
        }

        public static string GetDetailedEnchantmentDescription(EnchantmentRecipe enchantmentRecipe, Equipment equipment)
        {
            string output = "";

            if(enchantmentRecipe.GetHasMatchingEquipment(equipment))
            {
                Enchantment enchantment = ResourcesPrefabManager.Instance.GetEnchantmentPrefab(enchantmentRecipe.RecipeID);

                if (enchantment == null)
                    return output;

                output += $"Enchanting: {enchantment.Name} \n";
                output += ItemEnchantmentInformationHelper.BuildDescriptions(enchantment, equipment);
            }

            return output;
        }

        //includes pouch, backpack and equiped items
        public static List<EnchantmentRecipeDetailedData> GetUniqueAvailableEnchantmentRecipeDatasInInventory(Item item, CharacterInventory inventory)
        {
            List<Item> inventoryItems = ItemHelpers.GetUniqueItemsInInventory(inventory);
            List<EnchantmentRecipeItem> enchantments = ItemHelpers.GetAllItemsOfType<EnchantmentRecipeItem>(inventoryItems);
            Dictionary<int, EnchantmentRecipeDetailedData> haveEnchantments = new Dictionary<int, EnchantmentRecipeDetailedData>();

            foreach (EnchantmentRecipeItem enchantmentRecipeItem in enchantments)
            {
                foreach (EnchantmentRecipe enchantmentRecipe in enchantmentRecipeItem.Recipes)
                {
                    if (enchantmentRecipe.GetHasMatchingEquipment(item))
                    {
                        #if DEBUG
                        SL.Log($"{OutwardEnchantmentsViewer.prefix} equiment {item.Name} can be enchanted with {enchantmentRecipe.name} and {enchantmentRecipeItem.Name}, {enchantmentRecipeItem.name}");
                        #endif
                        if (haveEnchantments.TryGetValue(enchantmentRecipe.RecipeID, out var existing))
                        {
                            existing.Count++;
                        }
                        else
                        {
                            haveEnchantments[enchantmentRecipe.RecipeID] = new EnchantmentRecipeDetailedData(
                                new EnchantmentRecipeData(enchantmentRecipeItem, enchantmentRecipe), 1);
                        }
                    }
                }
            }

            return haveEnchantments.Values.ToList();
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
