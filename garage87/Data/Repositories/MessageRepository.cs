using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;

namespace garage87.Data.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private readonly DataContext _context;

        public MessageRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
