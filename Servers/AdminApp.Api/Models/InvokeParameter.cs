using System.Collections.Generic;

namespace AdminApp.Api.Models
{
    /// <summary>
    /// 类注释
    /// </summary>
    /// <summary>
    /// JSON参数（先取客户端，拿到Code取数据库，取到Map后反填客户端实体）
    /// </summary>
    public class InvokeParameter
    {
        /// <summary>
        /// 调用端传入Map别名
        /// </summary>
        public JsonParameter JsonParameter { get; set; }
        /// <summary>
        /// 调用类所在程序集(数据库中根据Code取出)
        /// </summary>
        public string ClassAssembly { get; set; }
        /// <summary>
        /// 调用类名(完全限定名)(数据库中根据Code取出)
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 查询实体所在程序集(数据库中根据Code取出)
        /// </summary>
        public string EntityAssembly { get; set; }
        /// <summary>
        /// 查询实体类型(完全限定名)(数据库中根据Code取出)
        /// </summary>
        public string EntityType { get; set; }
    }
}
