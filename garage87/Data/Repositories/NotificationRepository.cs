using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;
using garage87.Helpers;
using garage87.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace garage87.Data.Repositories
{
    public class NotificationRepository : GenericRepository<Notifications>, INotificationRepository
    {
        private readonly DataContext _context;

        public NotificationRepository(DataContext context) : base(context)
        {
            _context = context;

        }
        public void AddNotification()
        {
            var assigns = _context.VehicleAssignment
                                  .Where(x => x.TaskDate.Date == DateTime.Today)
                                  .Include(x => x.Employee)
                                  .Include(x => x.Vehicle)
                                  .ThenInclude(x => x.Customer)
                                  .ToList();


            if (assigns.Any())
            {
                foreach (var item in assigns)
                {
                    // Check if a notification with the same assignment ID already exists
                    var existingNotification = _context.Notifications
                        .Any(n => n.AssignId == item.Id);

                    if (!existingNotification) //
                    {
                        var notification = new Notifications
                        {
                            UserId = item.Employee.UserId,
                            IsRead = false,
                            Message = $"Hello {item.Employee.FullName}, you have an assignment scheduled for today.",
                            MessageDate = DateTime.Today,
                            AssignId = item.Id
                        };

                        _context.Notifications.Add(notification);
                    }
                }
                // Save all changes to the database
                _context.SaveChanges();
            }
        }
    }
}

