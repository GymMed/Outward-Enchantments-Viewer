using Mono.Cecil;
using OutwardEnchantmentsViewer.UI;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardEnchantmentsViewer.Managers
{
    public class ItemDisplayManager
    {
        Dictionary<ItemDetailsDisplay, ItemDisplaySection> _dictionaryDisplaySections = new Dictionary<ItemDetailsDisplay, ItemDisplaySection>();
        private static ItemDisplayManager _instance;

        private ItemDisplayManager()
        {
        }

        public static ItemDisplayManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ItemDisplayManager();

                return _instance;
            }
        }

        public Dictionary<ItemDetailsDisplay, ItemDisplaySection> DictionaryDisplaySections { get => _dictionaryDisplaySections; set => _dictionaryDisplaySections = value; }

        public void TryCreateSection(ItemDetailsDisplay itemDetailsDisplay)
        {
            DictionaryDisplaySections.TryGetValue(itemDetailsDisplay, out ItemDisplaySection itemSectionUI);

            if (itemSectionUI != null)
                return;

            DictionaryDisplaySections.Add(itemDetailsDisplay, new ItemDisplaySection(itemDetailsDisplay));
        }

        public void ShowDescriptionContainer(ItemDetailsDisplay itemDetailsDisplay, bool hideOrginalDescription = false)
        {
            ItemDisplaySection itemDisplaySection = GetItemDisplaySection(itemDetailsDisplay);
            itemDisplaySection.ShowDescription();

            if(hideOrginalDescription) 
            {
                itemDisplaySection.HideOriginalDescription();
            }
        }

        public void ShowDisabledDescription(ItemDetailsDisplay itemDetailsDisplay)
        {
            GetItemDisplaySection(itemDetailsDisplay).ShowDisabledDescription();
        }

        public void HideDisabledDescription(ItemDetailsDisplay itemDetailsDisplay)
        {
            GetItemDisplaySection(itemDetailsDisplay).HideDisabledDescription();
        }

        public void ShowOriginalDescription(ItemDetailsDisplay itemDetailsDisplay)
        {
            GetItemDisplaySection(itemDetailsDisplay).ShowOriginalDescription();
        }

        public void HideOriginalDescription(ItemDetailsDisplay itemDetailsDisplay)
        {
            GetItemDisplaySection(itemDetailsDisplay).HideOriginalDescription();
        }

        public void HideDescription(ItemDetailsDisplay itemDetailsDisplay)
        {
            GetItemDisplaySection(itemDetailsDisplay).HideDescription();
        }

        public void SetDescriptionText(ItemDetailsDisplay itemDetailsDisplay, string text)
        {
            GetItemDisplaySection(itemDetailsDisplay).SetDescriptiontext(text);
        }

        public void SetDisabledDescriptionText(ItemDetailsDisplay itemDetailsDisplay, string text)
        {
            GetItemDisplaySection(itemDetailsDisplay).SetDisabledDescriptiontext(text);
        }

        public void SetHeaderText(ItemDetailsDisplay itemDetailsDisplay, string leftText, string rightText)
        {
            GetItemDisplaySection(itemDetailsDisplay).SetHeaderText(leftText, rightText);
        }

        public ItemDisplaySection GetItemDisplaySection(ItemDetailsDisplay itemDetailsDisplay)
        {
            DictionaryDisplaySections.TryGetValue(itemDetailsDisplay, out ItemDisplaySection itemSectionUI);

            if (itemSectionUI == null)
                throw new Exception("Tried to retrieve missing itemDetailsDisplay from dictionary!");

            return itemSectionUI;
        }
    }
}
