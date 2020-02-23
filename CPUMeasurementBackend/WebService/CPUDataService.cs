using CPUMeasurementBackend.Repository;
using CPUMeasurementBackend.Repository.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.WebService
{
    public class CPUDataService
    {
        private readonly CPUDataRepository _cpuDataRepository;

        public CPUDataService(CPUDataRepository cpuDataRepository)
        {
            this._cpuDataRepository = cpuDataRepository;
        }

        public async  Task<List<CPUData>> GetList()
        {
            return (await this._cpuDataRepository.GetCPUData());
        }
    }
}
