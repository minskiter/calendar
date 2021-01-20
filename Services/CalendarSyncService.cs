using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace calendar.Services
{
    public class CalendarSyncService : IHostedService, IDisposable
    {
        private readonly ILogger<CalendarSyncService> _logger;
        private readonly IFZUCalendarService _service;
        private Timer _timer;

        public CalendarSyncService(ILogger<CalendarSyncService> logger, IFZUCalendarService service)
        {
            _logger = logger;
            _service = service;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("校历同步初始化");
            _timer = new Timer(SyncCalendar, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private void SyncCalendar(object state)
        {
            _logger.LogInformation("同步校历,，同步时间 {0}", DateTime.Now);
            _service.SyncCalendar().ConfigureAwait(false).GetAwaiter().GetResult();
            _logger.LogInformation("同步校历结束，完成时间 {0}", DateTime.Now);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("结束校历同步服务");
            _timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
