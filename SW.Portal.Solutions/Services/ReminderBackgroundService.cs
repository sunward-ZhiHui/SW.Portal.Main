using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Repositories.Query;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SW.Portal.Solutions.Services
{
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly IDashboardQueryRepository _dashboardQueryRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public static event Action<string, string>? OnReminderTriggered;

        public ReminderBackgroundService(IDashboardQueryRepository dashboardQueryRepository, IServiceScopeFactory serviceScopeFactory)
        {
            _dashboardQueryRepository = dashboardQueryRepository;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    using (var scope = _serviceScopeFactory.CreateScope())
            //    {
            //        var dashboardQueryRepository = scope.ServiceProvider.GetRequiredService<IDashboardQueryRepository>();
            //        var reminders = await dashboardQueryRepository.GetPendingRemindersAsync();

            //    }

            //    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Check every minute
            //}
        }
    }
}
