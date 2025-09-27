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
        public struct EnchantmentInformationData
        {
            public string DynamicDescription { get; set; }
            public string EquipmentType { get; set; }

            public EnchantmentInformationData(string dynamicDescription, string equipmentType)
            {
                DynamicDescription = dynamicDescription;
                EquipmentType = equipmentType;
            }
        }
        
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
                Enchantment enchantment = null;
                List<EnchantmentInformationData> enchantmentInformations = new List<EnchantmentInformationData>();

                foreach (EnchantmentRecipe currentRecipe in item.Recipes)
                {
                    enchantment = ResourcesPrefabManager.Instance.GetEnchantmentPrefab(currentRecipe.RecipeID);

                    if (enchantment == null)
                    {
#if DEBUG
                        SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@SetEnchantmentsDescription couldn't retrieve enchantment from RecipeID");
#endif
                        return;
                    }
#if DEBUG
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@SetEnchantmentsDescription got enchantment {enchantment?.Name} RecipeID {currentRecipe.RecipeID} ResultID {currentRecipe.ResultID}");
#endif
                    EnchantmentInformationData enchantmentData = GetEnchantmentInformation(enchantment, currentRecipe);
                    enchantmentInformations.Add(enchantmentData);

                    //output += $"{enchantmentData.EquipmentType}{enchantmentData.DynamicDescription}\n";
                }

                List<string> equipments = GetEquipmentTypesFromEnchantmentItem(item);
                string equipmentName = "";
                int enchantmentInformationsCountMinusOne = enchantmentInformations.Count - 1;

                for(int currentRecipe = 0; currentRecipe < item.Recipes.Length; currentRecipe++)
                {
                    if(currentRecipe > enchantmentInformationsCountMinusOne)
                    {
                        output += "Error: could not retrieve that many enchantment informations as there are recipes!";
                        continue;
                    }

                    if (string.IsNullOrEmpty(enchantmentInformations[currentRecipe].EquipmentType))
                    {
                        if (currentRecipe < equipments.Count)
                        {
                            equipmentName = equipments[currentRecipe];
                            output += $"{equipmentName}?\n{enchantmentInformations[currentRecipe].DynamicDescription}\n";
                        }
                        else
                        {
                            equipmentName = "Undetermined Equipment";
                            output += $"{equipmentName}\n{enchantmentInformations[currentRecipe].DynamicDescription}\n";
#if DEBUG
                            SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@SetEnchantmentsDescription Unknown Equipment recipes:{item.Recipes.Length}" +
                            "-" + string.Join("|", equipments));
#endif
                        }
                    }
                    else
                    {
                        output += $"{enchantmentInformations[currentRecipe].EquipmentType}{enchantmentInformations[currentRecipe].DynamicDescription}\n";
                    }
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

        public List<string> GetEquipmentTypesFromEnchantmentItem(EnchantmentRecipeItem enchantmentItem)
        {
            string[] fullItemsNames = ItemDescriptionsManager.GetEquipmentsFromEnchantmentItem(enchantmentItem);
            List<string> finalNames = new List<string>();
            string[] parts = new string[0];
            string receivedEquipmentType = "";

            foreach(string itemName in fullItemsNames)
            {
                parts = itemName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

#if DEBUG
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@GetEquipmentTypesFromEnchantmentItem description itemName: {itemName}");
                    string combinedParts = "";
                    foreach(string part in parts)
                    {
                        combinedParts += "[" + part + "]";
                    }
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@GetEquipmentTypesFromEnchantmentItem description parts: {combinedParts}");
#endif

                if(parts.Length > 1)
                {
                    receivedEquipmentType = getEquipmentTypeString(parts[parts.Length - 1]);
                    if (string.IsNullOrEmpty(receivedEquipmentType))
                    {
                        finalNames.Add(itemName);
                        continue;
                    }
                    finalNames.Add(receivedEquipmentType);
                    continue;
                }

                receivedEquipmentType = getEquipmentTypeString(itemName);
                if (string.IsNullOrEmpty(receivedEquipmentType))
                {
                    finalNames.Add(itemName);
                    continue;
                }
                //provides full item name instead of type
                finalNames.Add(receivedEquipmentType);
                continue;
            }

            return finalNames;
        }

        public static string getEquipmentTypeString(string equipmentType)
        {
            switch(equipmentType.ToLower())
            {
                case "helm":
                case "helmet":
                    {
                        return "Helmet";
                    }
                case "armor":
                case "chest":
                    {
                        return "Chest";
                    }
                case "boots":
                    {
                        return "Boots";
                    }
                case "weapon":
                    {
                        return "Weapon";
                    }
                case "backpack":
                    {
                        return "Backpack";
                    }
                default:
                    {
                        return "";
                    }
            }
        }

        public static string[] GetEquipmentsFromEnchantmentItem(EnchantmentRecipeItem enchantmentItem)
        {
            var matches = Regex.Matches(enchantmentItem.Description, @"Equipment:\s*(.*)");

            if (matches.Count > 0)
            {
                // Use Group[1] → only the part after "Equipment:"
                return matches[0]
                    .Groups[1]
                    .Value
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())   // remove leading/trailing spaces
                    .ToArray();
            }

            return Array.Empty<string>();
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

        public EnchantmentInformationData GetDynamicEnchantmentInformationSection(Enchantment enchantment, EnchantmentRecipe recipe)
        {
            return new EnchantmentInformationData(GetDynamicEnchantmentInformation(enchantment), GetEnchantmentRecipeEquipmentName(recipe));
        }

        public EnchantmentInformationData GetEnchantmentInformation(Enchantment enchantment, EnchantmentRecipe recipe)
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
                    return new EnchantmentInformationData(enchantmentDescription.description, "");

                EnchantmentInformationData dynamicEnchantmentData = GetDynamicEnchantmentInformationSection(enchantment, recipe);
                string fixedDescription = "";

                if (enchantmentDescription.description.StartsWith("\n"))
                    fixedDescription = enchantmentDescription.description.TrimStart('\n');
                else
                    fixedDescription = enchantmentDescription.description;

                return new EnchantmentInformationData(fixedDescription + dynamicEnchantmentData.DynamicDescription, dynamicEnchantmentData.EquipmentType);
            }
            catch (Exception ex) 
            {
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@GetEnchantmentInformation error: {ex.Message}");
                return new EnchantmentInformationData("Error", "");
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
                output += $"Global Status Resistance {enchantment.GlobalStatusResistance.ToString()} \n";
            }

            if (enchantment.HealthAbsorbRatio > 0.0f)
            {
                //output += $"Health Absorb Ratio {enchantment.HealthAbsorbRatio.ToString()} \n";
                output += $"Gain +{enchantment.HealthAbsorbRatio.ToString()}x Health Leech (damage " +
                    $"dealth will restore {enchantment.HealthAbsorbRatio.ToString()}x the damage as Health) \n";
            }

            if (enchantment.Indestructible)
            {
                output += $"Provides indestructibility \n";
            }

            if (enchantment.ManaAbsorbRatio > 0.0f)
            {
                //output += $"Mana Absorb Ratio {enchantment.ManaAbsorbRatio.ToString()} \n";
                output += $"Gain +{enchantment.ManaAbsorbRatio.ToString()}x Mana Leech (damage " +
                    $"dealth will restore {enchantment.ManaAbsorbRatio.ToString()}x the damage as Mana) \n";
            }

            if (enchantment.StaminaAbsorbRatio > 0.0f)
            {
                //output += $"Stamina Absorb Ratio {enchantment.StaminaAbsorbRatio.ToString()} \n";
                output += $"Gain +{enchantment.StaminaAbsorbRatio.ToString()}x Stamina Leech (damage " +
                    $"dealth will restore {enchantment.StaminaAbsorbRatio.ToString()}x the damage as Stamina) \n";
            }

            if (enchantment.TrackDamageRatio > 0.0f)
            {
                output += $"Track Damage Ratio {enchantment.TrackDamageRatio.ToString()} \n";
            }

            if (!string.IsNullOrWhiteSpace(enchantment.Description))
            {
                output += $"{enchantment.Description} \n";
            }

            output += CustomEnchantmentsDescriptionsExtensions.GetDescription(enchantment.PresetID);

            if(output == "")
            {
                return "Doesn't give stats in traditional way and uses unknown properties \n";
            }

            return output;
        }
    }
}
