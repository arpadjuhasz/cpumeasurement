using CPUMeasurementCommon.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementFrontend.Data
{
    public class ManagementService
    {
        private readonly CPUDataHttpClient _httpClient;
        private const string APIPATH = "/management";

        public ManagementService(CPUDataHttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<List<ClientData>> GetConnectedClients()
        {
            return await this._httpClient.GetJsonAsyncc<List<ClientData>>(APIPATH);
        }
    }
}
