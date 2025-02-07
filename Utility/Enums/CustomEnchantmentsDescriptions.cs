using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardEnchantmentsViewer.Utility.Enums
{
    public enum CustomEnchantmentsDescriptions
    {
        AngelLight = 20,
        BlazeBlue = 21,
        CopperFlame = 19,
        SanguineFlame = 18,
        //debug filter 52
    }

    public static class CustomEnchantmentsDescriptionsExtensions
    {
        //id
        public static readonly Dictionary<CustomEnchantmentsDescriptions, string> CustomEnchantmentRecipesDescriptions = new Dictionary<CustomEnchantmentsDescriptions, string>()
        {
            { CustomEnchantmentsDescriptions.AngelLight, "Changes the color of the lantern to white and effects of Flamethrower to Electric. \n\n" + 
                "Lantern can no longer be refueled, however it passively gains fuel when equipped but not lit." },
            { CustomEnchantmentsDescriptions.BlazeBlue, "Changes the color of the lantern to blue and effects of Flamethrower to Frost. \n\n" +
                "Lantern can no longer be refueled, however it passively gains fuel when equipped but not lit." },
            { CustomEnchantmentsDescriptions.CopperFlame, "Changes the color of the lantern to green and effects of Flamethrower to Rust. \n\n" +
                "Lantern can no longer be refueled, however it passively gains fuel when equipped but not lit." },
            { CustomEnchantmentsDescriptions.SanguineFlame, "Changes the color of the lantern to Red and effects of Flamethrower to Decay Decay. \n\n" +
                "Lantern can no longer be refueled, however it passively gains fuel when equipped but not lit." }
        };

        public static string GetDescription(this CustomEnchantmentsDescriptions enchantmentEnum)
        {
            return CustomEnchantmentRecipesDescriptions[enchantmentEnum];
        }

        public static string GetDescription(int recipeID)
        {
            string description = "";

            if (Enum.IsDefined(typeof(CustomEnchantmentsDescriptions), recipeID) &&
                CustomEnchantmentRecipesDescriptions.TryGetValue((CustomEnchantmentsDescriptions)recipeID, out description))
            {
                return description;
            }
            return description;
        }
    }

}
