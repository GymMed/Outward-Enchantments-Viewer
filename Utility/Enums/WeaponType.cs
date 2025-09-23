using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardEnchantmentsViewer.Utility.Enums
{
    //public enum WeaponType
    //{
    //    Sword_1H,
    //    Axe_1H,
    //    Mace_1H,
    //    Dagger_OH = 30,
    //    Chakram_OH = 40,
    //    Pistol_OH = 45,
    //    Halberd_2H = 50,
    //    Sword_2H,
    //    Axe_2H,
    //    Mace_2H,
    //    Spear_2H,
    //    FistW_2H,
    //    Shield = 100,
    //    Arrow = 150,
    //    Bow = 200
    //}

    public static class WeaponTypeExtensions
    {
        public static readonly Dictionary<Weapon.WeaponType, string> WeaponTypes = new()
        {
            { Weapon.WeaponType.Sword_1H, "One-Handed Sword" },
            { Weapon.WeaponType.Axe_1H, "One-Handed Axe" },
            { Weapon.WeaponType.Mace_1H, "One-Handed Mace" },
            { Weapon.WeaponType.Dagger_OH, "Dagger" },
            { Weapon.WeaponType.Chakram_OH, "Chakram" },
            { Weapon.WeaponType.Pistol_OH, "Pistol" },
            { Weapon.WeaponType.Halberd_2H, "Halberd" },
            { Weapon.WeaponType.Sword_2H, "Two-Handed Sword" },
            { Weapon.WeaponType.Axe_2H, "Two-Handed Axe" },
            { Weapon.WeaponType.Mace_2H, "Two-Handed Mace" },
            { Weapon.WeaponType.Spear_2H, "Two-Handed Spear" },
            { Weapon.WeaponType.FistW_2H, "Gauntlet" },
            { Weapon.WeaponType.Shield, "Shield" },
            { Weapon.WeaponType.Arrow, "Arrow" },
            { Weapon.WeaponType.Bow, "Bow" },
        };

        public static string GetWeaponTypeName(this Weapon.WeaponType type)
        {
            return WeaponTypes.TryGetValue(type, out string name) ? name : "Unknown";
        }
    }
}
