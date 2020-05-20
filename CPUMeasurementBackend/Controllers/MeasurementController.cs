using CPUMeasurementBackend.WebServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Controllers
{
    
    [Route("measurement")]
    [ApiController]
    [Authorize]
    public class MeasurementController : ControllerBase
    {
        private readonly MeasurementService _measurementService;

        public MeasurementController(MeasurementService cpuDataService)
        {
            this._measurementService = cpuDataService;
        }

        [HttpGet()]
        public IActionResult GetMeasurements([FromQuery]DateTime? date, [FromQuery]string ipAddress = null)
        {
            return  Ok(this._measurementService.GetMeasurements(date, ipAddress));
        }
    }
}
