using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SideLoader;
using UnityEngine;
using OutwardEnchanmentsViewer.Managers;

namespace OutwardEnchanmentsViewer
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class OutwardEnchanmentsViewer : BaseUnityPlugin
    {
        public const string GUID = "gymmed.outwardenchanmentsviewer";
        // Choose a NAME for your project, generally the same as your Assembly Name.
        public const string NAME = "Outward Enchanments Viewer";

        public const string VERSION = "0.0.1";

        public static string prefix = "[gymmed-Enchanments-Viewer]";

        // For accessing your BepInEx Logger from outside of this class (eg Plugin.Log.LogMessage("");)
        public static ManualLogSource Log 
        {
            get; private set;
        }

        public static OutwardEnchanmentsViewer Instance 
        {
            get; private set;
        }

        // If you need settings, define them like so:
        public static ConfigEntry<bool> ExampleConfig;

        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Log = this.Logger;
            Log.LogMessage($"Hello world from {NAME} {VERSION}!");
            Log.LogMessage($"{OutwardEnchanmentsViewer.prefix} Logger!");
            SL.Log($"{OutwardEnchanmentsViewer.prefix} Party hard!");

            // Any config settings you define should be set up like this:
            ExampleConfig = Config.Bind("ExampleCategory", "ExampleSetting", false, "This is an example setting.");

            // Harmony is for patching methods. If you're not patching anything, you can comment-out or delete this line.
            new Harmony(GUID).PatchAll();
        }

        // Update is called once per frame. Use this only if needed.
        // You also have all other MonoBehaviour methods available (OnGUI, etc)
        internal void Update()
        {

        }

        [HarmonyPatch(typeof(CharacterUI))]
        [HarmonyPatch("SetTargetCharacter", MethodType.Normal)]
        public class CharacterUI_SetTargetCharacterPatch
        {
            //For adding additional section in inventory UI
            [HarmonyPostfix]
            private static void Postfix(CharacterUI __instance)
            {
                try
                {
                    SL.Log($"{OutwardEnchanmentsViewer.prefix} CharacterUI@SetTargetCharacter called!");
                    ItemDisplayManager.Instance.CreateSection(__instance);
                }
                catch(Exception e)
                {

                    SL.Log($"{OutwardEnchanmentsViewer.prefix} CharacterUI@SetTargetCharacter error: {e.Message}");
                }
            }
        }

        [HarmonyPatch(typeof(ItemDetailsDisplay))]
        [HarmonyPatch("RefreshDetails", MethodType.Normal)]
        public class ItemDetailsDisplay_RefreshDetailsPatch
        {
            [HarmonyPostfix]
            private static void Postfix(ItemDetailsDisplay __instance)
            {
                try
                {
                    SL.Log($"{OutwardEnchanmentsViewer.prefix} ItemDetailsDisplay@RefreshDetails called!");
                    CharacterUI characterUI = __instance.CharacterUI;

                    if (!characterUI)
                    {
                        SL.Log($"{OutwardEnchanmentsViewer.prefix} ItemDetailsDisplay@RefreshDetails missing CharacterUI on itemDisplay");
                        return;
                    }

                    Item item = __instance.itemDisplay?.RefItem;
                    ItemDisplayManager.Instance.ShowOriginalDescription(characterUI);

                    CharacterInventory inventory = __instance.LocalCharacter?.Inventory;

                    if (!inventory)
                    {
                        SL.Log($"{OutwardEnchanmentsViewer.prefix} ItemDetailsDisplay@RefreshDetails missing inventory from ItemDisplay!");
                        return;
                    }

                    switch(item)
                    {
                        case Equipment equipment:
                            ItemDescriptionsManager.Instance.SetEquipmentsEnchantmentsDescription(item, inventory, characterUI);
                            break;
                        case EnchantmentRecipeItem enchantmentRecipeItem:
                            ItemDescriptionsManager.Instance.SetEnchantmentsDescription(enchantmentRecipeItem, inventory, characterUI);
                            break;
                        default:
                            ItemDisplayManager.Instance.HideDescription(characterUI);
                            return;
                    }
                }
                catch(Exception e)
                {

                    SL.Log($"{OutwardEnchanmentsViewer.prefix} ItemDetailsDisplay@RefreshDetails error: {e.Message}");
                }
            }
        }
        
        public static List<T> GetAllItemsOfType<T>(List<Item> items) where T : Item
        {
            List<T> filteredItems = new List<T>();

            foreach (Item item in items)
            {
                if(item is T specificItem)
                {
                    filteredItems.Add(specificItem);
                    SL.Log($"{OutwardEnchanmentsViewer.prefix} got item of type {specificItem.GetType().Name} name {specificItem.Name}");
                }
            }

            return filteredItems;
        }
    }
}
