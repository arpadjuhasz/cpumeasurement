using CPUMeasurementBackend.WebService;
using CPUMeasurementCommon.DataObjects;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Controllers
{
    [Route("management")]
    public class ManagementController : Controller
    {
        public readonly ManagementService _managementService;

        public ManagementController(ManagementService managementService)
        {
            this._managementService = managementService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetConnectedClients()
        {
            return Ok(await this._managementService.GetConnectedClients());
        }
    }
}
