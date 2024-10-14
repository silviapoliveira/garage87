using garage87.Data.Entities;

namespace garage87.Data.Repositories.IRepository
{
    public interface INotificationRepository : IGenericRepository<Notifications>
    {
        public void AddNotification();
    }
}
