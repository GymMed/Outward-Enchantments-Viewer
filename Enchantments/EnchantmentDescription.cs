using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OutwardEnchantmentsViewer.Enchantments
{
    [Serializable]
    public class EnchantmentDescription
    {
        //Can be additional information or complete overwrite
        [XmlElement("overwrite")]
        public bool overwrite { get; set; }
        [XmlElement("description")]
        public string description { get; set; }
    }
}
