using Epic.OnlineServices.AntiCheatCommon;
using SideLoader;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutwardEnchantmentsViewer.Utility.Helpers
{
    public class ItemEnchantmentInformationHelper
    {
        /*  Does not provide all current damages on item if they do not match with changes
         *  but does provide all applied changes on item if they doesn't match with original values
         *  Method used for look up changes to item
         */
        public static DamageList CalculateChangesInItemDamageList(DamageList itemDamageList, DamageList changesDamageList)
        {
            DamageList finalDamages = new DamageList();
            bool foundChanges = false;

            foreach (DamageType changesType in changesDamageList.m_list)
            { 
                foreach (DamageType type in itemDamageList.m_list) 
                {
                    if(changesType.Type == type.Type)
                    {
                        finalDamages.Add(new DamageType(type.Type, type.Damage + changesType.Damage));
                        foundChanges = true;
                        break;
                    }
                }

                if(foundChanges)
                {
                    foundChanges = false;
                }
                else
                {
                    finalDamages.Add(new DamageType(changesType.Type, changesType.Damage));
                }
            }

            return finalDamages;
        }

        public static string GetStatsModifcationDescriptions(Equipment equipment, Enchantment.StatModificationList stats)
        {
            string output = "";

            if (stats == null || stats.Count() < 1)
                return output;

            string additionalText = "";
            string statModificationName = "";
            string finalValueString = "";
            float baseValue = 0;
            int roundedBaseValue = 0;

            #if DEBUG
            output += $"Stats Modifications {stats.Count}\n";
            #endif
                
            foreach(Enchantment.StatModification stat in stats.statModifications)
            {
                additionalText = stat.Type == Enchantment.StatModification.BonusType.Modifier ? "%" : "";
                statModificationName = Regex.Replace(stat.Name.ToString(), "(?<!^)([A-Z])", " $1");
                baseValue = GetStatValueFromEquipment(equipment, stat);
                roundedBaseValue = (int)Math.Round(baseValue, MidpointRounding.AwayFromZero);

                switch (stat.Name)
                {
                    case Enchantment.Stat.Weight:
                        {
                            finalValueString = GetFinalCalcualtedStatValue(equipment, stat);

                            if (stat.Type == Enchantment.StatModification.BonusType.Modifier)
                                output += $"{roundedBaseValue} => {finalValueString} {statModificationName} ({stat.Value}%)\n";
                            else
                                output += $"{roundedBaseValue} => {finalValueString} {statModificationName}\n";
                            break;
                        }
                    case Enchantment.Stat.FoodDepletionRate:
                    case Enchantment.Stat.DrinkDepletionRate:
                    case Enchantment.Stat.SleepDepletionRate:
                        {
                            output += $"{stat.Value * -1}% {statModificationName}\n";
                            break;
                        }
                    case Enchantment.Stat.HealthRegen:
                    case Enchantment.Stat.ManaRegen:
                        {
                            finalValueString = GetFinalCalcualtedStatValue(equipment, stat);

                            output += $"{baseValue}{additionalText} => {finalValueString} passive {statModificationName} per second\n";
                            break;
                        }
                    case Enchantment.Stat.AttackSpeed:
                        {
                            finalValueString = GetFinalCalcualtedStatValue(equipment, stat);

                            output += $"{baseValue}{additionalText} => {finalValueString} {statModificationName}\n";
                            break;
                        }
                    default:
                        {
                            finalValueString = GetFinalCalcualtedStatValue(equipment, stat);

                            output += $"{roundedBaseValue}{additionalText} => {finalValueString} {statModificationName}\n";
                            break;
                        }
                }
            }

            return output;
        }

        public static string GetFinalCalcualtedStatValue(Equipment equipment, Enchantment.StatModification stat)
        {
            return GetCalculatedStatModificationValue(stat, GetStatValueFromEquipment(equipment, stat));
        }

        public static string GetCalculatedStatModificationValue(Enchantment.StatModification stat, float totalValue)
        {
            float finalValue = 0.0f;

            switch (stat.Name)
            {
                case Enchantment.Stat.Weight:
                    {
                        //Gets actual values instead of percentages

                        if (stat.Type == Enchantment.StatModification.BonusType.Modifier)
                        {
                            return (totalValue + (totalValue * (stat.Value / 100.0))).ToString();
                        }
                        else
                            return (totalValue + stat.Value).ToString();
                    }
                case Enchantment.Stat.FoodDepletionRate:
                case Enchantment.Stat.DrinkDepletionRate:
                case Enchantment.Stat.SleepDepletionRate:
                case Enchantment.Stat.StabilityRegen:
                case Enchantment.Stat.Impact:
                    {
                        if (stat.Type == Enchantment.StatModification.BonusType.Modifier)
                        {
                            finalValue = totalValue + stat.Value;
                            return finalValue.ToString() + "%";
                        }
                        else
                        {
                            finalValue = totalValue + stat.Value;
                            return finalValue.ToString();
                        }
                    }
                case Enchantment.Stat.CooldownReduction:
                case Enchantment.Stat.ManaCostReduction:
                case Enchantment.Stat.StaminaCostReduction:
                case Enchantment.Stat.MovementSpeed:
                {
                    return ((int)Math.Round(totalValue + stat.Value, MidpointRounding.AwayFromZero)).ToString() + "%";
                }
                default:
                        {
                        //calculate percentage
                        if (stat.Type == Enchantment.StatModification.BonusType.Modifier)
                        {
                            finalValue = totalValue * (stat.Value / 100);
                        }
                        else
                        {

                            finalValue = totalValue + stat.Value;
                        }

                    if (finalValue <= 0)
                        return stat.Value.ToString() + "%";

                    return finalValue.ToString();
                }
            }
        }

        public static string GetAdditionalDescriptions(Equipment equipment, Enchantment enchantment, string previousInput = "")
        {
            string output = "";

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
                output += $"Tracking Damage Ratio {enchantment.TrackDamageRatio.ToString()} \n";
            }

            if (!string.IsNullOrWhiteSpace(enchantment.Description))
            {
                output += $"{enchantment.Description} \n";
            }

            if(output == "" && previousInput == "")
            {
                return "Doesn't give stats in traditional way and uses unknown properties \n";
            }

            return output;
        }

        public static string GetDamageListDescription(Enchantment enchantment, Weapon weapon)
        {
            string output = "";

            if (enchantment.DamageBonus == null || enchantment.DamageBonus.Count < 1)
                return output;
            
            #if DEBUG
            output += $"Damage Bonus {enchantment.DamageBonus.Count}\n";
            #endif

            float physicalDamage = weapon.Stats.GetDamageAttack(DamageType.Types.Physical);

            float baseDamage = 0.0f;
            int roundedTotal = 0;

            foreach(DamageType type in enchantment.DamageBonus.List)
            {
                baseDamage = ItemEnchantmentInformationHelper.GetDamageOfType(type.Type, weapon);

                roundedTotal = (int)Math.Round(baseDamage + type.Damage, MidpointRounding.AwayFromZero);

                output += $"{(int)Math.Round(baseDamage, MidpointRounding.AwayFromZero)} => {roundedTotal} {type.Type} Damage Bonus\n";
            }

            return output;
        }

        public static float GetDamageOfType(DamageType.Types _type, Weapon weapon)
        {
            DamageType damageType = null;

            if(weapon.m_weaponStats != null || weapon.OwnerCharacter != null)
                damageType = weapon.GetDamage(0, false)[_type];
            else
                damageType = GetDamageFromWeapon(weapon)[_type];

            if (damageType == null)
            {
                return 0f;
            }
            return damageType.Damage;
        }

        //safer logic for Weapon.GetDamage() method
        public static DamageList GetDamageFromWeapon(Weapon weapon)
        {
            DamageList.EmptyCopy(weapon.Damage, ref weapon.baseDamage);
            WeaponBaseData data = null;

            if(weapon.m_weaponStats == null)
            {
                data = Global.Instance.WeaponBaseStats.WeaponStatsDict[(int)weapon.Type];
            }

            for (int i = 0; i < weapon.baseDamage.Count; i++)
            {
                if (weapon.Stats)
                {
                    IList<float> attackDamage = weapon.Stats.GetAttackDamage(0);
                    if (i < attackDamage.Count)
                    {
                        weapon.baseDamage[i].Damage = attackDamage[i];
                        int index;
                        if (weapon.IsDamageAddedByEnchantment(weapon.baseDamage[i].Type, out index))
                        {
                            weapon.baseDamage[i].Damage += weapon.m_enchantmentDamageBonus[index].Damage;
                        }
                    }
                    else
                    {
                        weapon.baseDamage[i].Damage = weapon.Damage[i].Damage;
                    }
                }
                else
                {
                    if (weapon.m_weaponStats != null)
                        weapon.baseDamage[i].Damage = weapon.m_weaponStats.GetDamage(0);
                    else
                    {
                        weapon.baseDamage[i].Damage = data.GetDamage(0);
                    }
                }
                if (weapon.baseDamage[i].Damage == -1f)
                {

                    if (weapon.m_weaponStats == null)
                    {
                        ItemEnchantmentInformationHelper.fixWeaponBaseDamages(weapon, data, i);
                    }
                    else
                    {
                        ItemEnchantmentInformationHelper.fixWeaponBaseDamages(weapon, weapon.m_weaponStats, i);
                    }
                }
            }
            if (weapon.Stats)
            {
                weapon.baseDamage *= weapon.Stats.Effectiveness;
            }

            return weapon.baseDamage;
        }

        public static void fixWeaponBaseDamages(Weapon weapon, WeaponBaseData weaponStats, int type)
        {
            float damage = weapon.m_baseDamage[type].Damage;
            if (weaponStats.Damage > 0f)
            {
                float damage2 = weaponStats.GetDamage(0);
                if (damage2 != -1f)
                {
                    weapon.baseDamage[type].Damage = damage * (damage2 / weaponStats.Damage);
                }
                else if (weapon.Damage[type].Damage != -1f)
                {
                    weapon.baseDamage[type].Damage = damage;
                }
                else
                {
                    weapon.baseDamage[type].Damage = weaponStats.Damage;
                }
            }
            else
            {
                weapon.baseDamage[type].Damage = damage;
            }
        }

        public static float GetWeaponEnchantmentDamageBonus(Weapon weapon, DamageType damageType)
        {
            DamageList enchantmentDamageBonuses = weapon.GetEnchantmentDamageBonuses();

            foreach(DamageType enchantmentDamage in enchantmentDamageBonuses.List)
            {
                if (damageType.Type == enchantmentDamage.Type)
                    return enchantmentDamage.Damage;
            }

            return 0.0f;
        }

        public static string GetDamageListDescription(Enchantment enchantment, Equipment equipment)
        {
            string output = "";

            if (enchantment.DamageBonus == null || enchantment.DamageBonus.Count < 1)
                return output;
            
            #if DEBUG
            output += $"Damage Bonus {enchantment.DamageBonus.Count}\n";
            #endif

            float physicalDamage = equipment.Stats.GetDamageAttack(DamageType.Types.Physical);

            float baseDamage = 0.0f;
            int roundedTotal = 0;

            foreach(DamageType type in enchantment.DamageBonus.List)
            {
                baseDamage = equipment.GetDamageAttack(type.Type);
                roundedTotal = (int)Math.Round(baseDamage + type.Damage, MidpointRounding.AwayFromZero);

                output += $"{(int)Math.Round(baseDamage, MidpointRounding.AwayFromZero)}% => {roundedTotal}% from {physicalDamage} {type.Type}\n";
            }

            return output;
        }


        public static string GetDamageListDescription(Enchantment enchantment, Armor armor)
        {
            string output = "";

            if (enchantment.DamageBonus == null || enchantment.DamageBonus.Count < 1)
                return output;
            
            #if DEBUG
            output += $"Damage Bonus {enchantment.DamageBonus.Count}\n";
            #endif

            float physicalDamage = armor.Stats.GetDamageAttack(DamageType.Types.Physical);

            float baseDamage = 0.0f;
            int roundedTotal = 0;

            foreach(DamageType type in enchantment.DamageBonus.List)
            {
                baseDamage = armor.Stats.GetDamageAttack(type.Type);
                roundedTotal = (int)Math.Round(baseDamage + type.Damage, MidpointRounding.AwayFromZero);

                output += $"{(int)Math.Round(baseDamage, MidpointRounding.AwayFromZero)}% => {roundedTotal}% from {physicalDamage} {type.Type}\n";
            }

            return output;
        }

        public static string GetDamageListDescription(Enchantment enchantment, DamageList equipmentDamageList)
        {
            string output = "";

            if (enchantment.DamageBonus == null || enchantment.DamageBonus.Count < 1)
                return output;
            
            #if DEBUG
            output += $"Damage Bonus {enchantment.DamageBonus.Count}\n";
            #endif
            DamageList changesInDamages = CalculateChangesInItemDamageList(equipmentDamageList, enchantment.DamageBonus);
            List<DamageType> equipmentDamages = equipmentDamageList.List;
            bool foundChanges = false;

            foreach(DamageType type in changesInDamages.List)
            {
                foreach (DamageType baseType in equipmentDamages)
                {
                    if (baseType.Type == type.Type)
                    {
                        foundChanges = true;
                        output += $"{baseType.Damage} => {type.Damage} {type.Type}\n";
                        break;
                    }
                }

                if(foundChanges)
                {
                    foundChanges = false;
                }
                else
                {
                    output += $"0 => {type.Damage} {type.Type}\n";
                }
            }

            return output;
        }

        public static string BuildDescriptions(Enchantment enchantment, Equipment equipment)
        {
            try
            {
                string methodsOutput = "";

                if (equipment is Weapon weapon)
                {
                    DamageList weaponStats = weapon.Stats.BaseDamage;
                    methodsOutput += GetDamageListDescription(enchantment, weapon);
                    methodsOutput += GetDamageModifiersDescription(enchantment, weapon);
                    methodsOutput += GetAdditionalDamagesDescription(enchantment, weaponStats);
                    methodsOutput += GetEffectsDescription(enchantment, weaponStats);
                    methodsOutput += GetStatsModifcationDescriptions(equipment, enchantment.StatModifications);
                    methodsOutput += GetElementalResistancesDescription(enchantment, weapon);
                    methodsOutput += GetAdditionalDescriptions(equipment, enchantment, methodsOutput);
                }
                else if (equipment is Armor armor)
                {
                    ArmorBaseData baseData = armor.m_baseData;

                    if(armor.m_baseData == null)
                    {
                        baseData = Global.Instance.WeaponBaseStats.GetArmorData(armor.EquipSlot, armor.Class);
            #if DEBUG
                        SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@BuildDescriptions armor.m_baseData is null!");
            #endif
                    }

                    if(baseData.DamageReduction == null)
                    {
                        return BuildDefaultDescriptions(enchantment, equipment);
            #if DEBUG
                        SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@BuildDescriptions armorBaseData.DamageReduction is null!");
            #endif
                    }

                    DamageType[] damageResistances = baseData.DamageReduction.ToArray();//equipment.Stats.m_damageResistance.ToArray();// armor.m_baseData.DamageReduction.ToArray();
                    //armor.m_baseData.DamageResistance
                    DamageList damageReductions = new DamageList(damageResistances);
                    //DamageList armorStats = armor.Stats.BaseDamage;
                    methodsOutput += BuildArmorDescriptions(enchantment, armor, damageReductions);
                }
                else if(equipment is Bag bag)
                {
                    methodsOutput += GetDamageListDescription(enchantment, bag);
                    methodsOutput += GetDamageModifiersDescription(enchantment, bag);
                    methodsOutput += GetEffectsDescription(enchantment);
                    methodsOutput += GetStatsModifcationDescriptions(equipment, enchantment.StatModifications);
                    methodsOutput += GetElementalResistancesDescription(enchantment, bag);
                    methodsOutput += GetAdditionalDescriptions(equipment, enchantment, methodsOutput);
                }

                return methodsOutput;
            }
            catch(Exception ex)
            {
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@BuildDescriptions error: {ex.Message}");
                return "Error";
            }
        }

        public static string BuildArmorDescriptions(Enchantment enchantment, Armor armor, DamageList damageReductions)
        {
            string methodsOutput = "";

            methodsOutput += GetDamageListDescription(enchantment, armor);
            methodsOutput += GetDamageModifiersDescription(enchantment, armor);
            methodsOutput += GetAdditionalDamagesDescription(enchantment, damageReductions);
            methodsOutput += GetEffectsDescription(enchantment, damageReductions);
            methodsOutput += GetStatsModifcationDescriptions(armor, enchantment.StatModifications);
            methodsOutput += GetElementalResistancesDescription(enchantment, armor);
            methodsOutput += GetAdditionalDescriptions(armor, enchantment, methodsOutput);

            return methodsOutput;
        }

        public static string BuildDefaultDescriptions(Enchantment enchantment, Equipment equipment)
        {
            try
            {
                string methodsOutput = "";

                methodsOutput += GetDamageListDescription(enchantment, equipment);
                methodsOutput += GetDamageModifiersDescription(enchantment, equipment);
                methodsOutput += GetEffectsDescription(enchantment);
                methodsOutput += GetStatsModifcationDescriptions(equipment, enchantment.StatModifications);
                methodsOutput += GetElementalResistancesDescription(enchantment, equipment);
                methodsOutput += GetAdditionalDescriptions(equipment, enchantment, methodsOutput);

                return methodsOutput;
            }
            catch(Exception ex)
            {
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@BuildDefaultDescriptions error: {ex.Message}");
                return "Error";
            }
        }
        
        public static string GetDamageModifiersDescription(Enchantment enchantment, Equipment equipment)
        {
            string output = "";

            if (enchantment.DamageModifier == null || enchantment.DamageModifier.Count < 1)
                return output;
            
            #if DEBUG
            output += $"Damage Modifiers \n";
            #endif
            DamageList changesInDamages = new DamageList();

            float baseDamage = 0.0f;
            int roundedResult = 0;

            foreach (DamageType type in enchantment.DamageModifier.List)
            {
                baseDamage = equipment.Stats.GetDamageAttack(type.Type);
                roundedResult = (int)Math.Round(baseDamage + type.Damage, MidpointRounding.AwayFromZero);

                output += $"{baseDamage}% => {roundedResult}% {type.Type} Damage Bonus\n";
            }

            return output;
        }

        public static string TryGetFromDamageText(DamageType physicalDamage, DamageType type)
        {
            string output = ""; 

            if(physicalDamage != null)
            {
                output = $"from {physicalDamage.Damage} ";
            }

            output += $"{type.Type} Damage\n";

            return output;
        }

        public static string GetAdditionalDamagesDescription(Enchantment enchantment, DamageList equipmentDamageList)
        {
            string output = "";

            if (enchantment.AdditionalDamages == null || enchantment.AdditionalDamages.Count() < 1)
                return output;

            #if DEBUG
            output += $"Additional Damages \n";
            #endif

            List<DamageType> equipmentDamages = equipmentDamageList.List;

            foreach (Enchantment.AdditionalDamage additionalDamage in enchantment.AdditionalDamages)
            {
                int converstion = (int)Math.Round((additionalDamage.ConversionRatio * 100), 0);
                string converstionRate = converstion.ToString();
                DamageType sourceDamageType = GetMatchingDamageType(equipmentDamages, additionalDamage.SourceDamageType);
                DamageType bonusDamageType = GetMatchingDamageType(equipmentDamages, additionalDamage.BonusDamageType);

                float sourceDamage = sourceDamageType != null ? sourceDamageType.Damage : 0.0f;
                float bonusDamage = bonusDamageType != null ? bonusDamageType.Damage : 0.0f;

                float convertedDamage = sourceDamage * (converstion / 100.0f);
                float totalDamage = bonusDamage + convertedDamage;
                int roundedTotal = (int)Math.Round(totalDamage, MidpointRounding.AwayFromZero);
                int roundedBonusDamage = (int)Math.Round(bonusDamage, MidpointRounding.AwayFromZero);

                if(roundedBonusDamage == roundedTotal)
                {
                    #if DEBUG
                    output += $"Rounded Bonus Damage matched Rounded Total = no changes \n";
                    #endif
                    continue;
                }

                //output += $"Source damage is found {sourceDamage} conv {converstion} and {convertDamage}\n";

                //output += $"Adds {converstionRate}% {convertDamage} of the existing weapon's {additionalDamage.SourceDamageType}" +
                //    $" damage as {additionalDamage.BonusDamageType} damage \n\n";
                output += $"{roundedBonusDamage} => {roundedTotal} ({converstion}% of {(int)Math.Round(sourceDamage, MidpointRounding.AwayFromZero)} {additionalDamage.SourceDamageType}) {additionalDamage.BonusDamageType} Damage\n";
            }

            return output;
        }

        public static DamageType GetMatchingDamageType(List<DamageType> damages, DamageType.Types searchingDamage)
        {
            foreach (DamageType type in damages)
            {
                if (type.Type == searchingDamage)
                {
                    return type;
                }
            }

            return null;
        }

        public static string GetElementalResistancesDescription(Enchantment enchantment, Equipment equipment)
        {
            string output = "";

            if (enchantment.ElementalResistances == null || enchantment.ElementalResistances.Count < 1)
                return output;

            #if DEBUG
            output += $"Elemental Resistances \n";
            #endif

            float baseResistance = 0.0f;
            int roundedTotal = 0;

            foreach (DamageType type in enchantment.ElementalResistances.List)
            {
                baseResistance = equipment.GetDamageResistance(type.Type);
                roundedTotal = (int)Math.Round(baseResistance + type.Damage, MidpointRounding.AwayFromZero);
                output += $"{(int)Math.Round(baseResistance, MidpointRounding.AwayFromZero)}% => {roundedTotal}% {type.Type} resistance\n";
            }

            return output;
        }

        public static string GetElementalResistancesDescription(Enchantment enchantment, DamageList equipmentDamageList)
        {
            string output = "";

            if (enchantment.ElementalResistances == null || enchantment.ElementalResistances.Count < 1)
                return output;

            #if DEBUG
            output += $"Elemental Resistances \n";
            #endif

            DamageList changesInDamages = new DamageList();

            changesInDamages = CalculateChangesInItemDamageList(equipmentDamageList, enchantment.ElementalResistances);
            List<DamageType> equipmentDamages = equipmentDamageList.List;
            bool foundMatch = false;

            foreach (DamageType type in changesInDamages.List)
            {
                foreach (DamageType baseType in equipmentDamages)
                {
                    if (baseType.Type == type.Type)
                    {
                        foundMatch = true;
                        output += $"{type.Damage} => {baseType.Damage * (type.Damage / 100)} {type.Type} resistance\n";
                        break;
                    }
                }

                if(foundMatch)
                {
                    foundMatch = false;
                }
                else 
                {
                    output += $"0% => {type.Damage}% {type.Type} resistance\n";
                }
            }

            return output;
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

            if (!output.EndsWith("\n"))
            {
                output += "\n";
            }

            return output;
        }

        public static string GetEffectsDescription(Enchantment enchantment, DamageList damages)
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
                if (shootBlast.BaseBlast == null)
                    continue;

                WeaponDamage weaponDamage = shootBlast.BaseBlast.GetComponentInChildren<WeaponDamage>();

                if (weaponDamage == null)
                    continue;

                bool foundMatch = false;

                if(damages == null)
                {
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@ItemEnchantmentInformationHelper couldn't find matching damage type for shootBlast converstion");
                    return "Weapon or it's damages are not found";
                }

                foreach (DamageType type in damages.List)
                {
                    if (type.Type == DamageType.Types.Physical)
                    {
                        float converstionRate = type.Damage * (shootBlast.DamageMultiplier);
                        int converstion = (int)Math.Round((shootBlast.DamageMultiplier * 100), 0);

                        output += $"Weapon deals an AoE {weaponDamage.OverrideDType} \"Blast\" with " +
                            $"{converstion}x ({converstionRate}) damage multiplier (based on Weapon's total base damage {type.Damage}) \n";

                        foundMatch = true;
                        break;
                    }
                }

                if(!foundMatch)
                {
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionsManager@ItemEnchantmentInformationHelper couldn't find matching damage type for shootBlast converstion");
                    output += "Physical damage not found!";
                }
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
                output += $"\nType: {effect.GetType()}";
            }

            if (!output.EndsWith("\n"))
            {
                output += "\n";
            }

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
                    $"({currentStatus.BuildUpValue}% buildup) \n";
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
                $"({currentStatusResistance.Value}% buildup) resistance";

            for(int currentResistancesEffect = 1; currentResistancesEffect < derivedResistancesLengthMinusOne; currentResistancesEffect++)
            {
                currentStatusResistance = derivedStatusResistances[currentResistancesEffect];

                output += $", {currentStatusResistance.StatusEffect.StatusName} " + 
                    $"({currentStatusResistance.Value}% buildup) resistance";
            }

            if (derivedResistancesLengthMinusOne > 0)
            {
                currentStatusResistance = derivedStatusResistances[derivedResistancesLengthMinusOne];
                output += $" and {currentStatusResistance.StatusEffect.StatusName} " +
                    $"({currentStatusResistance.Value}% buildup) resistance \n";
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

        public static float GetStatValueFromEquipment(Equipment equipment, Enchantment.StatModification stat)
        {
            switch(stat.Name)
            {
                case Enchantment.Stat.Weight:
                    {
                        return equipment.Weight;
                    }
                case Enchantment.Stat.Durability:
                    {
                        return equipment.MaxDurability;
                    }
                case Enchantment.Stat.ManaCostReduction:
                    {
                        //use base
                        return equipment.Stats.ManaUseModifier * -1;
                    }
                case Enchantment.Stat.StaminaCostReduction:
                    {
                        return equipment.Stats.StaminaCostReduction;
                    }
                case Enchantment.Stat.CooldownReduction:
                    {
                        //use base
                        return equipment.Stats.CooldownReduction * -1;
                    }
                case Enchantment.Stat.MovementSpeed:
                    {
                        return equipment.Stats.MovementPenalty * -1;
                    }
                case Enchantment.Stat.AttackSpeed:
                    {
                        //only found on weaponBaseData
                        if(equipment is Weapon weapon)
                            return weapon.Stats.AttackSpeed;
                        return 0.0f;
                    }
                case Enchantment.Stat.Impact:
                    {
                        return equipment.Stats.ImpactModifier;
                    }
                case Enchantment.Stat.StabilityRegen:
                    {
                        //get char base stability
                        return equipment.StabilityRegenModifier;
                    }
                case Enchantment.Stat.HealthRegen:
                    {
                        return equipment.HealthRegenBonus;
                    }
                case Enchantment.Stat.ManaRegen:
                    {
                        return equipment.ManaRegenBonus;
                    }
                case Enchantment.Stat.Protection:
                    {
                        return equipment.GetDamageProtection(DamageType.Types.Physical);
                    }
                case Enchantment.Stat.CorruptionResistance:
                    {
                        return equipment.CorruptionResistance;
                    }
                case Enchantment.Stat.FoodDepletionRate:
                    {
                        return stat.Value;
                    }
                case Enchantment.Stat.DrinkDepletionRate:
                    {
                        return stat.Value;
                    }
                case Enchantment.Stat.SleepDepletionRate:
                    {
                        return stat.Value;
                    }
                case Enchantment.Stat.PouchCapacity:
                    {
                        return equipment.PouchCapacityBonus;
                    }
                case Enchantment.Stat.ImpactResistance:
                    {
                        return equipment.Stats.ImpactResistance;
                    }
                case Enchantment.Stat.HeatProtection:
                    {
                        return equipment.HeatProtection;
                    }
                case Enchantment.Stat.ColdProtection:
                    {
                        return equipment.ColdProtection;
                    }
                case Enchantment.Stat.Barrier:
                    {
                        return equipment.Stats.BarrierProtection;
                    }
                default:
                    {
                        return 0.0f;
                    }
            }
        }
    }
}
