using calendar.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace calendar.HealthCheck
{
    /// <summary>
    /// 校历服务可用性监控
    /// </summary>
    public class CalendarHealthCheck : IHealthCheck
    {
        private readonly IFZUCalendarService _service;

        public string Name => "calendar_check";

        public CalendarHealthCheck(IFZUCalendarService service)
        {
            _service = service;
        }

        /// <inheritdoc/>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var status = _service.GetCalendar() == null ? HealthStatus.Degraded : HealthStatus.Healthy;
            return Task.FromResult(new HealthCheckResult(status, "校历信息"));
        }
    }

    public static class DeviceHealthCheckRouteBuilderExtension
    {
        public static IEndpointConventionBuilder MapCalendarHealth(this IEndpointRouteBuilder endpoints, string pattern)
        {
            return endpoints.MapHealthChecks(pattern);
        }
    }

    public static class DeviceHealthCheckBuilderExtension
    {
        public static IHealthChecksBuilder AddCalendarCheck(this IHealthChecksBuilder builder, string name, IEnumerable<string> tags = null)
        {
            builder.AddCheck<CalendarHealthCheck>(name, HealthStatus.Unhealthy, tags);
            return builder;
        }
    }
}
