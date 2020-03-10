using CPUMeasurementBackend.Repository;
using CPUMeasurementCommon.DataObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.WebService
{
    public class MeasurementService
    {
        private readonly CPUDataRepository _cpuDataRepository;

        public MeasurementService(CPUDataRepository cpuDataRepository)
        {
            this._cpuDataRepository = cpuDataRepository;
        }

        public async  Task<List<CPUData>> GetList(DateTime? date, string ipAddress)
        {
            return (await this._cpuDataRepository.GetCPUData(date, ipAddress));
            
        }
    }
}
