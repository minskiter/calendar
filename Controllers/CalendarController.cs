using calendar.Models;
using calendar.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace calendar.Controllers
{
    /// <summary>
    /// 校历接口
    /// </summary>
    [ApiController]
    [Route("/[controller]")]
    public class CalendarController : ControllerBase
    {
        private IFZUCalendarService _service;

        public CalendarController(IFZUCalendarService service)
        {
            _service = service;
        }

        /// <summary>
        ///  查询指定日期对应的学期周数，默认为当前日期
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns>学期周数</returns>
        [HttpGet("term")]
        [ProducesResponseType(typeof(TermWeek), StatusCodes.Status200OK)]
        public IActionResult GetTermWeek([FromQuery] DateTime date = default)
        {
            return Ok(_service.GetTermWeek(date));
        }

        /// <summary>
        /// 获取周起始
        /// </summary>
        /// <param name="term">学期周</param>
        /// <returns>周起始的时间</returns>
        [HttpGet("term/weekstart")]
        [ProducesResponseType(typeof(DateTime), StatusCodes.Status200OK)]
        public IActionResult GetTermWeekStart([FromQuery] TermWeek term)
        {
            return Ok(_service.GetTermWeekStart(term));
        }

        /// <summary>
        /// 获取周结束时间
        /// </summary>
        /// <param name="term">学期周</param>
        /// <returns>周结束的时间</returns>
        [HttpGet("term/weekend")]
        [ProducesResponseType(typeof(DateTime), StatusCodes.Status200OK)]
        public IActionResult GetTermWeekEnd([FromQuery] TermWeek term)
        {
            return Ok(_service.GetTermWeekEnd(term));
        }

        /// <summary>
        /// 获取周数
        /// </summary>
        /// <param name="term">学期</param>
        /// <returns>学期对应的周数</returns>
        [HttpGet("term/weekcount")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public IActionResult GetTermWeekCount([FromQuery] Term term)
        {
            return Ok(_service.GetTermWeekCount(term));
        }

        /// <summary>
        /// 获取学期事件
        /// </summary>
        /// <param name="term">学期</param>
        /// <returns>学期事件</returns>
        [HttpGet("term/weekevents")]
        [ProducesResponseType(typeof(List<TermEvent>), StatusCodes.Status200OK)]
        public IActionResult GetTermEvents([FromQuery] Term term)
        {
            return Ok(_service.GetTermEvents(term));
        }

    }
}
