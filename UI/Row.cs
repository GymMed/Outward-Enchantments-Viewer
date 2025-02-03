using OutwardEnchanmentsViewer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OutwardEnchanmentsViewer.UI
{
    public class Row 
    {
        Text _rightText;
        Text _leftText;
        GameObject _gameObject;

        public Text RightText { get => _rightText; set => _rightText = value; }
        public Text LeftText { get => _leftText; set => _leftText = value; }
        public GameObject GameObject { get => _gameObject; set => _gameObject = value; }

        public static Font TextFont;

        public Row(Transform parentTransform)
        {
            GameObject = new GameObject("gymmed-Row");
            RectTransform parentRect = GameObject.AddComponent<RectTransform>();
            parentRect.SetParent(parentTransform, false);
            
            // Add Horizontal Layout Group to Parent
            HorizontalLayoutGroup layoutGroup = GameObject.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childAlignment = TextAnchor.LowerLeft;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.spacing = 10;

            // Create first text element
            GameObject child1 = CreateTextChild("LeftText", "Left Side", GameObject.transform, TextAnchor.LowerLeft);
            LeftText = child1.GetComponent<Text>();

            // Create second text element
            GameObject child2 = CreateTextChild("RightText", "Right Side", GameObject.transform, TextAnchor.LowerRight);
            RightText = child2.GetComponent<Text>();
        }

        public void ChangeRightText(string text)
        {
            RightText.text = text;
        }

        public void ChangeLeftText(string text)
        {
            LeftText.text = text;
        }

        private GameObject CreateTextChild(string name, string textValue, 
            Transform parent, TextAnchor anchor = TextAnchor.MiddleCenter, bool isFlexible = false)
        {
            GameObject container = new GameObject($"gymmed-{name}-container");
            RectTransform containerRectTransform = container.AddComponent<RectTransform>();
            containerRectTransform.SetParent(parent, false);

            HorizontalLayoutGroup layoutGroup = container.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childAlignment = anchor;
            layoutGroup.childControlWidth = false; 
            layoutGroup.childForceExpandWidth = false;

            // Create the child GameObject
            GameObject child = new GameObject(name);
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.SetParent(container.transform, false);

            // Add a Text component
            Text textComponent = child.AddComponent<Text>();
            textComponent.text = textValue;
            textComponent.alignment = anchor;
            textComponent.font = TextFont ? TextFont : Resources.GetBuiltinResource<Font>("Ariel.tff"); // Default font
            textComponent.fontSize = 19;
            textComponent.color = StatColorExtensions.GetColor(StatColor.Enchantment);

            textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            textComponent.verticalOverflow = VerticalWrapMode.Overflow;

            LayoutElement layoutElement = child.AddComponent<LayoutElement>();
            layoutElement.minWidth = 50; // Prevent it from shrinking too much
            
            if(!isFlexible)
                layoutElement.flexibleWidth = 0; // Do not stretch
            else
                layoutElement.flexibleWidth = 1;

            return child;
        }
    }
}
