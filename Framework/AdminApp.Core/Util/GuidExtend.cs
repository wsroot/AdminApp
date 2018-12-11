/************
 * 命名空间：ZhongYan.Util
 * 类名称：GuidExtends
 * 文件名：GuidExtends
 * 创建时间：2017/9/18 11:57:25
 * 创建人：liucaixi
 * 创建说明：
 ****************************
 * 修改人：
 * 修改时间：
 * 修改说明：
*****************************/

using System;

namespace AdminApp.Core.Util
{
    /// <summary>
    ///     类注释
    /// </summary>
    public static class IdHelper
    {
        public static string CreateId()
        {
            return Guid.NewGuid().ToString("N").ToUpper();
        }
    }
}