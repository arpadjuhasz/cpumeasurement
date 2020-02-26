using CPUMeasurementBackend.HostedService;
using CPUMeasurementCommon.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
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
            List<ClientData> result = this._management.ConnectedClients.Values.ToList();
            return result;
        }
    }
}
