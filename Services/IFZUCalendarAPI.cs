using calendar.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace calendar.Services
{
    /// <summary>
    ///  福州大学校历接口
    /// </summary>
    public interface IFZUCalendarAPI
    {
        /// <summary>
        /// 获取校历完整信息
        /// </summary>
        /// <returns></returns>
        public Task<List<TermInfo>> GetTermInfos();
    }
}
