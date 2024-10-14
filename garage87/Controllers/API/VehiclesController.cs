using garage87.Data.Repositories.IRepository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace garage87.Controllers.API
{

    [ApiController]
    public class VehiclesController : ControllerBase
    {

        private readonly IRepairRepository _repairService;

        public VehiclesController(IRepairRepository repairService)
        {
            _repairService = repairService;
        }

        [HttpGet]
        [Route("api/VehicleInterventions/{id}")]
        public async Task<IActionResult> VehicleInterventions(int id)
        {
            try
            {
                var data = _repairService.GetAll().Where(x => x.VehicleId == id);
                var repair = data.Include(x => x.VehicleAssignment).Include(x => x.Vehicle).Include(x => x.Employee).Include(x => x.RepairDetail).ThenInclude(x => x.Service).ToList();
                return Ok(repair);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, data = ex.Message.ToString() });
            }
        }


    }
}
