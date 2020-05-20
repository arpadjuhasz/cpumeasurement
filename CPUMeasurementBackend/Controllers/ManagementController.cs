using CPUMeasurementBackend.WebServices;
using CPUMeasurementBackend.WebServices.Managements;
using CPUMeasurementCommon.DataObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Controllers
{
    [Route("management")]
    [ApiController]
    [Authorize]
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

        [HttpPut("client/{ip}")]
        public async Task<IActionResult> UpdateMeasurementIntervalInSeconds(string ip,[FromBody]MeasurementIntervalUpdate dto)
        {
            this._managementService.UpdateMeasurementInterval(ip, new MeasurementIntervalUpdate { MeasurementIntervalInSeconds = dto.MeasurementIntervalInSeconds });
            return NoContent(); 
        }
    }
}
