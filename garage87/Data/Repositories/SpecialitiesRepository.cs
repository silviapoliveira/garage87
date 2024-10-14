using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;

namespace garage87.Data.Repositories
{
    public class SpecialitiesRepository : GenericRepository<Specialities>, ISpecialitiesRepository
    {
        private readonly DataContext _context;

        public SpecialitiesRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
