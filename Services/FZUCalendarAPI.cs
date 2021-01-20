using calendar.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace calendar.Services
{
    /// <summary>
    /// 校历接口
    /// </summary>
    public class FZUCalendarAPI : IFZUCalendarAPI
    {
        /// <summary>
        /// 客户端
        /// </summary>
        private readonly HttpClient _client;
        private readonly ILogger<FZUCalendarAPI> _logger;

        public static string ClientName => "FZUCalendar";

        public FZUCalendarAPI(IHttpClientFactory factory, ILogger<FZUCalendarAPI> logger)
        {
            _client = factory.CreateClient(ClientName);
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<List<TermInfo>> GetTermInfos()
        {
            _logger.LogInformation("获取校历信息");
            var res = await _client.PostAsync("/xl.asp", null);
            if (res.IsSuccessStatusCode)
            {
                var result = new List<TermInfo>();
                var bytes = await res.Content.ReadAsByteArrayAsync();
                var html = Encoding.GetEncoding("GBK").GetString(bytes);
                // 分析校历页面
                var option = new Regex(@"<option value=(\d+)>\d+</option>");
                var matches = option.Matches(html);
                foreach (Match match in matches)
                {
                    var value = match.Groups[1].Value;
                    var format = "yyyyMMdd";
                    result.Add(new TermInfo
                    {
                        Year = int.Parse(value.Substring(0, 4)),
                        Semester = int.Parse(value.Substring(4, 2)),
                        Start = DateTime.ParseExact(value.Substring(6, 8), format, null),
                        End = DateTime.ParseExact(value.Substring(14, 8), format, null).AddHours(23).AddMinutes(59).AddSeconds(59),
                        Events = await GetTermEventsAsync(value)
                    });
                }
                // 对result进行排序，按最新的时间到过去来排
                result.Sort((a, b) => a.Start > b.Start ? -1 : 1);
                return result;
            }
            else
            {
                _logger.LogWarning("获取校历失败");
            }
            return null;
        }

        /// <summary>
        /// 获取某个学期的具体事件
        /// </summary>
        /// <param name="xq">具体的学期格式为 学期开始时间结束时间</param>
        /// <exception cref="ArgumentException">xq格式错误时抛出</exception>
        /// <returns></returns>
        private async Task<List<TermEvent>> GetTermEventsAsync(string xq = null)
        {
            if (!Regex.IsMatch(xq, @"\d{22}"))
            {
                throw new ArgumentException("xq必须为数字,且长度必须为22位");
            }
            _logger.LogInformation("获取 {0} 事件", xq.Substring(0, 6));
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("xq",xq)
            });
            var res = await _client.PostAsync("/xl.asp", formData);
            if (res.IsSuccessStatusCode)
            {
                var bytes = await res.Content.ReadAsByteArrayAsync();
                var html = Encoding.GetEncoding("GBK").GetString(bytes);
                var events = new List<TermEvent>();
                var eventPattern = new Regex(@"([\d-]{10})至([\d-]{10})为([\u4e00-\u9fa5]+)；");
                var matches = eventPattern.Matches(html);
                foreach (Match match in matches)
                {
                    events.Add(new TermEvent
                    {
                        Start = DateTime.Parse(match.Groups[1].Value),
                        End = DateTime.Parse(match.Groups[2].Value).AddHours(23).AddMinutes(59).AddSeconds(59),
                        Name = match.Groups[3].Value
                    });
                }
                return events;
            }
            else
            {
                _logger.LogWarning("获取 {0} 事件失败", xq.Substring(0, 6));
            }
            return null;
        }
    }
}
