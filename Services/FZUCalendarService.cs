using calendar.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace calendar.Services
{

    /// <inheritdoc/>
    public class FZUCalendarService : IFZUCalendarService
    {
        private readonly IFZUCalendarAPI _calendarAPI;
        private readonly ILogger<FZUCalendarService> _logger;
        private List<TermInfo> _calendar;
        private readonly object _calendarLock = new object();

        private readonly int[] MaxWeek = new int[] { 28, 26 };

        public FZUCalendarService(IFZUCalendarAPI calendarAPI, ILogger<FZUCalendarService> logger)
        {
            _calendarAPI = calendarAPI;
            _logger = logger;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">初始化校历参数错误时抛出</exception>      
        public async Task SyncCalendar()
        {
            _logger.LogInformation("同步校历");
            var calendar = await _calendarAPI.GetTermInfos();
            if (calendar == null || calendar.Count == 0)
            {
                _logger.LogError("校历同步错误");
            }
            else
            {
                lock (_calendarLock)
                {
                    _calendar = calendar;
                }
                _logger.LogInformation("校历同步成功");
            }

        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException">转换后范围错误时抛出</exception>
        /// <exception cref="NullReferenceException">校历未初始化时抛出</exception>
        public TermWeek GetTermWeek(DateTime date = default)
        {
            if (_calendar == null)
            {
                throw new NullReferenceException("校历未初始化");
            }
            if (date == default)
            {
                date = DateTime.Now;
            }
            foreach (var term in _calendar)
            {
                if (date >= term.Start)
                {
                    var teamweek = new TermWeek
                    {
                        Semester = term.Semester,
                        Year = term.Year,
                        Week = ((date - term.Start).Days - 1) / 7 + 1
                    };
                    if (teamweek.Week >= MaxWeek[0])
                    {
                        throw new ArgumentOutOfRangeException(nameof(date));
                    }
                    return teamweek;
                }
            }
            return null;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException">当学期范围错误时抛出</exception>
        /// <exception cref="NullReferenceException">校历未初始化时抛出</exception>
        public DateTime GetTermWeekStart(TermWeek termWeek)
        {
            if (_calendar == null)
            {
                throw new NullReferenceException("校历未初始化");
            }
            if (!IsTermWeek(termWeek))
            {
                throw new ArgumentOutOfRangeException(nameof(termWeek));
            }
            foreach (var term in _calendar)
            {
                if (term.Year.Equals(termWeek.Year) && term.Semester.Equals(termWeek.Semester))
                {
                    // 转换为周一 00:00:00
                    if (term.Start.DayOfWeek != DayOfWeek.Monday)
                    {
                        var count = term.Start.DayOfWeek - DayOfWeek.Monday;
                        if (count == -1) count = 6;
                        term.Start = term.Start.AddDays(count);
                    }
                    return term.Start.AddDays(7 * (termWeek.Week - 1));
                }
            }
            return default;
        }


        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException">当学期范围错误时抛出</exception>
        /// <exception cref="NullReferenceException">校历未初始化时抛出</exception>
        public DateTime GetTermWeekEnd(TermWeek termWeek)
        {
            if (_calendar == null)
            {
                throw new NullReferenceException("校历未初始化");
            }
            var date = GetTermWeekStart(termWeek);
            if (date != default)
            {
                date.AddDays(6).AddHours(23).AddMinutes(59).AddMinutes(59);
            }
            return date;
        }

        /// <inheritdoc/>
        /// <exception cref="NullReferenceException">校历未初始化时抛出</exception>
        /// <exception cref="ArgumentOutOfRangeException">学期范围错误时抛出</exception>
        public int GetTermWeekCount(Term term)
        {
            if (_calendar == null)
            {
                throw new NullReferenceException("校历未初始化");
            }
            if (!IsTerm(term))
            {
                throw new ArgumentOutOfRangeException(nameof(term));
            }
            for (var index = 0; index < _calendar.Count; ++index)
            {
                var terminfo = _calendar[index];
                if (terminfo.Year == term.Year && term.Semester == terminfo.Semester)
                {
                    if (index == 0)
                    {
                        return term.Semester == 2 ? MaxWeek[0] : MaxWeek[1];
                    }
                    else
                    {
                        var timespan = _calendar[index - 1].Start - terminfo.Start;
                        return (timespan.Days - 1) / 7 + 1;
                    }
                }
            }
            return 0;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">范围错误时抛出</exception>
        /// <exception cref="NullReferenceException">校历未初始化时抛出</exception>
        public List<TermEvent> GetTermEvents(Term term)
        {
            if (_calendar == null)
            {
                throw new NullReferenceException("校历未初始化");
            }
            if (!IsTerm(term))
            {
                throw new ArgumentOutOfRangeException(nameof(term));
            }
            var terminfo = _calendar.Find(e => e.Year == term.Year && e.Semester == term.Semester);
            if (terminfo != null)
            {
                return terminfo.Events;
            }
            return null;
        }

        /// <summary>
        /// 判断学期是否正确或者在范围内
        /// </summary>
        /// <param name="term">学期</param>
        /// <returns>格式范围是否正确</returns>
        /// <exception cref="NullReferenceException">校历未初始化时抛出</exception>
        private bool IsTerm(Term term)
        {
            if (_calendar == null)
            {
                throw new NullReferenceException("校历未初始化");
            }
            if (term.Semester < 1 || term.Semester > 2 || term.Year > _calendar[0].Year || term.Year < _calendar[^1].Year)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断学期周数是否在范围内
        /// </summary>
        /// <param name="term">学期周数</param>
        /// <returns>格式范围是否正确</returns>
        /// <exception cref="NullReferenceException">校历未初始化时抛出</exception>
        private bool IsTermWeek(TermWeek term)
        {
            if (_calendar == null)
            {
                throw new NullReferenceException("校历未初始化");
            }
            if (term.Semester < 1 || term.Semester > 2 || term.Year > _calendar[0].Year || term.Year < _calendar[^1].Year)
            {
                return false;
            }
            if (term.Week < 1 || term.Week > GetTermWeekCount(term))
            {
                return false;
            }
            return true;
        }

        public List<TermInfo> GetCalendar()
        {
            return _calendar;
        }
    }
}
