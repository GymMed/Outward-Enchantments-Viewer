using OutwardEnchantmentsViewer.Enchantments;
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
using UnityEngine.Networking.Match;

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

        public void SetEquipmentsEnchantmentsDescription(Item item, CharacterInventory inventory, ItemDetailsDisplay itemDetailsDisplay)
        {
            List<EnchantmentRecipe> availableEnchantments = EnchantmentsHelper.GetAvailableEnchantmentRecipies(item);
            List<EnchantmentRecipeDetailedData> haveEnchantmentsDetailedDatas = EnchantmentsHelper.GetUniqueAvailableEnchantmentRecipeDatasInInventory(item, inventory);
            List<EnchantmentRecipe> haveEnchantments = haveEnchantmentsDetailedDatas
                .Select(data => data.Data.enchantmentRecipe)
                .ToList();

            string haveRecipesDescriptions = "";

            if (item is Equipment equipment && OutwardEnchantmentsViewer.ShowEquipmentOwnedEnchantmentsDetailed.Value)
            {
                haveRecipesDescriptions = EnchantmentsHelper.GetDetailedEnchantmentsDescriptions(haveEnchantmentsDetailedDatas, equipment);
            }
            else
            {
                haveRecipesDescriptions = EnchantmentsHelper.GetEnchantmentsDescriptions(haveEnchantmentsDetailedDatas);
            }

            bool startNewLineMissingRecipies = false;

            if(!string.IsNullOrEmpty(haveRecipesDescriptions))
            {
                haveRecipesDescriptions = "\n" + haveRecipesDescriptions;
            }
            else
            {
                startNewLineMissingRecipies = true;
            }

            string headerRightText = "Know Enchantments";

            string headerLeftText = haveEnchantments.Count.ToString();

            if(OutwardEnchantmentsViewer.ShowAllAvailableEnchantmentsCountForEquipment.Value)
            {
                headerLeftText = $"{headerLeftText}/{availableEnchantments.Count}";
            }

            ItemDisplayManager.Instance.SetHeaderText(
                itemDetailsDisplay, 
                headerRightText,
                headerLeftText
            );

            ItemDisplayManager.Instance.ShowDescriptionContainer(itemDetailsDisplay, String.IsNullOrEmpty(item.Description));
            ItemDisplayManager.Instance.SetDescriptionText(itemDetailsDisplay, haveRecipesDescriptions);

            if(!OutwardEnchantmentsViewer.ShowMissingEnchantmentsForEquipment.Value)
            {
                ItemDisplayManager.Instance.HideDisabledDescription(itemDetailsDisplay);
                return;
            }

            string missingRecipesDescriptions = startNewLineMissingRecipies ? "\n" : "";
            List<EnchantmentRecipe> missingEnchantments = EnchantmentsHelper.GetMissingEnchantments(availableEnchantments, haveEnchantments);

            if (item is Equipment equipmentItem && OutwardEnchantmentsViewer.ShowEquipmentUnownedEnchantmentsDetailed.Value)
            {
                missingRecipesDescriptions += EnchantmentsHelper.GetDetailedEnchantmentsDescriptions(missingEnchantments, equipmentItem);
            }
            else
            {
                missingRecipesDescriptions += EnchantmentsHelper.GetEnchantmentsDescriptions(missingEnchantments);
            }

            if(missingEnchantments.Count > 0)
            {
                ItemDisplayManager.Instance.SetDisabledDescriptionText(itemDetailsDisplay, missingRecipesDescriptions);
                ItemDisplayManager.Instance.ShowDisabledDescription(itemDetailsDisplay);
            }
            else
            {
                ItemDisplayManager.Instance.HideDisabledDescription(itemDetailsDisplay);
            }
        }

        public void SetEnchantmentsDescription(EnchantmentRecipeItem item, CharacterInventory inventory, ItemDetailsDisplay itemDetailsDisplay)
        {
            try
            {
                //List<Item> inventoryItems = GetUniqueItemsInInventory(inventory);
                string output = "\n";

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
                    output += GetEnchantmentInformation(enchantment, recipe);
                }

                ItemDisplayManager.Instance.SetHeaderText(
                    itemDetailsDisplay,
                    "Enchantment Properties",
                    $""
                );

                ItemDisplayManager.Instance.ShowDescriptionContainer(itemDetailsDisplay);
                ItemDisplayManager.Instance.SetDescriptionText(itemDetailsDisplay, output);
            }
            catch(Exception e)
            {
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@SetEnchantmentsDescription error: {e.Message}");
            }
        }

        public string GetEnchantmentRecipeEquipmentName(EnchantmentRecipe recipe)
        {
            string output = "";
            string type = EnchantmentsHelper.GetEnchantmentArmorIngrediantType(recipe);

            if (!String.IsNullOrEmpty(type))
            {
#if DEBUG
                output += $"{type} {recipe.name} {recipe.RecipeID}\n";
#else
                output += $"{type}\n";
#endif
            }

            return output;
        }

        public string GetDynamicEnchantmentInformationSection(Enchantment enchantment, EnchantmentRecipe recipe)
        {
            string output = "";

            output += GetEnchantmentRecipeEquipmentName(recipe);
            output += GetDynamicEnchantmentInformation(enchantment);

            return output;

        }

        public string GetEnchantmentInformation(Enchantment enchantment, EnchantmentRecipe recipe)
        {
            try
            {
                EnchantmentDescription enchantmentDescription = CustomEnchantmentsManager.Instance.TryGetDescription(recipe.RecipeID);

                if (enchantmentDescription == null)
                {
                    #if DEBUG
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@GetEnchantmentInformation recipe id: {recipe.RecipeID} couldn't retrieve description!");
                    #endif
                    return GetDynamicEnchantmentInformationSection(enchantment, recipe);
                }
                #if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@GetEnchantmentInformation recipe id: {recipe.RecipeID} retrieved description: {enchantmentDescription.description}!");
                #endif

                if (enchantmentDescription.overwrite)
                    return enchantmentDescription.description;

                string output = "";

                output += GetDynamicEnchantmentInformationSection(enchantment, recipe);
                output += enchantmentDescription.description;

                return output;
            }
            catch (Exception ex) 
            {
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@GetEnchantmentInformation error: {ex.Message}");
                return "Error";
            }
        }

        public string GetDynamicEnchantmentInformation(Enchantment enchantment)
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

            if (!string.IsNullOrWhiteSpace(enchantment.Description))
            {
                output += $"{enchantment.Description} \n\n";
            }

            output += CustomEnchantmentsDescriptionsExtensions.GetDescription(enchantment.PresetID);

            if(output == "")
            {
                return "Doesn't give stats in traditional way and uses unknown properties";
            }

            return output;
        }
    }
}
