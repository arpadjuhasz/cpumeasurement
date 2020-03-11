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
        private readonly HostedService.Management _management;

        public ManagementService(HostedService.Management management)
        {
            this._management = management;
        }

        public async Task<List<ManagementData>> GetConnectedClients()
        {
            this._management.RemoveNotRespondingClients();
            List<ManagementData> result = this._management.ConnectedClients.Values.ToList();
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
