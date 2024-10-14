using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;

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
