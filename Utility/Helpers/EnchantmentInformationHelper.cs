using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutwardEnchantmentsViewer.Utility.Helpers
{
    public class EnchantmentInformationHelper
    {
        public static string GetDamageListDescription(Enchantment enchantment)
        {
            string output = "";

            if (enchantment.DamageBonus == null || enchantment.DamageBonus.Count < 1)
                return output;
            
            #if DEBUG
            output += $"Damage Bonus \n";
            #endif

            int totalDamageBonuses = enchantment.DamageBonus.Count;
            int totalDamageBonusesMinusOne = totalDamageBonuses - 1;
            DamageType damageBonus = enchantment.DamageBonus[0];

            output += GetDamageDescription(damageBonus, true);

            for(int currentDamageBonus = 1; currentDamageBonus < totalDamageBonusesMinusOne; currentDamageBonus++)
            {
                damageBonus = enchantment.DamageBonus[currentDamageBonus];
                output += ", " + GetDamageDescription(damageBonus);
            }

            if (totalDamageBonusesMinusOne > 0)
            {
                damageBonus = enchantment.DamageBonus[totalDamageBonusesMinusOne];
                output += " and " + GetDamageDescription(damageBonus);
            }

            output += "\n\n";

            return output;
        }

        private static string GetDamageDescription(DamageType damageType, bool fullGrowthWords = false)
        {
            string damageGrowthWord = "";

            if (fullGrowthWords)
            {
                damageGrowthWord = "Lose ";

                if (damageType.Damage > 0)
                    damageGrowthWord = "Gain +";
            }
            else 
            {
                if (damageType.Damage > 0)
                    damageGrowthWord = "+";
            }

            return $"{damageGrowthWord}{damageType.Damage} flat {damageType.Type} damage";
        }

        public static string GetModifiersListDescriptions(Enchantment enchantment)
        {
            string output = "";

            if (enchantment.DamageModifier == null || enchantment.DamageModifier.Count < 1)
                return output;

            #if DEBUG
            output += $"Damage Modifiers \n";
            #endif

            int totalDamageModifiers = enchantment.DamageModifier.Count;
            int totalDamageModifiersMinusOne = totalDamageModifiers - 1;
            DamageType damageModifier = enchantment.DamageModifier[0];

            output += GetModifierDescription(damageModifier, true);

            for(int currentDamageModifier = 1; currentDamageModifier < totalDamageModifiersMinusOne; currentDamageModifier++)
            {
                damageModifier = enchantment.DamageModifier[currentDamageModifier];
                output += ", " + GetModifierDescription(damageModifier);
            }

            if (totalDamageModifiersMinusOne > 0)
            {
                damageModifier = enchantment.DamageModifier[totalDamageModifiersMinusOne];
                output += " and " + GetModifierDescription(damageModifier);
            }
            output += "\n\n";

            return output;
        }

        private static string GetModifierDescription(DamageType damageType, bool fullGrowthWords = false)
        {
            string damageGrowthWord = "";

            if (fullGrowthWords)
            {
                damageGrowthWord = "Lose ";

                if (damageType.Damage > 0)
                    damageGrowthWord = "Gain +";
            }
            else 
            {
                if (damageType.Damage > 0)
                    damageGrowthWord = "+";
            }

            return $"{damageGrowthWord}{damageType.Damage}% {damageType.Type} Damage Bonus";
        }

        public static string GetAdditionalDamagesDescription(Enchantment enchantment)
        {
            string output = "";

            if (enchantment.AdditionalDamages == null || enchantment.AdditionalDamages.Count() < 1)
                return output;

            #if DEBUG
            output += $"Additional Damages \n";
            #endif

            foreach (Enchantment.AdditionalDamage additionalDamage in enchantment.AdditionalDamages)
            {
                string damageGrowthWord = "Removes ";

                if (additionalDamage.ConversionRatio > 0)
                    damageGrowthWord = "Adds +";

                string converstionRate = ((int)Math.Round((additionalDamage.ConversionRatio * 100), 0)).ToString();

                output += $"{damageGrowthWord}{converstionRate}% of the existing weapon's {additionalDamage.SourceDamageType}" +
                    $" damage as {additionalDamage.BonusDamageType} damage \n\n";
            }

            return output;
        }

        public static string GetStatModificationsDescription(Enchantment enchantment)
        {
            string output = "";

            if (enchantment.StatModifications == null || enchantment.StatModifications.Count < 1)
                return output;

            #if DEBUG
            output += $"Stat Modifications \n";
            #endif

            int totalStatModifications = enchantment.StatModifications.Count;
            int totalStatModificationsMinusOne = totalStatModifications - 1;
            Enchantment.StatModification statModification = enchantment.StatModifications[0];

            output += GetStatModificationDescription(statModification, true);

            for(int currentStatModification = 1; currentStatModification < totalStatModificationsMinusOne; currentStatModification++)
            {
                statModification = enchantment.StatModifications[currentStatModification];
                output += ", " + GetStatModificationDescription(statModification);
            }

            if (totalStatModificationsMinusOne > 0)
            {
                statModification = enchantment.StatModifications[totalStatModificationsMinusOne];
                output += " and " + GetStatModificationDescription(statModification);
            }
            output += "\n\n";

            return output;
        }

        private static string GetStatModificationDescription(Enchantment.StatModification statModification, bool fullGrowthWords = false)
        {
            string statGrowthWord = "";
            string statModificationName = Regex.Replace(statModification.Name.ToString(), "(?<!^)([A-Z])", " $1");

            //removes/adds original words
            if (fullGrowthWords)
            {
                statGrowthWord = "Removes ";

                if (statModification.Value > 0)
                    statGrowthWord = "Adds +";
            }
            else 
            {
                if (statModification.Value > 0)
                    statGrowthWord = "+";
            }

            string typeInformation = (statModification.Type.ToString() == "Modifier") ? "% " : " " + statModification.Type.ToString() + " ";

            return $"{statGrowthWord}{statModification.Value.ToString()}{typeInformation}{statModificationName}";
        }

        public static string GetElementalResistancesDescription(Enchantment enchantment)
        {
            string output = "";

            if (enchantment.ElementalResistances == null || enchantment.ElementalResistances.Count < 1)
                return output;

            #if DEBUG
            output += $"Elemental Resistances \n";
            #endif
            int totalElementalResistances = enchantment.ElementalResistances.Count;
            int totalElementalResistancesMinusOne = totalElementalResistances - 1;
            DamageType elementalResistance = enchantment.ElementalResistances[0];

            output += GetElementalResistanceDescription(elementalResistance, true);

            for(int currentElementalResistance = 1; currentElementalResistance < totalElementalResistancesMinusOne; currentElementalResistance++)
            {
                elementalResistance = enchantment.ElementalResistances[currentElementalResistance];
                output += ", " + GetElementalResistanceDescription(elementalResistance);
            }

            if (totalElementalResistancesMinusOne > 0)
            {
                elementalResistance = enchantment.ElementalResistances[totalElementalResistancesMinusOne];
                output += " and " + GetElementalResistanceDescription(elementalResistance);
            }

            output += "\n\n";

            return output;
        }

        private static string GetElementalResistanceDescription(DamageType damageType, bool fullGrowthWords = false)
        {
            string resistanceGrowthWord = "";

            //removes/adds original words
            if (fullGrowthWords)
            {
                resistanceGrowthWord = "Removes ";

                if (damageType.Damage > 0)
                    resistanceGrowthWord = "Adds +";
            }
            else 
            {
                if (damageType.Damage > 0)
                    resistanceGrowthWord = "+";
            }

            return $"{resistanceGrowthWord}{damageType.Damage}% {damageType.Type} resistance";
        }

        public static string GetEffectsDescription(Enchantment enchantment)
        {
            string output = "";

            if (enchantment.Effects.Count() < 1)
                return output;

            #if DEBUG
            output += $"Effects \n";
            #endif

            GenericHelper.SplitDerivedClasses(
                enchantment.Effects, 
                out Effect[] remainingEffects, 
                out AddStatusEffectBuildUp[] derivedStatusEffects
            );

            output += GetAddStatusEffectBuildUpDescription(derivedStatusEffects);

            GenericHelper.SplitDerivedClasses(
                remainingEffects, 
                out remainingEffects, 
                out ShootEnchantmentBlast[] derivedShootBlasts
            );

            foreach (ShootEnchantmentBlast shootBlast in derivedShootBlasts)
            {
                output += $"Weapon deals an AoE {shootBlast.BaseBlast?.GetComponentInChildren<WeaponDamage>()?.OverrideDType} \"Blast\" with " + 
                    $"{shootBlast.DamageMultiplier}x damage multiplier (based on Weapon's total base damage) \n\n";
            }

            GenericHelper.SplitDerivedClasses(
                remainingEffects, 
                out remainingEffects, 
                out AffectStatusEffectBuildUpResistance[] derivedStatusResistances
            );

            output += GetAffectStatusEffectBuildUpResistancesDescription(derivedStatusResistances);

            GenericHelper.SplitDerivedClasses(
                remainingEffects, 
                out remainingEffects, 
                out AddStatusEffect[] derivedAddStatusEffect
            );

            output += GetAddStatusEffectsDescription(derivedAddStatusEffect);

            foreach (Effect effect in remainingEffects)
            {
                //left for people to report
                output += $"Type: {effect.GetType()}";
            }

            output += "\n";
            return output;
        }

        private static string GetAddStatusEffectBuildUpDescription(AddStatusEffectBuildUp[] derivedStatusEffects)
        {
            string output = "";

            if (derivedStatusEffects.Length < 1)
                return output;

            int derivedLengthMinusOne = derivedStatusEffects.Length - 1;
            AddStatusEffectBuildUp currentStatus = derivedStatusEffects[0];

            output += $"Weapon now inflicts {currentStatus.Status.StatusName} " +
                $"({currentStatus.BuildUpValue}% buildup)";

            for (int currentStatusEffect = 1; currentStatusEffect < derivedLengthMinusOne; currentStatusEffect++)
            {
                currentStatus = derivedStatusEffects[currentStatusEffect];

                output += $", {currentStatus.Status.StatusName} " +
                    $"({currentStatus.BuildUpValue}% buildup)";
            }

            if (derivedLengthMinusOne > 0)
            {
                currentStatus = derivedStatusEffects[derivedLengthMinusOne];
                output += $" and {currentStatus.Status.StatusName} " +
                    $"({currentStatus.BuildUpValue}% buildup) \n\n";
            }

            return output;
        }

        private static string GetAffectStatusEffectBuildUpResistancesDescription(AffectStatusEffectBuildUpResistance[] derivedStatusResistances)
        {
            string output = "";

            if (derivedStatusResistances.Length < 1)
                return output;

            int derivedResistancesLengthMinusOne = derivedStatusResistances.Length - 1;
            AffectStatusEffectBuildUpResistance currentStatusResistance = derivedStatusResistances[0];

            output += $"Euipment now provides {currentStatusResistance.StatusEffect.StatusName} " + 
                $"({currentStatusResistance.RealValue}% buildup) resistance";

            for(int currentResistancesEffect = 0; currentResistancesEffect < derivedResistancesLengthMinusOne; currentResistancesEffect++)
            {
                currentStatusResistance = derivedStatusResistances[currentResistancesEffect];

                output += $", {currentStatusResistance.StatusEffect.StatusName} " + 
                    $"({currentStatusResistance.RealValue}% buildup) resistance";
            }

            if (derivedResistancesLengthMinusOne > 0)
            {
                currentStatusResistance = derivedStatusResistances[derivedResistancesLengthMinusOne];
                output += $" and {currentStatusResistance.StatusEffect.StatusName} " +
                    $"({currentStatusResistance.RealValue}% buildup) resistance \n";
            }

            return output;
        }

        private static string GetAddStatusEffectsDescription(AddStatusEffect[] derivedAddStatusEffect)
        {
            string output = "";

            if (derivedAddStatusEffect.Length < 1)
                return output;

            int derivedStatusEffectLengthMinusOne = derivedAddStatusEffect.Length - 1;
            AddStatusEffect currentAddStatusEffect = null;

            #if DEBUG
            SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@GetEnchantmentInformation has AddStatusEffect");
            #endif
            string growthText = "";

            for(int currentAddStatusEffectNumber = 0; currentAddStatusEffectNumber < derivedAddStatusEffect.Length; currentAddStatusEffectNumber++)
            {
                currentAddStatusEffect = derivedAddStatusEffect[currentAddStatusEffectNumber];

                if(currentAddStatusEffect.ChancesToContract > 0)
                {
                    growthText = "+";
                }
                else
                {
                    growthText = "";
                }

                if(currentAddStatusEffectNumber == 0)
                {
                    output += $"Euipment now contracts {currentAddStatusEffect.Status.StatusName} " + 
                        $"({growthText}{currentAddStatusEffect.ChancesToContract})";
                }
                else if(currentAddStatusEffectNumber == derivedStatusEffectLengthMinusOne)
                {
                    output += $" and {currentAddStatusEffect.Status.StatusName} " + 
                        $"({growthText}{currentAddStatusEffect.ChancesToContract}) \n";
                }
                else
                {
                    output += $", {currentAddStatusEffect.Status.StatusName} " + 
                        $"({growthText}{currentAddStatusEffect.ChancesToContract})";
                }
            }

            return output;
        }
    }
}
