﻿using OutwardEnchantmentsViewer.Enchantments;
using OutwardEnchantmentsViewer.Utility.Enums;
using OutwardEnchantmentsViewer.Utility.Helpers;
using SideLoader;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutwardEnchantmentsViewer.Managers
{
    public class ItemDescriptionsManager
    {
        private static ItemDescriptionsManager _instance;

        private ItemDescriptionsManager()
        {
        }

        public static ItemDescriptionsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ItemDescriptionsManager();

                return _instance;
            }
        }

        public void SetEquipmentsEnchantmentsDescription(Item item, CharacterInventory inventory, CharacterUI characterUI)
        {
            List<EnchantmentRecipe> availableEnchantments = EnchantmentsHelper.GetAvailableEnchantmentRecipies(item);
            List<EnchantmentRecipeData> haveEnchantmentsDatas = EnchantmentsHelper.GetAvailableEnchantmentRecipeDatasInInventory(item, inventory);
            List<EnchantmentRecipe> haveEnchantments = haveEnchantmentsDatas
                .Select(data => data.enchantmentRecipe)
                .ToList();

            string haveRecipesDescriptions = EnchantmentsHelper.GetEnchantmentsDescriptions(haveEnchantmentsDatas);

            string headerText = "Unlocked Enchantments";
            string unlockedEnchantments = $"{haveEnchantments.Count}/{availableEnchantments.Count}";

            ItemDisplayManager.Instance.SetHeaderText(
                characterUI, 
                headerText,
                unlockedEnchantments
            );

            ItemDisplayManager.Instance.SetDescriptionText(characterUI, haveRecipesDescriptions);
            ItemDisplayManager.Instance.ShowDescription(characterUI, String.IsNullOrEmpty(item.Description));

            List<EnchantmentRecipe> missingEnchantments = EnchantmentsHelper.GetMissingEnchantments(availableEnchantments, haveEnchantments);
            string missingRecipesDescriptions = EnchantmentsHelper.GetEnchantmentsDescriptions(missingEnchantments);

            if(missingEnchantments.Count > 0)
            {
                ItemDisplayManager.Instance.SetDisabledDescriptionText(characterUI, missingRecipesDescriptions);
                ItemDisplayManager.Instance.ShowDisabledDescription(characterUI);
            }
            else
            {
                ItemDisplayManager.Instance.HideDisabledDescription(characterUI);
            }
        }

        public void SetEnchantmentsDescription(EnchantmentRecipeItem item, CharacterInventory inventory, CharacterUI characterUI)
        {
            try
            {
                //List<Item> inventoryItems = GetUniqueItemsInInventory(inventory);
                string output = "";

                foreach (EnchantmentRecipe recipe in item.Recipes)
                {
                    Enchantment enchantment = ResourcesPrefabManager.Instance.GetEnchantmentPrefab(recipe.RecipeID);

                    if (enchantment == null)
                    {
#if DEBUG
                        SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@SetEnchantmentsDescription couldn't retrieve enchantment from RecipeID");
#endif
                        return;
                    }
#if DEBUG
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@SetEnchantmentsDescription got enchantment {enchantment?.Name} RecipeID {recipe.RecipeID} ResultID {recipe.ResultID}");
#endif
                    string type = EnchantmentsHelper.GetEnchantmentArmorIngrediantType(recipe);

                    if (!String.IsNullOrEmpty(type))
                    {
#if DEBUG
                        output += $"{type} {recipe.name} {recipe.RecipeID}\n";
#else
                        output += $"{type}\n";
#endif
                    }
                    output += GetEnchantmentInformation(enchantment);
                    output += CustomEnchantmentsDescriptionsExtensions.GetDescription(recipe.RecipeID);
                }

                ItemDisplayManager.Instance.SetHeaderText(
                    characterUI,
                    "Enchantment Properties",
                    $""
                );

                ItemDisplayManager.Instance.SetDescriptionText(characterUI, output);
                ItemDisplayManager.Instance.ShowDescription(characterUI);
            }
            catch(Exception e)
            {
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@SetEnchantmentsDescription error: {e.Message}");
            }
        }

        public string GetEnchantmentInformation(Enchantment enchantment)
        {
            try
            {
                string output = "";

                output += EnchantmentInformationHelper.GetDamageListDescription(enchantment);
                output += EnchantmentInformationHelper.GetModifiersListDescriptions(enchantment);
                output += EnchantmentInformationHelper.GetAdditionalDamagesDescription(enchantment);
                output += EnchantmentInformationHelper.GetEffectsDescription(enchantment);
                output += EnchantmentInformationHelper.GetStatModificationsDescription(enchantment);
                output += EnchantmentInformationHelper.GetElementalResistancesDescription(enchantment);

                if (enchantment.GlobalStatusResistance > 0.0f)
                {
                    output += $"Global Status Resistance {enchantment.GlobalStatusResistance.ToString()} \n\n";
                }

                if (enchantment.HealthAbsorbRatio > 0.0f)
                {
                    //output += $"Health Absorb Ratio {enchantment.HealthAbsorbRatio.ToString()} \n";
                    output += $"Gain +{enchantment.HealthAbsorbRatio.ToString()}x Health Leech (damage " +
                        $"dealth will restore {enchantment.HealthAbsorbRatio.ToString()}x the damage as Health) \n\n";
                }

                if (enchantment.Indestructible)
                {
                    output += $"Provides indestructibility \n";
                }

                if (enchantment.ManaAbsorbRatio > 0.0f)
                {
                    //output += $"Mana Absorb Ratio {enchantment.ManaAbsorbRatio.ToString()} \n";
                    output += $"Gain +{enchantment.ManaAbsorbRatio.ToString()}x Mana Leech (damage " +
                        $"dealth will restore {enchantment.ManaAbsorbRatio.ToString()}x the damage as Mana) \n\n";
                }

                if (enchantment.StaminaAbsorbRatio > 0.0f)
                {
                    //output += $"Stamina Absorb Ratio {enchantment.StaminaAbsorbRatio.ToString()} \n";
                    output += $"Gain +{enchantment.StaminaAbsorbRatio.ToString()}x Stamina Leech (damage " +
                        $"dealth will restore {enchantment.StaminaAbsorbRatio.ToString()}x the damage as Stamina) \n\n";
                }

                if (enchantment.TrackDamageRatio > 0.0f)
                {
                    output += $"Track Damage Ratio {enchantment.TrackDamageRatio.ToString()} \n\n";
                }

                return output;
            }
            catch (Exception ex) 
            {
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@GetEnchantmentInformation error: {ex.Message}");
                return "Error";
            }
        }
    }
}
