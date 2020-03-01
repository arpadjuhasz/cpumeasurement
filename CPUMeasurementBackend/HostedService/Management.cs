using CPUMeasurementCommon.DataObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.HostedService
{
    public class Management
    {
        public ConcurrentDictionary<IPAddress, ClientData> ConnectedClients = new ConcurrentDictionary<IPAddress, ClientData>();

        public Management()
        {
            
        }

        public void RemoveNotRespondingClients()
        {
            foreach (var item in this.ConnectedClients)
            {
                if (item.Value.LastUpdate.AddSeconds(5) < DateTime.UtcNow)
                {
                    this.ConnectedClients.TryRemove(item.Key, out ClientData value);
                }
            }
        }
    }
}
