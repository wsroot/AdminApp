using System.Collections.Generic;
using AdminApp.Core.Util;

namespace AdminApp.Api.Models
{
    /// <summary>
    /// 类注释
    /// </summary>
    /// <summary>
    /// JSON参数（先取客户端，拿到Code取数据库，取到Map后反填客户端实体）
    /// </summary>
    public class JsonParameter
    {
        /// <summary>
        /// 调用端传入Map别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 调用端传入调用方法
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 目标sqlID
        /// </summary>
        public string SqlId { get; set; }

        /// <summary>
        /// 上下文
        /// </summary>
        public string ScopeName { get; set; }

        /// <summary>
        /// 调用端传入查询条件字典
        /// </summary>
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
    }
}
