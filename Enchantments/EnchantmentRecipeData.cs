using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardEnchantmentsViewer.Enchantments
{
    //EnchantmentRecipe cannot reference EnchantmentRecipeItem
    //Only EnchantmentRecipeItem can reference EnchantmentRecipe
    //Grouping is most effeciant way to manage them and to keep the reference
    public class EnchantmentRecipeData
    {
        public EnchantmentRecipeItem item;
        public EnchantmentRecipe enchantmentRecipe;

        public EnchantmentRecipeData(EnchantmentRecipeItem item, EnchantmentRecipe enchantmentRecipe)
        {
            this.item = item;
            this.enchantmentRecipe = enchantmentRecipe;
        }
    }
}
