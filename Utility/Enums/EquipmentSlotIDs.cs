using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardEnchantmentsViewer.Utility.Enums
{
    public static class EquipmentSlotIDsExtensions
    {
        //public enum EquipmentSlotIDs
        //{
        //    Helmet,
        //    Chest,
        //    Legs,
        //    Foot,
        //    Hands,
        //    RightHand,
        //    LeftHand,
        //    Back,
        //    Quiver,
        //    Count
        //}

        public static readonly Dictionary<EquipmentSlot.EquipmentSlotIDs, string> ArmorTypes = new()
        {
            { EquipmentSlot.EquipmentSlotIDs.Helmet, "Helmet" },
            { EquipmentSlot.EquipmentSlotIDs.Chest, "Chest" },
            { EquipmentSlot.EquipmentSlotIDs.Legs, "Legs" },
            { EquipmentSlot.EquipmentSlotIDs.Foot, "Boots" },
            { EquipmentSlot.EquipmentSlotIDs.Hands, "Hands" },
            { EquipmentSlot.EquipmentSlotIDs.RightHand, "Right Hand" },
            { EquipmentSlot.EquipmentSlotIDs.LeftHand, "Left Hand" },
            { EquipmentSlot.EquipmentSlotIDs.Back, "Back" },
            { EquipmentSlot.EquipmentSlotIDs.Quiver, "Quiver" },
            { EquipmentSlot.EquipmentSlotIDs.Count, "Count" },
        };

        public static string GetArmorTypeName(this EquipmentSlot.EquipmentSlotIDs type)
        {
            return ArmorTypes.TryGetValue(type, out string name) ? name : "Unknown";
        }
    }
}
