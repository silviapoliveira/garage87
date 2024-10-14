using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;

namespace garage87.Data.Repositories
{
    public class ModelRepository : GenericRepository<Model>, IModelRepository
    {
        private readonly DataContext _context;

        public ModelRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
