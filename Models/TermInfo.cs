using System;
using System.Collections.Generic;

namespace calendar.Models
{
    /// <summary>
    /// 学期信息
    /// </summary>
    public class TermInfo : Term
    {
        /// <summary>
        /// 开始的时间
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// 结束的时间
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// 事件
        /// </summary>
        public List<TermEvent> Events { get; set; }
    }
}
