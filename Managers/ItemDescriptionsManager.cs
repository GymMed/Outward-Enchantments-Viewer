using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutwardEnchanmentsViewer.Managers
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
            List<EnchantmentRecipe> enchantmentRecipes = RecipeManager.Instance.GetEnchantmentRecipes();
            List<EnchantmentRecipe> availableEnchantments = new List<EnchantmentRecipe>();

            foreach (EnchantmentRecipe enchantmentRecipe in enchantmentRecipes)
            {
                if (enchantmentRecipe.GetHasMatchingEquipment(item))
                {
                    SL.Log($"{OutwardEnchanmentsViewer.prefix} ItemDescriptionsManager@SetEquipmentsEnchantmentsDescription equiment {item.Name} can be enchanted with {enchantmentRecipe.name}");
                    availableEnchantments.Add(enchantmentRecipe);
                }
            }

            string headerText = "Item doesn't have any available enchantments!";
            string unlockedEnchantments = "";
            string haveRecipesNames = "";

            if (availableEnchantments.Count > 0)
            {
                List<Item> inventoryItems = GetUniqueItemsInInventory(inventory);
                List<EnchantmentRecipeItem> enchantments = OutwardEnchanmentsViewer.GetAllItemsOfType<EnchantmentRecipeItem>(inventoryItems);
                List<EnchantmentRecipe> haveEnchantments = new List<EnchantmentRecipe>();

                foreach (EnchantmentRecipeItem enchantmentRecipeItem in enchantments)
                {
                    foreach (EnchantmentRecipe enchantmentRecipe in enchantmentRecipeItem.Recipes)
                    {
                        if (enchantmentRecipe.GetHasMatchingEquipment(item))
                        {
                            SL.Log($"{OutwardEnchanmentsViewer.prefix} equiment {item.Name} can be enchanted with {enchantmentRecipe.name} and {enchantmentRecipeItem.Name}, {enchantmentRecipeItem.name}");
                            haveEnchantments.Add(enchantmentRecipe);
                            haveRecipesNames += $"{enchantmentRecipeItem.Name} \n";
                        }
                    }
                }

                headerText = "Unlocked Enchantments";
                unlockedEnchantments = $"{haveEnchantments.Count}/{availableEnchantments.Count}";
            }

            ItemDisplayManager.Instance.SetHeaderText(
                characterUI, 
                headerText,
                unlockedEnchantments
            );
            ItemDisplayManager.Instance.SetDescriptionText(characterUI, haveRecipesNames);
            ItemDisplayManager.Instance.ShowDescription(characterUI, String.IsNullOrEmpty(item.Description));
        }

        public void SetEnchantmentsDescription(EnchantmentRecipeItem item, CharacterInventory inventory, CharacterUI characterUI)
        {
            //List<Item> inventoryItems = GetUniqueItemsInInventory(inventory);
            Enchantment enchantment = ResourcesPrefabManager.Instance.GetEnchantmentPrefab(item.Recipes[0].RecipeID);

            if (enchantment == null) 
            { 
                SL.Log($"{OutwardEnchanmentsViewer.prefix} ItemDescriptionsManager@SetEnchantmentsDescription couldn't retrieve enchantment from RecipeID");
                return;
            }
            SL.Log($"{OutwardEnchanmentsViewer.prefix} ItemDescriptionsManager@SetEnchantmentsDescription got enchantment {enchantment?.Name}");
            string output = "";

            if (enchantment.DamageBonus.Count > 0)
            {
                output += $"Damage Bonus \n";

                foreach (DamageType damageBonus in enchantment.DamageBonus.List)
                {
                    string damageGrowthWord = "Lose ";

                    if (damageBonus.Damage > 0)
                        damageGrowthWord = "Gain +";

                    output += $"{damageGrowthWord}{damageBonus.Damage.ToString()} flat {damageBonus.Type.ToString()} damage \n";
                    //output += $"{damageBonus.Type.ToString()} \n";
                    //output += $"+{damageBonus.Damage.ToString()}% \n";
                }
            }

            if (enchantment.DamageModifier.Count > 0)
            {
                output += $"Damage Modifiers \n";

                foreach (DamageType damageModifier in enchantment.DamageModifier.List)
                {
                    string statusGrowthWord = "Reduces";

                    if (damageModifier.Damage > 0)
                        statusGrowthWord = "Increases";

                    output += $"{statusGrowthWord} {damageModifier.Type.ToString()} by {damageModifier.Damage.ToString()}% \n";
                    //output += $"{damageModifier.Type.ToString()} \n";
                    ////output += $"+{damageModifier.Damage.ToString()} \n";
                }

                output += "\n";
            }

            if (enchantment.AdditionalDamages.Count() > 0)
            {
                output += $"Additional Damages \n";

                foreach (Enchantment.AdditionalDamage additionalDamage in enchantment.AdditionalDamages)
                {
                    string damageGrowthWord = "Removes ";

                    if (additionalDamage.ConversionRatio > 0)
                        damageGrowthWord = "Adds +";

                    output += $"{damageGrowthWord}{(int)(additionalDamage.ConversionRatio * 100)}% of the existing weapon's {additionalDamage.SourceDamageType.ToString()}" +
                        $" damage as {additionalDamage.BonusDamageType.ToString()} damage \n";
                    //output += $"{additionalDamage.SourceDamageType.ToString()} \n";
                    //output += $"{additionalDamage.ConversionRatio.ToString()} \n";
                    //output += $"{additionalDamage.BonusDamageType.ToString()} \n\n";
                }
                output += "\n";
            }

            if (enchantment.Effects.Count() > 0)
            {
                output += $"Effects \n";

                foreach (Effect effect in enchantment.Effects)
                {
                    output += $"Weapon now inflicts {effect.EffectType.Name.ToString()} ({effect.BasePotencyValue.ToString()}% {effect.CalculatePotency().ToString()} buildup) \n";
                }

                output += "\n";
            }

            if (enchantment.StatModifications.Count > 0)
            {
                output += $"Stat Modifications \n";


                foreach (Enchantment.StatModification statModification in enchantment.StatModifications)
                {
                    //split PascalCase with spaces
                    string statModificationName = Regex.Replace(statModification.Name.ToString(), "(?<!^)([A-Z])", " $1");
                    string statGrowthWord = "Removes ";

                    if (statModification.Value > 0)
                        statGrowthWord = "Adds +";

                    output += $"{statGrowthWord}{statModification.Value.ToString()} {statModification.Type.ToString()} {statModificationName} \n";
                    //output += $"{statModification.Type.ToString()} \n";
                    //output += $"{statModificationName} \n";
                    //output += $"{statModification.Value.ToString()} \n\n";
                    //mana
                    //output += $"Gain passive +{statModification.Value.ToString()} {statModification.Type.ToString()} regeneration per second";
                    //attack speed bonus
                    //output += $"Gain +{statModification.Value.ToString()} flat {statModification.Type.ToString()}";
                }
                output += "\n";
            }

            if(enchantment.ElementalResistances.Count > 0)
            {
                output += $"Elemental Resistances \n";

                foreach (DamageType elementalResistance in enchantment.ElementalResistances.List)
                {
                    string  resistenceGrowthString= "Removes ";

                    if (elementalResistance.Damage > 0)
                        resistenceGrowthString = "Adds +";

                    output += $"{resistenceGrowthString}{elementalResistance.Damage.ToString()}% {elementalResistance.Type.ToString()} \n";
                    //output += $"{elementalResistance.Type.ToString()} \n";
                    //output += $"{elementalResistance.Damage.ToString()} \n";
                }
            }

            if(enchantment.GlobalStatusResistance > 0.0f)
            {
                output += $"Global Status Resistance {enchantment.GlobalStatusResistance.ToString()} \n";
            }

            if(enchantment.HealthAbsorbRatio > 0.0f)
            {
                //output += $"Health Absorb Ratio {enchantment.HealthAbsorbRatio.ToString()} \n";
                output += $"Gain +{enchantment.HealthAbsorbRatio.ToString()}x Health Leech (damage " +
                    $"dealth will restore {enchantment.HealthAbsorbRatio.ToString()}x the damage as Health) \n";
            }

            if(enchantment.Indestructible)
            {
                output += $"Provides indestructibility \n";
            }

            if(enchantment.ManaAbsorbRatio > 0.0f)
            {
                //output += $"Mana Absorb Ratio {enchantment.ManaAbsorbRatio.ToString()} \n";
                output += $"Gain +{enchantment.ManaAbsorbRatio.ToString()}x Mana Leech (damage " +
                    $"dealth will restore {enchantment.ManaAbsorbRatio.ToString()}x the damage as Mana) \n";
            }

            if(enchantment.StaminaAbsorbRatio > 0.0f)
            {
                //output += $"Stamina Absorb Ratio {enchantment.StaminaAbsorbRatio.ToString()} \n";
                output += $"Gain +{enchantment.StaminaAbsorbRatio.ToString()}x Stamina Leech (damage " +
                    $"dealth will restore {enchantment.StaminaAbsorbRatio.ToString()}x the damage as Stamina) \n";
            }

            if(enchantment.TrackDamageRatio > 0.0f)
            {
                output += $"Track Damage Ratio {enchantment.TrackDamageRatio.ToString()} \n";
            }

            ItemDisplayManager.Instance.SetHeaderText(
                characterUI, 
                "Enchantment Properties",
                $""
            );

            

            ItemDisplayManager.Instance.SetDescriptionText(characterUI, output);
            ItemDisplayManager.Instance.ShowDescription(characterUI);
        }

        private List<Item> GetUniqueItemsInInventory(CharacterInventory inventory)
        {
            List<Item> pouchItems = inventory.Pouch?.GetContainedItems();
            List<Item> bagItems = new List<Item>();

            if (inventory.HasABag)
            {
                bagItems = inventory.EquippedBag.Container.GetContainedItems();
            }
            //List<Item> equipedItems = inventory.Equipment.EquipmentSlots.GetContainedItems();

            return pouchItems.Union(bagItems).ToList();
        }
    }
}
