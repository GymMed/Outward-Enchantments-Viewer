using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OutwardEnchantmentsViewer.Utility.Helpers
{
    public class XmlSerializerHelper
    {
        public static string GetProjectLocation()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static T DeserializeXML<T>(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            
            // Open the XML file and read it
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                return (T)serializer.Deserialize(fs);
            }
        }
    }
}
