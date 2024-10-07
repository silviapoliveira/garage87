using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.DependencyInjection;
using garage87.Data.Repositories.IRepository;


namespace garage87.Services
{
    public class NotificationService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var notificationDb = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                    notificationDb.AddNotification();
                }
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }
}