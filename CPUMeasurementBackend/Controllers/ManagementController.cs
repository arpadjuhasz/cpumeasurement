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

        [HttpPut("client/{ip}")]
        public IActionResult UpdateMeasurementIntervalInSeconds(string ip,[FromBody]MeasurementIntervalUpdate dto)
        {
            this._managementService.UpdateMeasurementInterval(ip, dto);
            return NoContent();
        }

        [HttpGet("client/{second}")]
        public IActionResult UpdateMeasurementIntervalInSeconds(int second)
        {
            this._managementService.UpdateMeasurementInterval("192.168.0.80", new MeasurementIntervalUpdate {  MeasurementIntervalInSeconds = second });
            return NoContent();
        }
    }
}
