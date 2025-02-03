using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OutwardEnchanmentsViewer.Enums
{
    public enum StatColor
    {
        Default,
        Health,
        Stamina,
        Mana,
        Needs,
        Corruption,
        StatusEffect,
        StatusCures,
        Enchantment
    }

    public static class StatColorExtensions
    {
        public static readonly Dictionary<StatColor, Color> Colors = new()
        {
            { StatColor.Default, new Color(0.8627f, 0.8627f, 0.8627f, 1f) },
            { StatColor.Health, new Color(0.765f, 0.522f, 0.525f, 1f) },
            { StatColor.Stamina, new Color(0.827f, 0.757f, 0.584f, 1f) },
            { StatColor.Mana, new Color(0.529f, 0.702f, 0.816f, 1f) },
            { StatColor.Needs, new Color(0.584f, 0.761f, 0.522f, 1f) },
            { StatColor.Corruption, new Color(0.655f, 0.647f, 0.282f, 1f) },
            { StatColor.StatusEffect, new Color(0.780f, 1f, 0.702f, 1f) },
            { StatColor.StatusCures, new Color(1f, 0.702f, 0.706f, 1f) },
            { StatColor.Enchantment, new Color(0.961f, 0.157f, 0.569f, 1f) }
        };

        public static Color GetColor(this StatColor stat)
        {
            return Colors[stat];
        }
    }
}
