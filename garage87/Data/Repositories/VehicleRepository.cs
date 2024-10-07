using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;
using garage87.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace garage87.Data.Repositories
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        private readonly DataContext _context;

        public VehicleRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
