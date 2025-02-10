using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardEnchantmentsViewer.Utility.Helpers
{
    public class ItemHelpers
    {
        public static List<T> GetAllItemsOfType<T>(List<Item> items) where T : Item
        {
            List<T> filteredItems = new List<T>();

            foreach (Item item in items)
            {
                if(item is T specificItem)
                {
                    filteredItems.Add(specificItem);
                    #if DEBUG
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} got item of type {specificItem.GetType().Name} name {specificItem.Name} id {specificItem.ItemID}");
                    #endif
                }
            }

            return filteredItems;
        }

        public static List<Item> GetUniqueItemsInInventory(CharacterInventory inventory)
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
