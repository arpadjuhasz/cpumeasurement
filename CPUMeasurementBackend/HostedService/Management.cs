using CPUMeasurementCommon.DataObjects;
using CPUMeasurementCommon.Management;
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

        public  void UpdateMeasurementInterval(IPAddress clientAddress, int measurementIntervalInSeconds)
        {
            ClientData clientData = new ClientData();
            clientData = this.ConnectedClients.GetValueOrDefault(clientAddress);
            clientData.UpdateRequested = true;
            if (clientData != null)
            {
                clientData.UpdateRequested = true;
                clientData.MeasurementIntervalRequested = measurementIntervalInSeconds;
            }
        }

        public void AddClientData(IPAddress key, ClientData value)
        {
            this.ConnectedClients.TryAdd(key, value);
        }

        internal MeasurementIntervalUpdatePacket UpdateClientData(IPAddress clientIPAddress, ClientData clientData)
        {
            var exists = !this.ConnectedClients.TryAdd(clientIPAddress, clientData);
            MeasurementIntervalUpdatePacket packet = null;
            if (exists)
            {
                var comparisonClientData = this.ConnectedClients.GetValueOrDefault(clientIPAddress);
                if (comparisonClientData.UpdateRequested)
                {
                    if (comparisonClientData.MeasurementIntervalRequested.GetValueOrDefault() == clientData.MeasurementInterval)
                    {
                        comparisonClientData.UpdateRequested = false;
                        comparisonClientData.MeasurementIntervalRequested = null;
                        comparisonClientData.MeasurementInterval = clientData.MeasurementInterval;
                    }
                    else
                    {
                        packet = new MeasurementIntervalUpdatePacket { MeasurementIntervalInSeconds = comparisonClientData.MeasurementIntervalRequested.GetValueOrDefault() };
                    }
                }
                comparisonClientData.LastUpdate = DateTime.UtcNow;
            }
            return packet;
            
        }
    }
}
