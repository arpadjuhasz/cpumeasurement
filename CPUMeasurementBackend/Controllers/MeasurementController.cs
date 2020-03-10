using CPUMeasurementBackend.WebService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Controllers
{
    
    [Route("measurement")]
    [Authorize]
    public class MeasurementController : ControllerBase
    {
        private readonly MeasurementService _cpuDataService;

        public MeasurementController(MeasurementService cpuDataService)
        {
            this._cpuDataService = cpuDataService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetCPUData([FromQuery]DateTime? date, [FromQuery]string ipAddress = null)
        {
            return  Ok(await this._cpuDataService.GetList(date, ipAddress));
        }
    }
}
