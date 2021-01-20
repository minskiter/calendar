using calendar.HealthCheck;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;

namespace calendar.Services
{
    public static class FZUCalendarExtension
    {
        public static IServiceCollection AddFZUCalendar(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            // 注册监控服务
            services.AddHealthChecks().AddCalendarCheck("CalendarCheck");
            // 注册校历HttpClient
            services.AddHttpClient("FZUCalendar", c =>
            {
                c.BaseAddress = new Uri("http://59.77.226.32/");
                c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36 Edg/87.0.664.75");
            });
            // 注册同步服务
            services.AddHostedService<CalendarSyncService>();
            // 注册GBK编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton<IFZUCalendarService, FZUCalendarService>();
                    services.AddSingleton<IFZUCalendarAPI, FZUCalendarAPI>();
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped<IFZUCalendarService, FZUCalendarService>();
                    services.AddScoped<IFZUCalendarAPI, FZUCalendarAPI>();
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient<IFZUCalendarService, FZUCalendarService>();
                    services.AddTransient<IFZUCalendarAPI, FZUCalendarAPI>();
                    break;
            }
            return services;
        }
    }
}
