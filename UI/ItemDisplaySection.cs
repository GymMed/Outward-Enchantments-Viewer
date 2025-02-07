using Mono.Cecil;
using OutwardEnchantmentsViewer.Utility.Enums;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OutwardEnchantmentsViewer.UI
{
    public class ItemDisplaySection
    {
        // My personal description and separator
        Transform _separator;
        Transform _description;
        Transform _disabledDescription;

        Transform _originalSeparator;
        Transform _originalDescription;

        Text _descriptionText;
        Text _disabledDescriptionText;
        Row _headerRow;

        public ItemDisplaySection(CharacterUI characterUI)
        {
            CreateSection(characterUI);
            HideDescription();
        }

        public Transform Separator { get => _separator; set => _separator = value; }
        public Transform Description { get => _description; set => _description = value; }

        public Text DescriptionText { get => _descriptionText; set => _descriptionText = value; }
        public Text DisabledDescriptionText { get => _disabledDescriptionText; set => _disabledDescriptionText = value; }
        public Row HeaderRow { get => _headerRow; set => _headerRow = value; }

        public Transform OriginalSeparator { get => _originalSeparator; set => _originalSeparator = value; }
        public Transform OriginalDescription { get => _originalDescription; set => _originalDescription = value; }
        public Transform DisabledDescription { get => _disabledDescription; set => _disabledDescription = value; }

        public void ShowDescription()
        {
            Separator.gameObject.SetActive(true);
            Description.gameObject.SetActive(true);
            HeaderRow.GameObject.SetActive(true);
        }

        public void HideDescription()
        {
            Separator.gameObject.SetActive(false);
            Description.gameObject.SetActive(false);
            HeaderRow.GameObject.SetActive(false);
        }

        public void ShowDisabledDescription()
        {
            DisabledDescription.gameObject.SetActive(true);
        }

        public void HideDisabledDescription()
        {
            DisabledDescription.gameObject.SetActive(false);
        }

        public void ShowOriginalDescription()
        {
            OriginalSeparator.gameObject.SetActive(true);
            OriginalDescription.gameObject.SetActive(true);
        }

        //for empty item descriptions
        //looks more visually appealing
        public void HideOriginalDescription()
        {
            OriginalSeparator.gameObject.SetActive(false);
            OriginalDescription.gameObject.SetActive(false);
        }

        public void SetHeaderText(string leftText = null, string rightText = null)
        {
            if (HeaderRow == null) 
            {
                return;
            }

            if(leftText != null)
            {
                HeaderRow.ChangeLeftText(leftText);
            }

            if(rightText != null)
            {
                HeaderRow.ChangeRightText(rightText);
            }
        }

        public void SetDescriptiontext(string text)
        {
            if(!DescriptionText)
            {
                #if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDisplaySection@SetDescriptiontext trying to change description text on missing text component!");
                #endif
                return;
            }

            DescriptionText.text = text;
        }

        public void SetDisabledDescriptiontext(string text)
        {
            if(!DisabledDescriptionText)
            {
                #if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDisplaySection@SetDisabledDescriptiontext trying to change description text on missing text component!");
                #endif
                return;
            }

            DisabledDescriptionText.text = text;
        }

        private void CreateSection(CharacterUI characterUI)
        {
            Transform itemDescriptionUI = characterUI.transform.Find("Canvas/GameplayPanels/Menus/CharacterMenus/MainPanel/Content/MiddlePanel/Inventory/DetailPanel/ItemDetailsPanel/ItemDetails/Stats/Scroll View/Viewport");

            if(!itemDescriptionUI)
            {
                #if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDisplaySection@CreateSection couldn't find Item Description UI!");
                #endif
                return;
            }

            OriginalSeparator = itemDescriptionUI.Find("Content/Separator");
            OriginalDescription = itemDescriptionUI.Find("Content/Description");

            if(!OriginalSeparator || !OriginalDescription)
            {
                #if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDisplaySection@CreateSection couldn't find Separator or Description in Item Description UI!");
                #endif
                return;
            }

            Separator = UnityEngine.Object.Instantiate(OriginalSeparator, OriginalSeparator.parent);

            Row.TextFont = OriginalDescription.Find("lblDescription")?.GetComponent<Text>()?.font;
            HeaderRow = new Row(OriginalSeparator.parent);

            Description = UnityEngine.Object.Instantiate(OriginalDescription, OriginalDescription.parent);
            DisabledDescription = UnityEngine.Object.Instantiate(OriginalDescription, OriginalDescription.parent);

            Separator.name = "gymmed-Separator";
            Description.name = "gymmed-Description";
            DisabledDescription.name = "gymmed-Disabled-Description";
            HeaderRow.GameObject.SetActive(false);

            Transform textTransform = Description.Find("lblDescription");

            if(!textTransform)
            {
                #if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDisplaySection@CreateSection copied description doesn't have text child");
                #endif
                return;
            }

            Transform disabledTextTransform = DisabledDescription.Find("lblDescription");

            if(!disabledTextTransform)
            {
                #if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDisplaySection@CreateSection copied disabled description doesn't have text child");
                #endif
                return;
            }

            DescriptionText = textTransform.GetComponent<Text>();
            DisabledDescriptionText = disabledTextTransform.GetComponent<Text>();
            DisabledDescriptionText.color = StatColorExtensions.GetColor(StatColor.Disabled);

            #if DEBUG
            SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDisplaySection@CreateSection made description and separator!");
            #endif
        }

    }
}
