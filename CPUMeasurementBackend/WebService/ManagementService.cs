using CPUMeasurementBackend.HostedService;
using CPUMeasurementCommon.DataObjects;
using CPUMeasurementCommon.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.WebService
{
    public class ManagementService
    {
        private readonly Management _management;

        public ManagementService(Management management)
        {
            this._management = management;
        }

        public async Task<List<ClientData>> GetConnectedClients()
        {
            this._management.RemoveNotRespondingClients();
            List<ClientData> result = this._management.ConnectedClients.Values.ToList();
            return result;
        }

        public void UpdateMeasurementInterval(string ip, MeasurementIntervalUpdate dto)
        {
            try
            {
                IPAddress clientAddress = IPAddress.Parse(ip);
                this._management.UpdateMeasurementInterval(clientAddress, dto.MeasurementIntervalInSeconds);
            }
            catch (Exception)
            { 
            
            }
        }

    }
}
