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
using OutwardEnchantmentsViewer.Managers;
using OutwardEnchantmentsViewer.Utility.Fixers;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using OutwardEnchantmentsViewer.Utility.Helpers;
using OutwardEnchantmentsViewer.Enchantments;

namespace OutwardEnchantmentsViewer
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class OutwardEnchantmentsViewer : BaseUnityPlugin
    {
        public const string GUID = "gymmed.outwardenchantmentsviewer";
        // Choose a NAME for your project, generally the same as your Assembly Name.
        public const string NAME = "Outward Enchantments Viewer";

        public const string VERSION = "0.5.0";

        public static string prefix = "[gymmed-Enchantments-Viewer]";

        // For accessing your BepInEx Logger from outside of this class (eg Plugin.Log.LogMessage("");)
        public static ManualLogSource Log 
        {
            get; private set;
        }

        public static OutwardEnchantmentsViewer Instance 
        {
            get; private set;
        }

        // If you need settings, define them like so:
        public static ConfigEntry<bool> ShowEnchantmentDescriptions;
        public static ConfigEntry<bool> ShowEquipmentDescriptions;
        public static ConfigEntry<bool> ShowAllAvailableEnchantmentsCountForEquipment;
        public static ConfigEntry<bool> ShowMissingEnchantmentsForEquipment;
        public static ConfigEntry<bool> ShowDescriptionsOnlyForInventory;
        public static ConfigEntry<bool> ShowEquipmentOwnedEnchantmentsDetailed;
        public static ConfigEntry<bool> ShowEquipmentUnownedEnchantmentsDetailed;
        public static ConfigEntry<bool> ShowInShopEnchantmentWithEquipmentType;

        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Log = this.Logger;
            Log.LogMessage($"Hello world from {NAME} {VERSION}!");

            #if DEBUG
            Log.LogMessage($"{OutwardEnchantmentsViewer.prefix} Logger!");
            SL.Log($"{OutwardEnchantmentsViewer.prefix} Party hard!");
            #endif

            // Any config settings you define should be set up like this:
            ShowEnchantmentDescriptions = Config.Bind("Enchantments Descriptions", "ShowEnchantmentDescriptions", true, "Show detailed descriptions for enchantments?");
            ShowEquipmentDescriptions = Config.Bind("Equipment Descriptions", "ShowEquipmentEnchantmentsDescriptions", true, "Show enchantments for equipment?");
            
            ShowAllAvailableEnchantmentsCountForEquipment = Config.Bind("Equipment Descriptions Header", "ShowAllAvailableEnchantmentsCountForEquipment", true, "Show all available enchantments count for equipment?");
            ShowMissingEnchantmentsForEquipment = Config.Bind("Equipment Descriptions Body", "ShowMissingEnchantmentsForEquipment", true, "Show missing enchantments for equipment?");

            ShowDescriptionsOnlyForInventory = Config.Bind("Show Descriptions in Panels", "ShowDescriptionsOnlyForInventory", false, "Show descriptions only for items in inventory?");

            ShowEquipmentDescriptions = Config.Bind("Equipment Descriptions", "ShowEquipmentEnchantmentsDescriptions", true, "Show enchantments for equipment?");

            ShowEquipmentOwnedEnchantmentsDetailed = Config.Bind(
                "Equipment Descriptions",
                "ShowEquipmentOwnedEnchantmentsDetailed",
                true,
                "Show owned equipment enchantments in greater detail?"
            );

            ShowEquipmentUnownedEnchantmentsDetailed = Config.Bind(
                "Equipment Descriptions",
                "ShowEquipmentUnownedEnchantmentsDetailed",
                true,
                "Show unowned equipment enchantments in greater detail?"
            );

            ShowInShopEnchantmentWithEquipmentType = Config.Bind(
                "Enchantments Descriptions",
                "ShowEnchantmentEquipmentTypeInShop",
                true,
                "Show in shop enchantment with equipment type?"
            );

            // Harmony is for patching methods. If you're not patching anything, you can comment-out or delete this line.
            new Harmony(GUID).PatchAll();
        }

        // Update is called once per frame. Use this only if needed.
        // You also have all other MonoBehaviour methods available (OnGUI, etc)
        internal void Update()
        {

        }

        [HarmonyPatch(typeof(ItemDetailsDisplay), "OnScrollDownPressed")]
        public class Patch_OnScrollDownPressed
        {
            static bool Prefix(ItemDetailsDisplay __instance)
            {
                #if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDetailsDisplay@OnScrollDownPressed called!");
                #endif
                // Ensure we don't run when already moving
                if (__instance.m_movingScrollview)
                {
                    return false;
                }

                //split screen character has a bug where scroll view port size is not passed let's fix it
                if(__instance.m_viewPortSize == 0)
                {
                    RectTransform viewPort = __instance.transform.Find("ItemDetails/Stats/Scroll View/Viewport").GetComponent<RectTransform>();

                    if(viewPort == null)
                    {
                        SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDetailsDisplay@OnScrollDownPressed missing ViewPort for ItemDetailsDisplay");
                        return true;
                    }
                    __instance.m_viewPortSize = viewPort.rect.height;
                }

                float scrollAmount = __instance.m_viewPortSize / __instance.m_contentScrollView.content.rect.height;
                __instance.m_targetScrollPos = Mathf.Clamp(__instance.m_contentScrollView.verticalNormalizedPosition - scrollAmount, 0f, 1f);
                __instance.m_movingScrollview = true;
                __instance.m_btnScrollDown.interactable = false;
                __instance.m_btnScrollUp.interactable = false;

                #if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDetailsDisplay@OnScrollDownPressed called end! scrollAmount: {scrollAmount}" + 
                    $" viewportSize: {__instance.m_viewPortSize} scrollViewHeight: {__instance.m_contentScrollView.content.rect.height}" + 
                    $" normalized: {__instance.m_contentScrollView.verticalNormalizedPosition}");
                
                #endif

                return false; // Block the original method from executing (because we handled everything)
            }
        }

        [HarmonyPatch(typeof(ItemDetailsDisplay), "OnScrollUpPressed")]
        public class Patch_OnScrollUpPressed
        {
            static bool Prefix(ItemDetailsDisplay __instance)
            {
                #if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} Patch_OnScrollUpPressed called!");
                #endif
                if (__instance.m_movingScrollview)
                {
                    return false;
                }

                //split screen character has a bug where scroll view port size is not passed let's fix it
                if(__instance.m_viewPortSize == 0)
                {
                    RectTransform viewPort = __instance.transform.Find("ItemDetails/Stats/Scroll View/Viewport").GetComponent<RectTransform>();

                    if(viewPort == null)
                    {
                        SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDetailsDisplay@OnScrollUpPressed missing ViewPort for ItemDetailsDisplay");
                        return true;
                    }
                    __instance.m_viewPortSize = viewPort.rect.height;
                }

                float scrollAmount = __instance.m_viewPortSize / __instance.m_contentScrollView.content.rect.height;
                __instance.m_targetScrollPos = Mathf.Clamp(__instance.m_contentScrollView.verticalNormalizedPosition + scrollAmount, 0f, 1f);
                __instance.m_movingScrollview = true;
                __instance.m_btnScrollDown.interactable = false;
                __instance.m_btnScrollUp.interactable = false;

                #if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} Patch_OnScrollUpPressed called ends! scrollAmount: {scrollAmount}" + 
                    $" viewportSize: {__instance.m_viewPortSize} scrollViewHeight: {__instance.m_contentScrollView.content.rect.height}" + 
                    $" normalized: {__instance.m_contentScrollView.verticalNormalizedPosition}");
                #endif

                return false; // Block the original method since we handled everything
            }
        }

        [HarmonyPatch(typeof(ResourcesPrefabManager), nameof(ResourcesPrefabManager.Load))]
        public class ResourcesPrefabManager_Load
        {
            static void Postfix(ResourcesPrefabManager __instance)
            {
                #if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ResourcesPrefabManager@Load called!");
                #endif

                string path = Path.Combine(
                    XmlSerializerHelper.GetProjectLocation(),
                    "customEnchantmentsDescriptions.xml"
                );

                CustomEnchantmentsManager.Instance.LoadEnchantmentDictionaryFromXml(
                    path
                );

                EnchantmentsHelper.FixFilterRecipe();
            }
        }

        //only in StartInit parents get their names
        //in AwakeInit they still not assigned
        [HarmonyPatch(typeof(ItemDetailsDisplay))]
        [HarmonyPatch("StartInit", MethodType.Normal)]
        public class ItemDetailsDisplay_AwakeInit
        {
            //For adding additional section in inventory UI
            [HarmonyPostfix]
            private static void Postfix(ItemDetailsDisplay __instance)
            {
                try
                {
                    #if DEBUG
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDetailsDisplay@StartInit called!");
                    #endif
                    new ItemDescriptionScrollFixer(__instance);
                    ItemDisplayManager.Instance.TryCreateSection(__instance);
                }
                catch(Exception e)
                {
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDetailsDisplay@StartInit error: {e.Message}");
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
                    #if DEBUG
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDetailsDisplay@RefreshDetails called!");
                    #endif

                    Item item = __instance.itemDisplay?.RefItem;
                    ItemDisplayManager.Instance.ShowOriginalDescription(__instance);

                    CharacterInventory inventory = __instance.LocalCharacter?.Inventory;

                    if (!inventory)
                    {
                        #if DEBUG
                        SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDetailsDisplay@RefreshDetails missing inventory from ItemDisplay!");
                        #endif
                        return;
                    }

                    //checks if item is not being sold
                    if (OutwardEnchantmentsViewer.ShowDescriptionsOnlyForInventory.Value &&
                        item.m_lastParentItemContainer != null && item.m_lastParentItemContainer is MerchantPouch)
                    {
                        ItemDisplayManager.Instance.HideDescription(__instance);
                        ItemDisplayManager.Instance.HideDisabledDescription(__instance);
                        return;
                    }

                    switch(item)
                    {
                        case Equipment equipment:
                            {
                                if (!ShowEquipmentDescriptions.Value || item.IsNonEnchantable) //|| equipment.EquipSlot == EquipmentSlot.EquipmentSlotIDs.Quiver)
                                {
                                    ItemDisplayManager.Instance.HideDescription(__instance);
                                    ItemDisplayManager.Instance.HideDisabledDescription(__instance);
                                    return;
                                }
                                ItemDescriptionsManager.Instance.SetEquipmentsEnchantmentsDescription(item, inventory, __instance);
                                break;
                            }
                        case EnchantmentRecipeItem enchantmentRecipeItem:
                            {
                                if (!ShowEnchantmentDescriptions.Value)
                                {
                                    ItemDisplayManager.Instance.HideDescription(__instance);
                                    ItemDisplayManager.Instance.HideDisabledDescription(__instance);
                                    return;
                                }
                                ItemDescriptionsManager.Instance.SetEnchantmentsDescription(enchantmentRecipeItem, inventory, __instance);
                                ItemDisplayManager.Instance.HideDisabledDescription(__instance);
                                break;
                            }
                        default:
                            {
                                ItemDisplayManager.Instance.HideDescription(__instance);
                                ItemDisplayManager.Instance.HideDisabledDescription(__instance);
                                return;
                            }
                    }
                }
                catch(Exception e)
                {
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDetailsDisplay@RefreshDetails error: {e.Message}");
                }
            }
        }
    }
}
