using CPUMeasurementBackend.WebService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Controllers
{
    [Route("cpudata")]
    public class CPUDataController : ControllerBase
    {
        private readonly CPUDataService _cpuDataService;

        public CPUDataController(CPUDataService cpuDataService)
        {
            this._cpuDataService = cpuDataService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetCPUData([FromQuery]DateTime? date)
        {
            return  Ok(await this._cpuDataService.GetList(date));
        }
    }
}
