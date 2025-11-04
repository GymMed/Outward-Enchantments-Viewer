using OutwardEnchantmentsViewer.Enchantments;
using OutwardEnchantmentsViewer.Utility.Helpers;
using SideLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OutwardEnchantmentsViewer.Managers
{
    public class CustomEnchantmentsManager
    {
        private static CustomEnchantmentsManager _instance;

        private Dictionary<int, EnchantmentDescription> _enchantmentsDictionary;

        private CustomEnchantmentsManager()
        {
            configPath = Path.Combine(OutwardModsCommunicator.Managers.PathsManager.ConfigPath, "Enchantments_Viewer");
            xmlFilePath = Path.Combine(configPath, "PlayersCustomEnchantmentsDescriptions.xml");
        }

        public static CustomEnchantmentsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CustomEnchantmentsManager();

                return _instance;
            }
        }

        public string configPath = "";
        public string xmlFilePath = "";

        public Dictionary<int, EnchantmentDescription> EnchantmentsDictionary { get => _enchantmentsDictionary; set => _enchantmentsDictionary = value; }

        public EnchantmentDescription TryGetDescription(int RecipeID)
        {
            #if DEBUG
            SL.Log($"{OutwardEnchantmentsViewer.prefix} CustomEnchantmentsManager@TryGetDescription Tried to get enchantment with id: {RecipeID}!");
            #endif
            if(EnchantmentsDictionary == null)
            {
                return null;
            }

            EnchantmentsDictionary.TryGetValue(RecipeID, out EnchantmentDescription enchantmentDescription);

            return enchantmentDescription;
        }

        public void LoadPlayerCustomEnchantmentsDescriptions()
        {
            if (!File.Exists(xmlFilePath))
                return;

            LoadEnchantmentDictionaryFromXml(xmlFilePath);
        }

        public void LoadEnchantmentDictionaryFromXml(string filePath)
        {
            try
            {
                EnchantmentDescriptionsControllerXml container = XmlSerializerHelper.DeserializeXML<EnchantmentDescriptionsControllerXml>(filePath);

                if (container == null || container.enchantments == null || container.enchantments.Count < 1)
                {
                    #if DEBUG
                    SL.Log($"{OutwardEnchantmentsViewer.prefix} CustomEnchantmentsManager@LoadEnchantmentDictionaryFromXML couldn't open enchantments Xml file!");
                    #endif
                    return;
                }

                Dictionary<int, EnchantmentDescription> finalData = container.ToDictionary();

                EnchantmentsDictionary = finalData;
                SL.Log($"{OutwardEnchantmentsViewer.prefix} Successfully loaded xml: {filePath} for enchantment descriptions!");
            }
            catch(Exception ex)
            {
                SL.Log($"{OutwardEnchantmentsViewer.prefix} CustomEnchantmentsManager@LoadEnchantmentDictionaryFromXml error:{ex.Message}!");
            }
        }
    }
}
