/***************************
 * 命名空间：AdminApp.Core.Entity
 * 类名称：ReadMe
 * 文件名：ReadMe
 * 创建时间：2017-8-17 10:02:44
 * 创建人：liucaixi
 * 创建说明：
 ***************************
 * 修改人：
 * 修改时间：
 * 修改说明：
****************************/

using System.Collections.Generic;

namespace AdminApp.Core.Util
{
    /// <summary>
    ///     分页类
    /// </summary>
    public class PageInfo<T>
    {
        public string total { get; set; }
        public IEnumerable<T> rows { get; set; }
        public string page { get; set; }
        public string records { get; set; }
    }
}