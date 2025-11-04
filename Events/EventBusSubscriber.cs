using OutwardEnchantmentsViewer.Managers;
using OutwardModsCommunicator.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardEnchantmentsViewer.Events
{
    public static class EventBusSubscriber
    {
        public const string Event_LoadCustomEnchantmentsDescriptionsXml = "LoadCustomEnchantmentsDescriptionsXml";

        public static void AddSubscribers()
        {
            EventBus.Subscribe(OutwardEnchantmentsViewer.EVENT_BUS_ALL_GUID, Event_LoadCustomEnchantmentsDescriptionsXml, LoadCustomEnchantmentsDescriptionsXml);
        }

        public static void LoadCustomEnchantmentsDescriptionsXml(EventPayload payload)
        {
            if (payload == null) return;

            string xmlFilePath = payload.Get<string>("filePath", null);

            if(string.IsNullOrEmpty(xmlFilePath))
            {
                OutwardEnchantmentsViewer.Log.LogMessage($"EventBusSubscriber@LoadCustomEnchantmentsDescriptionsXml didn't receive {xmlFilePath} variable! Cannot add custom enchantmentsDescriptions!");
                return;
            }

            CustomEnchantmentsManager.Instance.LoadEnchantmentDictionaryFromXml(xmlFilePath);
        }
    }
}
