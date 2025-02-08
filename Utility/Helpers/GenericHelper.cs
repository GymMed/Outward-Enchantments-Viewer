using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardEnchantmentsViewer.Utility.Helpers
{
    public class GenericHelper
    {
        public static void SplitDerivedClasses<TBase, TDerived>(
            TBase[] sourceArray, 
            out TBase[] remainingArray, 
            out TDerived[] derivedArray
        )
        {
            List<TDerived> derivedList = new List<TDerived>();
            List<TBase> remainingList = new List<TBase>();

            foreach (var item in sourceArray)
            {
                if (item is TDerived derivedItem)
                {
                    derivedList.Add(derivedItem);
                }
                else
                {
                    remainingList.Add(item);
                }
            }

            derivedArray = derivedList.ToArray();
            remainingArray = remainingList.ToArray();
        }

        public static int CountDerivedClassesInArray<T>(T[] array, Type TypeToCount)
        {
            int totalDerviedClasses = 0;

            foreach(var item in array) 
            {
                if(item != null && TypeToCount.IsAssignableFrom(item.GetType()))
                {
                    totalDerviedClasses++;
                }
            }

            return totalDerviedClasses;
        }

        public static int CountDerivedClassesInList<T>(List<T> list, Type TypeToCount)
        {
            int totalDerviedClasses = 0;

            foreach(var item in list) 
            {
                if(item != null && TypeToCount.IsAssignableFrom(item.GetType()))
                {
                    totalDerviedClasses++;
                }
            }

            return totalDerviedClasses;
        }

        public static void TryCleaningGameObjectWithNames(Transform parent, string[] elementNames)
        {
            foreach (string elementName in elementNames)
            {
                TryCleaningGameObjectWithName(parent, elementName);
            }
        }

        public static void TryCleaningGameObjectWithName(Transform parent, string elementName)
        {
            Transform element = parent.Find(elementName);

            if (!element)
                return;

            GameObject.DestroyImmediate(element.gameObject);
        }
    }
}
