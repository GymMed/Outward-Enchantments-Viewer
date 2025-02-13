using Mono.Cecil;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OutwardEnchantmentsViewer.Utility.Fixers
{
    public class ItemDescriptionScrollFixer
    {
        ItemDetailsDisplay _itemDetailsDisplay;

        Button _btnScrollUpButton;
        Button _btnScrollDownButton;
        ScrollRect _scrollView;
        RectTransform _viewport;

        public Button BtnScrollUpButton { get => _btnScrollUpButton; set => _btnScrollUpButton = value; }
        public Button BtnScrollDownButton { get => _btnScrollDownButton; set => _btnScrollDownButton = value; }
        public ScrollRect ScrollView { get => _scrollView; set => _scrollView = value; }
        public RectTransform Viewport { get => _viewport; set => _viewport = value; }
        public ItemDetailsDisplay ItemDetailsDisplay { get => _itemDetailsDisplay; set => _itemDetailsDisplay = value; }

        public ItemDescriptionScrollFixer(ItemDetailsDisplay itemDetailsDisplay) 
        {
            ItemDetailsDisplay = itemDetailsDisplay;
            CreateFixer(itemDetailsDisplay);
        }

        public void CreateFixer(ItemDetailsDisplay itemDetailsDisplay)
        {
            Transform itemDetailsUI = itemDetailsDisplay.transform.Find("ItemDetails/");
                #if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionScrollFixer@CreateFixer called!");
                #endif

            if(!itemDetailsUI)
            {
                #if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionScrollFixer@CreateFixer couldn't find Item Details UI!");
                #endif
                return;
            }

            BtnScrollUpButton = itemDetailsUI.Find("btnScrollUp")?.GetComponent<Button>();
            BtnScrollDownButton = itemDetailsUI.Find("btnScrollDown")?.GetComponent<Button>();

            ScrollView = itemDetailsUI.Find("Stats/Scroll View")?.GetComponent<ScrollRect>();
            Viewport = ScrollView?.viewport;

            if(!BtnScrollDownButton || !BtnScrollUpButton || !ScrollView || !Viewport)
            {
                #if DEBUG
                SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionScrollFixer@CreateFixer couldn't find one of the scroll UI parts!");
                #endif
                return;
            }

            BtnScrollUpButton.onClick.RemoveAllListeners();
            BtnScrollDownButton.onClick.RemoveAllListeners();
            //ScrollView.onValueChanged.RemoveAllListeners();

            BtnScrollDownButton.onClick = new Button.ButtonClickedEvent();
            BtnScrollUpButton.onClick = new Button.ButtonClickedEvent();

            BtnScrollUpButton.GetComponent<Button>().onClick.AddListener(ScrollUp);
            BtnScrollDownButton.GetComponent<Button>().onClick.AddListener(ScrollDown);

            #if DEBUG
            SL.Log($"{OutwardEnchantmentsViewer.prefix} ItemDescriptionScrollFixer@CreateFixer finnished!");
            #endif
        }

        private void ScrollUp()
        {
            // Move up by Viewport height
            float scrollAmount = Viewport.rect.height / ScrollView.content.rect.height;
            ScrollView.verticalNormalizedPosition = Mathf.Clamp(ScrollView.verticalNormalizedPosition + scrollAmount, 0f, 1f);
        }

        private void ScrollDown()
        {
            // Move down by Viewport height
            float scrollAmount = Viewport.rect.height / ScrollView.content.rect.height;
            ScrollView.verticalNormalizedPosition = Mathf.Clamp(ScrollView.verticalNormalizedPosition - scrollAmount, 0f, 1f);
        }
    }
}
