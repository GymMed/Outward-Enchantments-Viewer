using Mono.Cecil;
using OutwardEnchanmentsViewer.Enums;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OutwardEnchanmentsViewer.UI
{
    public class ItemDisplaySection
    {
        // My personal description and separator
        Transform _separator;
        Transform _description;

        Transform _originalSeparator;
        Transform _originalDescription;

        Text _descriptionText;
        Row _headerRow;

        public ItemDisplaySection(CharacterUI characterUI)
        {
            CreateSection(characterUI);
            HideDescription();
        }

        public Transform Separator { get => _separator; set => _separator = value; }
        public Transform Description { get => _description; set => _description = value; }

        public Text DescriptionText { get => _descriptionText; set => _descriptionText = value; }
        public Row HeaderRow { get => _headerRow; set => _headerRow = value; }

        public Transform OriginalSeparator { get => _originalSeparator; set => _originalSeparator = value; }
        public Transform OriginalDescription { get => _originalDescription; set => _originalDescription = value; }

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
                SL.Log($"{OutwardEnchanmentsViewer.prefix} ItemDisplaySection@SetDescriptiontext trying to change description text on missing text component!");
                return;
            }

            DescriptionText.text = text;
        }

        private void CreateSection(CharacterUI characterUI)
        {
            Transform itemDescriptionUI = characterUI.transform.Find("Canvas/GameplayPanels/Menus/CharacterMenus/MainPanel/Content/MiddlePanel/Inventory/DetailPanel/ItemDetailsPanel/ItemDetails/Stats/Scroll View/Viewport");

            if(!itemDescriptionUI)
            {
                SL.Log($"{OutwardEnchanmentsViewer.prefix} ItemDisplaySection@CreateSection couldn't find Item Description UI!");
                return;
            }

            OriginalSeparator = itemDescriptionUI.Find("Content/Separator");
            OriginalDescription = itemDescriptionUI.Find("Content/Description");

            if(!OriginalSeparator || !OriginalDescription)
            {
                SL.Log($"{OutwardEnchanmentsViewer.prefix} ItemDisplaySection@CreateSection couldn't find Separator or Description in Item Description UI!");
                return;
            }

            Separator = UnityEngine.Object.Instantiate(OriginalSeparator, OriginalSeparator.parent);

            Row.TextFont = OriginalDescription.Find("lblDescription")?.GetComponent<Text>()?.font;
            HeaderRow = new Row(OriginalSeparator.parent);

            Description = UnityEngine.Object.Instantiate(OriginalDescription, OriginalDescription.parent);

            Separator.name = "gymmed-Separator";
            Description.name = "gymmed-Description";
            HeaderRow.GameObject.SetActive(false);

            Transform textTransform = Description.Find("lblDescription");

            if(!textTransform)
            {
                SL.Log($"{OutwardEnchanmentsViewer.prefix} ItemDisplaySection@CreateSection copied description doesn't have text child");
                return;
            }

            DescriptionText = textTransform.GetComponent<Text>();
            //textTransform.GetComponent<>;
            SL.Log($"{OutwardEnchanmentsViewer.prefix} ItemDisplaySection@CreateSection made description and separator!");
        }

    }
}
