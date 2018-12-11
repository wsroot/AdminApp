/************
 * 命名空间：ZhongYan.Util
 * 类名称：PostDataParamKeys
 * 文件名：PostDataParamKeys
 * 创建时间：2017/8/20 16:32:45
 * 创建人：liucaixi
 * 创建说明：
 ****************************
 * 修改人：
 * 修改时间：
 * 修改说明：
*****************************/

namespace AdminApp.Core.Util
{
    /// <summary>
    ///     全局参数key
    /// </summary>
    public struct PostDataParamKeys
    {
        public const string Entity = "ENTITY";
        public const string Filter = "FILTER";
        public const string Id = "ID";
        public const string PageNum = "PAGENUM";
        public const string PageSize = "PAGESIZE";

        public const string LoginUserId = "USERID";
        public const string ApplicationId = "APPLICATIONID";
        public const string ExpTime = "EXPTIME";

        public const string OrganiseunitId = "AUTHORGANISEUNITID";
    }
}