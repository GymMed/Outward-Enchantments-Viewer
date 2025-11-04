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

                output += GetEnchantmentDescription(item);

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

        public string GetEnchantmentDescription(EnchantmentRecipeItem item)
        {
            string output = "";

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
                    continue;
                }
#if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@SetEnchantmentsDescription got enchantment {enchantment?.Name} RecipeID {currentRecipe.RecipeID} ResultID {currentRecipe.ResultID}");
#endif
                EnchantmentInformationData enchantmentData = GetEnchantmentInformation(enchantment, currentRecipe);
                enchantmentInformations.Add(enchantmentData);

                //output += $"{enchantmentData.EquipmentType}{enchantmentData.DynamicDescription}\n";
            }

            if(OutwardEnchantmentsViewer.ShowInShopEnchantmentWithEquipmentType.Value == true)
            {
                output += GetEnchantmentDescriptionWithEquipmentType(item, enchantmentInformations);
            }
            else
            {

                foreach (EnchantmentInformationData informationData in enchantmentInformations)
                {
                    output += $"Unknown Equipment\n{informationData.DynamicDescription}\n";
                }
            }

            return output;
        }

        public static string GetEnchantmentDescriptionWithEquipmentType(EnchantmentRecipeItem item, List<EnchantmentInformationData> enchantmentInformations)
        {
            string output = "";
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

            return output;
        }

        public static List<string> GetEquipmentTypesFromEnchantmentItem(EnchantmentRecipeItem enchantmentItem)
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
            string enchantmentDescription = GetLocalizedEnchantmentDescription(enchantmentItem);
