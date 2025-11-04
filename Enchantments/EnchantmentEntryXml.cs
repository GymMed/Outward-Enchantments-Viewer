using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OutwardEnchantmentsViewer.Enchantments
{
    [Serializable]
    public class EnchantmentEntryXml
    {
        [XmlElement("id")]
        public int id { get; set; }
        [XmlElement("data")]
        public EnchantmentDescription data { get; set; }
    }
}
