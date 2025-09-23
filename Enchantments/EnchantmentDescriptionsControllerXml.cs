using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OutwardEnchantmentsViewer.Enchantments
{
    [Serializable]
    [XmlRoot("root")]
    public class EnchantmentDescriptionsControllerXml
    {
        [XmlArray("enchantments")]
        [XmlArrayItem("enchantment")]
        public List<EnchantmentEntryJson> enchantments { get; set; }

        public Dictionary<int, EnchantmentDescription> ToDictionary()
        {
            var dictionary = new Dictionary<int, EnchantmentDescription>();

            foreach (var enchantment in enchantments)
            {
                dictionary[enchantment.id] = enchantment.data;
            }

            return dictionary;
        }
    }
}
