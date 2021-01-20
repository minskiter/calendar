using System;

namespace calendar.Models
{
    /// <summary>
    /// 学期事件
    /// </summary>
    public class TermEvent
    {
        /// <summary>
        /// 事件开始的时间
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// 事件结束的时间
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// 事件的名称
        /// </summary>
        public string Name { get; set; }
    }
}
