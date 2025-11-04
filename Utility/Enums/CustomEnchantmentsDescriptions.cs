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

    // Currently not needed, default descriptions in game are provided.
    public static class CustomEnchantmentsDescriptionsExtensions
    {
        //The difference between these descriptions and CustomEnchantmentsManager is that these are not retrieved from json
        public static readonly Dictionary<CustomEnchantmentsDescriptions, string> CustomEnchantmentRecipesDescriptions = new Dictionary<CustomEnchantmentsDescriptions, string>()
        {
            { CustomEnchantmentsDescriptions.AngelLight, "Changes the color of the lantern to white and effects of Flamethrower to Electric. \n" + 
                "Lantern can no longer be refueled, however it passively gains fuel when equipped but not lit." },
            { CustomEnchantmentsDescriptions.BlazeBlue, "Changes the color of the lantern to blue. \n" +
                "Lantern can no longer be refueled, however it passively gains fuel when equipped but not lit." },
            { CustomEnchantmentsDescriptions.CopperFlame, "Changes the color of the lantern to green and effects of Flamethrower to Rust. \n" +
                "Lantern can no longer be refueled, however it passively gains fuel when equipped but not lit." },
            { CustomEnchantmentsDescriptions.SanguineFlame, "Changes the color of the lantern to Red and effects of Flamethrower to Decay. \n" +
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
