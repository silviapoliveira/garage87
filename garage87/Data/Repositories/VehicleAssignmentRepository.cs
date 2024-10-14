using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;

namespace garage87.Data.Repositories
{
    public class VehicleAssignmentRepository : GenericRepository<VehicleAssignment>, IVehicleAssignmentRepository
    {
        private readonly DataContext _context;

        public VehicleAssignmentRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
