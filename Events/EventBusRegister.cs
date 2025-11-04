using OutwardModsCommunicator.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardEnchantmentsViewer.Events
{
    public static class EventBusRegister
    {
        public static void RegisterEvents()
        {
            EventBus.RegisterEvent(
                OutwardEnchantmentsViewer.EVENT_BUS_ALL_GUID,
                EventBusSubscriber.Event_LoadCustomEnchantmentsDescriptionsXml,
                "Listens for event, when triggered deserializes xml from file path and inserts enhcantment descriptions.",
                ("filePath", typeof(string), "xml file path location")
            );
        }
    }
}
