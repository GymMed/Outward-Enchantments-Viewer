using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardEnchantmentsViewer.UI
{
    public class AdditionalTextContainer
    {
        List<Transform> _additionalTexts = new List<Transform>();
        Transform _additionalTextsContainer;

        public List<Transform> AdditionalTexts { get => _additionalTexts; set => _additionalTexts = value; }
        public Transform AdditionalTextsContainer { get => _additionalTextsContainer; set => _additionalTextsContainer = value; }

        public AdditionalTextContainer()
        {

        }

        public void ShowAdditionalTextsContainer()
        {
            AdditionalTextsContainer.gameObject.SetActive(true);
        }

        public void HideAdditionalTextsContainer()
        {
            AdditionalTextsContainer.gameObject.SetActive(false);
        }
    }
}