#if DEBUG
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@GetEquipmentsFromEnchantmentItem enchantmentDescription: {enchantmentDescription}");
#endif

            var matches = Regex.Matches(enchantmentDescription, @"Equipment:\s*(.*)");

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
        
        public static string GetLocalizedEnchantmentDescription(EnchantmentRecipeItem item)
        {
            string output = "";

            string shopDescription = null;

            if (!string.IsNullOrEmpty(item.m_shopLocalizedDescription))
            {
                shopDescription = item.m_shopLocalizedDescription;
                item.m_shopLocalizedDescription = null;
            }

            if (LocalizationManager.Instance.Loaded)
            {
                if (item.m_lastDescLang != LocalizationManager.Instance.CurrentLanguage)
                {
                    item.m_lastDescLang = LocalizationManager.Instance.CurrentLanguage;
                }
				output = GetProcessedEnchantmentItemDescription(item);
            }

            if(!string.IsNullOrEmpty(shopDescription))
            {
                item.m_shopLocalizedDescription = shopDescription;
            }

            return output;
        }

        //original game method but it retrieves string instead of assigning it to item.m_localizedDescription
        public static string GetProcessedEnchantmentItemDescription(EnchantmentRecipeItem item)
        {
            if (item.Recipes.Length < 1)
            {
                return item.Description;
            }
            StringBuilder stringBuilder = new StringBuilder();
            string loc = LocalizationManager.Instance.GetLoc("ItemTag_Incense");
            stringBuilder.AppendLine(loc + ": " + item.Recipes[0].PillarDatas[0].CompatibleIngredients[0].SpecificIngredient.Name);
            stringBuilder.AppendLine("");
            StringBuilder stringBuilder2 = new StringBuilder();
            for (int i = 0; i < item.Recipes[0].PillarDatas.Length; i++)
            {
                if (i > 0)
                {
                    stringBuilder2.Append(", ");
                }
                stringBuilder2.Append(LocalizationManager.Instance.GetLoc(string.Format("Compass_{0}", item.Recipes[0].PillarDatas[i].Direction)));
                if (item.Recipes[0].PillarDatas[i].IsFar)
                {
                    stringBuilder2.Append(LocalizationManager.Instance.GetLoc("EnchantmentCondition_PillarFar"));
                }
                else
                {
                    stringBuilder2.Append(LocalizationManager.Instance.GetLoc("EnchantmentCondition_PillarClose"));
                }
            }
            stringBuilder.AppendLine(LocalizationManager.Instance.GetLoc("EnchantmentCondition_PillarPlacement") + ": " + stringBuilder2.ToString());
            stringBuilder.AppendLine("");
            bool flag = true;
            for (int j = 0; j < item.Recipes.Length; j++)
            {
                for (int k = 0; k < item.Recipes[j].CompatibleEquipments.CompatibleEquipments.Length; k++)
                {
                    if (item.Recipes[j].CompatibleEquipments.CompatibleEquipments[k].Type == EnchantmentRecipe.IngredientData.IngredientType.Generic)
                    {
                        flag = false;
                        break;
                    }
                }
                if (!flag)
                {
                    break;
                }
            }
            StringBuilder stringBuilder3 = new StringBuilder();
            HashSet<string> hashSet = new HashSet<string>();
            for (int l = 0; l < item.Recipes.Length; l++)
            {
                if (l > 0)
                {
                    stringBuilder3.Append(", ");
                }
                stringBuilder3.Append(LocalizationManager.Instance.GetLoc("ItemTag_" + item.Recipes[l].CompatibleEquipments.EquipmentTag.Tag.TagName.Replace(" ", "")));
                for (int m = 0; m < item.Recipes[l].CompatibleEquipments.CompatibleEquipments.Length; m++)
                {
                    if (item.Recipes[l].CompatibleEquipments.CompatibleEquipments[m].Type == EnchantmentRecipe.IngredientData.IngredientType.Generic)
                    {
                        string text = "";
                        if (item.Recipes[l].CompatibleEquipments.CompatibleEquipments[m].IngredientTag.Tag == item.Recipes[l].CompatibleEquipments.EquipmentTag.Tag)
                        {
                            text = text + LocalizationManager.Instance.GetLoc("EnchantmentCondition_AnyTag") + " ";
                        }
                        text += LocalizationManager.Instance.GetLoc("ItemTag_" + item.Recipes[l].CompatibleEquipments.CompatibleEquipments[m].IngredientTag.Tag.TagName.Replace(" ", ""));
                        hashSet.Add(text);
                    }
                    else if (item.Recipes[l].CompatibleEquipments.CompatibleEquipments[m].SpecificIngredient != null)
                    {
                        hashSet.Add(LocalizationManager.Instance.GetItemName(item.Recipes[l].CompatibleEquipments.CompatibleEquipments[m].SpecificIngredient.ItemID));
                    }
                }
            }
            StringBuilder stringBuilder4 = new StringBuilder();
            int num = 0;
            foreach (string value in hashSet)
            {
                if (num >= 1)
                {
                    stringBuilder4.Append(", ");
                }
                stringBuilder4.Append(value);
                num++;
            }
            if (!flag)
            {
                stringBuilder.AppendLine(LocalizationManager.Instance.GetLoc("EnchantmentCondition_Equipment") + ": " + stringBuilder3.ToString());
                stringBuilder.AppendLine(LocalizationManager.Instance.GetLoc("EnchantmentCondition_Type") + ": " + stringBuilder4.ToString());
            }
            else
            {
                stringBuilder.AppendLine(LocalizationManager.Instance.GetLoc("EnchantmentCondition_Equipment") + ": " + stringBuilder4.ToString());
            }
            stringBuilder.AppendLine("");
            bool flag2 = false;
            StringBuilder stringBuilder5 = new StringBuilder();
            for (int n = 0; n < item.Recipes[0].TimeOfDay.Length; n++)
            {
                if (flag2)
                {
                    stringBuilder5.Append(", ");
                }
                if (item.Recipes[0].TimeOfDay[n].x == 18f && item.Recipes[0].TimeOfDay[n].y == 5f)
                {
                    stringBuilder5.Append(LocalizationManager.Instance.GetLoc("TimeOfDay_Night"));
                }
                else if (item.Recipes[0].TimeOfDay[n].x == 5f && item.Recipes[0].TimeOfDay[n].y == 18f)
                {
                    stringBuilder5.Append(LocalizationManager.Instance.GetLoc("TimeOfDay_Day"));
                }
                else
                {
                    stringBuilder5.Append(string.Format("Between {0} and {1}", item.Recipes[0].TimeOfDay[n].x, item.Recipes[0].TimeOfDay[n].y));
                }
                flag2 = true;
            }
            if (item.Recipes[0].Temperature.Contains(TemperatureSteps.Cold))
            {
                if (flag2)
                {
                    stringBuilder5.Append(", ");
                }
                stringBuilder5.Append(LocalizationManager.Instance.GetLoc("EnchantmentCondition_ColdWeather"));
                flag2 = true;
            }
            if (item.Recipes[0].Temperature.Contains(TemperatureSteps.Hot))
            {
                if (flag2)
                {
                    stringBuilder5.Append(", ");
                }
                stringBuilder5.Append(LocalizationManager.Instance.GetLoc("EnchantmentCondition_HotWeather"));
                flag2 = true;
            }
            if (item.Recipes[0].WindAltarActivated)
            {
                if (flag2)
                {
                    stringBuilder5.Append(", ");
                }
                stringBuilder5.Append(LocalizationManager.Instance.GetLoc("EnchantmentCondition_WindAltar"));
                flag2 = true;
            }
            if (item.Recipes[0].Weather.Length != 0)
            {
                for (int num2 = 0; num2 < item.Recipes[0].Weather.Length; num2++)
                {
                    if (flag2)
                    {
                        stringBuilder5.Append(", ");
                    }
                    switch (item.Recipes[0].Weather[num2].Weather)
                    {
                    case EnchantmentRecipe.WeaterType.Clear:
                        stringBuilder5.Append(LocalizationManager.Instance.GetLoc("EnchantmentCondition_WeatherClear"));
                        flag2 = true;
                        break;
                    case EnchantmentRecipe.WeaterType.Rain:
                        stringBuilder5.Append(LocalizationManager.Instance.GetLoc("EnchantmentCondition_WeatherRain"));
                        flag2 = true;
                        break;
                    case EnchantmentRecipe.WeaterType.Snow:
                        stringBuilder5.Append(LocalizationManager.Instance.GetLoc("EnchantmentCondition_WeatherSnow"));
                        flag2 = true;
                        break;
                    case EnchantmentRecipe.WeaterType.SeasonEffect:
                        stringBuilder5.Append(LocalizationManager.Instance.GetLoc("EnchantmentCondition_WeatherFog"));
                        flag2 = true;
                        break;
                    }
                }
            }
            if (flag2)
            {
                stringBuilder.AppendLine(LocalizationManager.Instance.GetLoc("EnchantmentCondition_ExtraConditions") + ": " + stringBuilder5.ToString());
            }
            if (string.IsNullOrEmpty(item.Recipes[0].OverrideRegionLocKey))
            {
                if (item.Recipes[0].Region != null && item.Recipes[0].Region.Length != 0)
                {
                    StringBuilder stringBuilder6 = new StringBuilder();
                    if (item.Recipes[0].Region.SequenceEqualExt(AreaManager.Regions))
                    {
                        stringBuilder6.Append(LocalizationManager.Instance.GetLoc("EnchantmentCondition_Outdoors"));
                    }
                    else if (item.Recipes[0].Region.SequenceEqualExt(AreaManager.TrogDungeons))
                    {
                        stringBuilder6.Append(LocalizationManager.Instance.GetLoc("EnchantmentCondition_TrogDungeons"));
                    }
                    else
                    {
                        for (int num3 = 0; num3 < item.Recipes[0].Region.Length; num3++)
                        {
                            if (num3 >= 1)
                            {
                                stringBuilder6.Append(", ");
                            }
                            stringBuilder6.Append(AreaManager.Instance.GetAreaName(item.Recipes[0].Region[num3]));
                        }
                    }
                    stringBuilder.AppendLine(LocalizationManager.Instance.GetLoc("EnchantmentCondition_Locations") + ": " + stringBuilder6.ToString());
                }
            }
            else
            {
                string str;
                if (item.Recipes[0].OverrideRegionLocKey.StartsWith("BuildingID_"))
                {
                    int itemID;
                    if (int.TryParse(item.Recipes[0].OverrideRegionLocKey.Replace("BuildingID_", ""), out itemID))
                    {
                        str = LocalizationManager.Instance.GetItemName(itemID);
                    }
                    else
                    {
                        str = LocalizationManager.Instance.GetLoc(item.Recipes[0].OverrideRegionLocKey);
                    }
                }
                else
                {
                    str = LocalizationManager.Instance.GetLoc(item.Recipes[0].OverrideRegionLocKey);
                }
                stringBuilder.AppendLine(LocalizationManager.Instance.GetLoc("EnchantmentCondition_Locations") + ": " + str);
            }
            return stringBuilder.ToString();
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

                return GetDynamicEnchantmentInformationSection(enchantment, recipe);
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

            output += EnchantmentsHelper.GetModifiedEnchantmentDescription(enchantment);

            if(output == "")
            {
                return "Doesn't give stats in traditional way and uses unknown properties \n";
            }

            return output;
        }
    }
}
