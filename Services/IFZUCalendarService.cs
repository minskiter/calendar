using calendar.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace calendar.Services
{
    /// <summary>
    /// FZU日历分析服务
    /// </summary>
    public interface IFZUCalendarService
    {
        /// <summary>
        /// 获取校历信息
        /// </summary>
        /// <returns>校历信息列表</returns>
        List<TermInfo> GetCalendar();

        /// <summary>
        /// 同步校历
        /// </summary>
        /// <returns></returns>
        Task SyncCalendar();

        /// <summary>
        /// 获取指定时间的学期，默认为当前时间
        /// </summary>
        /// <param name="date">指定日期</param>
        /// <returns>对应的学期</returns>
        TermWeek GetTermWeek(DateTime date = default);

        /// <summary>
        /// 获取指定周开始的时间
        /// </summary>
        /// <param name="termWeek">指定学期和周</param>
        /// <returns>该周周一00:00:00</returns>
        DateTime GetTermWeekStart(TermWeek termWeek);

        /// <summary>
        /// 获取指定周结束的时间
        /// </summary>
        /// <param name="termWeek">指定学期和周</param>
        /// <returns>该周周日23:59:59</returns>
        DateTime GetTermWeekEnd(TermWeek termWeek);

        /// <summary>
        /// 获取指定学期的周数
        /// </summary>
        /// <param name="term">学期</param>
        /// <returns>周的总数</returns>
        int GetTermWeekCount(Term term);

        /// <summary>
        /// 获取当前学期的事件
        /// </summary>
        /// <param name="term"></param>
        /// <returns>返回事件的列表</returns>
        public List<TermEvent> GetTermEvents(Term term);
    }
}
