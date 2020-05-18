using CPUMeasurementBackend.Repositories;
using CPUMeasurementCommon.DataObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.WebService
{
    public class MeasurementService
    {
        private readonly CPUMeasurementRepository _measurementRepository;

        public MeasurementService(CPUMeasurementRepository cpuDataRepository)
        {
            this._measurementRepository = cpuDataRepository;
        }

        public List<CPUMeasurement> GetMeasurements(DateTime? date, string ipAddress)
        {
            return this._measurementRepository.GetMeasurementData(date, ipAddress);
        }
    }
}
