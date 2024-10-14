using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;

namespace garage87.Data.Repositories
{
    public class RepairDetailRepository : GenericRepository<RepairDetail>, IRepairDetailRepository
    {
        private readonly DataContext _context;

        public RepairDetailRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
