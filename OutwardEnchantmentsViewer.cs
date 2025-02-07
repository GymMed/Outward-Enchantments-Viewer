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

namespace OutwardEnchantmentsViewer
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class OutwardEnchantmentsViewer : BaseUnityPlugin
    {
        public const string GUID = "gymmed.outwardenchantmentsviewer";
        // Choose a NAME for your project, generally the same as your Assembly Name.
        public const string NAME = "Outward Enchantments Viewer";

        public const string VERSION = "0.0.1";

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
        public static ConfigEntry<bool> ExampleConfig;

        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Log = this.Logger;
            Log.LogMessage($"Hello world from {NAME} {VERSION}!");
            Log.LogMessage($"{OutwardEnchantmentsViewer.prefix} Logger!");

            #if DEBUG
            SL.Log($"{OutwardEnchantmentsViewer.prefix} Party hard!");
            #endif

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

        [HarmonyPatch(typeof(ItemDetailsDisplay), "OnScrollDownPressed")]
        public class Patch_OnScrollDownPressed
        {
            static bool Prefix(ItemDetailsDisplay __instance)
            {
                // Ensure we don't run when already moving
                if (__instance.m_movingScrollview)
                {
                    return false;
                }

                float scrollAmount = __instance.m_viewPortSize / __instance.m_contentScrollView.content.rect.height;
                __instance.m_targetScrollPos = Mathf.Clamp(__instance.m_contentScrollView.verticalNormalizedPosition - scrollAmount, 0f, 1f);
                __instance.m_movingScrollview = true;
                __instance.m_btnScrollDown.interactable = false;
                __instance.m_btnScrollUp.interactable = false;

                return false; // Block the original method from executing (because we handled everything)
            }
        }

        [HarmonyPatch(typeof(ItemDetailsDisplay), "OnScrollUpPressed")]
        public class Patch_OnScrollUpPressed
        {
            static bool Prefix(ItemDetailsDisplay __instance)
            {
                if (__instance.m_movingScrollview)
                {
                    return false;
                }

                float scrollAmount = __instance.m_viewPortSize / __instance.m_contentScrollView.content.rect.height;
                __instance.m_targetScrollPos = Mathf.Clamp(__instance.m_contentScrollView.verticalNormalizedPosition + scrollAmount, 0f, 1f);
                __instance.m_movingScrollview = true;
                __instance.m_btnScrollDown.interactable = false;
                __instance.m_btnScrollUp.interactable = false;

                return false; // Block the original method since we handled everything
            }
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
                    #if DEBUG
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} CharacterUI@SetTargetCharacter called!");
                    #endif
                    new ItemDescriptionScrollFixer(__instance);
                    ItemDisplayManager.Instance.CreateSection(__instance);
                }
                catch(Exception e)
                {
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} CharacterUI@SetTargetCharacter error: {e.Message}");
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
                    CharacterUI characterUI = __instance.CharacterUI;

                    if (!characterUI)
                    {
                        #if DEBUG
                        SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDetailsDisplay@RefreshDetails missing CharacterUI on itemDisplay");
                        #endif
                        return;
                    }

                    Item item = __instance.itemDisplay?.RefItem;
                    ItemDisplayManager.Instance.ShowOriginalDescription(characterUI);

                    CharacterInventory inventory = __instance.LocalCharacter?.Inventory;

                    if (!inventory)
                    {
                        #if DEBUG
                        SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDetailsDisplay@RefreshDetails missing inventory from ItemDisplay!");
                        #endif
                        return;
                    }

                    switch(item)
                    {
                        case Equipment equipment:
                            {
                                if (item.IsNonEnchantable)
                                {
                                    ItemDisplayManager.Instance.HideDescription(characterUI);
                                    ItemDisplayManager.Instance.HideDisabledDescription(characterUI);
                                    return;
                                }
                                ItemDescriptionsManager.Instance.SetEquipmentsEnchantmentsDescription(item, inventory, characterUI);
                                break;
                            }
                        case EnchantmentRecipeItem enchantmentRecipeItem:
                            {
                                ItemDescriptionsManager.Instance.SetEnchantmentsDescription(enchantmentRecipeItem, inventory, characterUI);
                                ItemDisplayManager.Instance.HideDisabledDescription(characterUI);
                                break;
                            }
                        default:
                            {
                                ItemDisplayManager.Instance.HideDescription(characterUI);
                                ItemDisplayManager.Instance.HideDisabledDescription(characterUI);
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
