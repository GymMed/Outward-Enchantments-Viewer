using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardEnchantmentsViewer.Enchantments
{
    public class EnchantmentRecipeDetailedData
    {
        public EnchantmentRecipeData Data { get; }
        public int Count { get; set; }

        public EnchantmentRecipeDetailedData(EnchantmentRecipeData data, int count = 1)
        {
            this.Data = data;
            this.Count = count;
        }
    }
}
