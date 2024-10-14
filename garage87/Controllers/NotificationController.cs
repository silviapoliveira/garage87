using garage87.Data.Repositories.IRepository;
using garage87.Helpers;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace garage87.Controllers
{
    public class NotificationController : Controller
    {
        public readonly INotificationRepository _notificationRepo;
        private readonly IUserHelper _userHelper;
        public NotificationController(INotificationRepository notificationRepo, IUserHelper userHelper)
        {
            _notificationRepo = notificationRepo;
            _userHelper = userHelper;

        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var notifications = _notificationRepo.GetAll().Where(x => x.UserId == user.Id && x.IsRead == false);
            return Json(notifications); // Return as JSON
        }
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var obj = await _notificationRepo.GetByIdAsync(id);
            if (obj != null)
            {
                obj.IsRead = true;
                await _notificationRepo.UpdateAsync(obj);
                return Ok();

            }

            return BadRequest();
        }
    }
}
