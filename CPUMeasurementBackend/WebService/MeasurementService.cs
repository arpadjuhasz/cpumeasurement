using CPUMeasurementBackend.Repository;
using CPUMeasurementCommon.DataObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.WebService
{
    public class MeasurementService
    {
        private readonly MeasurementRepository _measurementRepository;

        public MeasurementService(MeasurementRepository cpuDataRepository)
        {
            this._measurementRepository = cpuDataRepository;
        }

        public List<MeasurementData> GetMeasurements(DateTime? date, string ipAddress)
        {
            return this._measurementRepository.GetMeasurementData(date, ipAddress);
        }
    }
}
