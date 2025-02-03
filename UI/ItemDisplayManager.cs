using Mono.Cecil;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardEnchanmentsViewer.UI
{
    public class ItemDisplayManager
    {
        Dictionary<CharacterUI, ItemDisplaySection> _dictionaryDisplaySections = new Dictionary<CharacterUI, ItemDisplaySection>();
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

        public Dictionary<CharacterUI, ItemDisplaySection> DictionaryDisplaySections { get => _dictionaryDisplaySections; set => _dictionaryDisplaySections = value; }

        public void CreateSection(CharacterUI characterUI)
        {
            DictionaryDisplaySections.Add(characterUI, new ItemDisplaySection(characterUI));
        }

        public void ShowDescription(CharacterUI characterUI)
        {
            GetItemDisplaySection(characterUI).ShowDescription();
        }

        public void HideDescription(CharacterUI characterUI)
        {
            GetItemDisplaySection(characterUI).HideDescription();
        }

        public void SetDescriptionText(CharacterUI characterUI, string text)
        {
            GetItemDisplaySection(characterUI).SetDescriptiontext(text);
        }

        public void SetHeaderText(CharacterUI characterUI, string leftText, string rightText)
        {
            GetItemDisplaySection(characterUI).SetHeaderText(leftText, rightText);
        }

        public ItemDisplaySection GetItemDisplaySection(CharacterUI characterUI)
        {
            ItemDisplaySection itemSectionUI = DictionaryDisplaySections[characterUI];

            if (itemSectionUI == null)
                throw new Exception("Tried to retrieve missing characterUI from dictionary!");

            return itemSectionUI;
        }
    }
}
