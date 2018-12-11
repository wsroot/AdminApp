using AdminApp.Api.Models;
using AdminApp.Base.Entities;
using AdminApp.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AdminApp.Core.Util;

namespace AdminApp.Api.Controllers
{
    /// <summary>
    /// 数据代理接口
    /// </summary>
    [Authorize]
    [Route("api/dispatcher")]
    public class DispatcherController : Controller
    {
        /// <summary>
        /// 接收代理请求
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [HttpPost]
        public ObjectResult Post(JsonParameter jsonData)
        {
            if (jsonData!=null)
            {
                if (string.IsNullOrEmpty(jsonData.Alias)||string.IsNullOrEmpty(jsonData.MethodName))
                    return StatusCode(StatusCodes.Status417ExpectationFailed,new { result = "未传递服务名称或方法名",success = false });

                InvokeParameter invokeData;
                try
                {
                    //读取Map取到.NET字段 实例化为JsonParameter 将前端传入参数设置到postData中
                    var mapService = new ServiceContext<ServiceMap>();
                    var serviceMap = mapService.GetList(new { jsonData.Alias,InUse = 1 }).FirstOrDefault();
                    if (serviceMap==null)
                        return StatusCode(StatusCodes.Status417ExpectationFailed,new { result = $"服务{jsonData.Alias}未注册",success = false });
                    invokeData=JsonConvert.DeserializeObject<InvokeParameter>(serviceMap.DotNetPath);

                    invokeData.JsonParameter=jsonData;

                    var currentUserId = User.Claims.ToList().FirstOrDefault(x => x.Type=="uid")?.Value;

                    invokeData.JsonParameter.Parameters.Add(new Parameter{ Key =PostDataParamKeys.LoginUserId,Value = currentUserId });
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,new { result = ex.Message,success = false });
                }
                if (!string.IsNullOrEmpty(invokeData.EntityType)&&!string.IsNullOrEmpty(invokeData.EntityAssembly))
                {
#if DEBUG
                    var assemblies =
                        new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory+"/DLL/").GetFiles()
                            .Select(r => Assembly.LoadFrom(r.FullName))
                            .ToArray();
                    var assemblyentity = assemblies.FirstOrDefault(m => m.GetName().Name==invokeData.EntityAssembly);//目标实体所在程序集
#else
                    var assemblyentity = Assembly.Load(postData.EntityAssembly);//目标实体所在程序集
#endif
                    if (assemblyentity==null)
                        return StatusCode(StatusCodes.Status417ExpectationFailed,new { result = $"未找到实体{invokeData.EntityAssembly}程序集",success = false });
                    var entityType = assemblyentity.GetType(invokeData.EntityType);//目标实体完全限定名
                    if (entityType==null)
                        return StatusCode(StatusCodes.Status417ExpectationFailed,new { result = $"实体{invokeData.EntityType}加载失败",success = false });
                    if (!string.IsNullOrEmpty(invokeData.ClassAssembly)&&!string.IsNullOrEmpty(invokeData.ClassName))
                    {
                        var assemblyBllName = invokeData.ClassAssembly;
                        var classTypeName = invokeData.ClassName;
#if DEBUG
                        var assemblyBll = assemblies.FirstOrDefault(m => m.GetName().Name==assemblyBllName);//目标实体所在程序集
#else
                        var assemblyBll = Assembly.Load(assemblyBllName);//服务类所在程序集
#endif
                        if (assemblyBll==null)
                            return StatusCode(StatusCodes.Status417ExpectationFailed,new { result = $"未找到服务{assemblyBllName}程序集",success = false });

                        var classType = assemblyBll.GetType(classTypeName);//服务类
                        if (classType==null)
                        {
                            classType=assemblyBll.GetType(classTypeName+"`1"); //检测是否泛型类
                            if (classType==null)
                                return StatusCode(StatusCodes.Status417ExpectationFailed,new { result = $"服务{classTypeName}加载失败",success = false });
                        }
                        object instance;
                        if (classType.IsGenericTypeDefinition) //serviceBase泛型类
                        {
                            classType=classType.MakeGenericType(entityType);
                            var scopeName = !string.IsNullOrEmpty(invokeData.JsonParameter.ScopeName)
                                ? invokeData.JsonParameter.ScopeName
                                : invokeData.EntityType.Split('.').Last();
                            instance=assemblyBll.CreateInstance(classType.FullName,true,BindingFlags.Default,
                                null,new object [ ] { scopeName },null,null);
                        }
                        else
                        {
                            instance=assemblyBll.CreateInstance(classType.FullName);
                        }


                        if (instance==null)
                            return StatusCode(StatusCodes.Status417ExpectationFailed,new { result = $"服务{classTypeName}实例化失败",success = false });
                        var mi = classType.GetMethods().FirstOrDefault(m => m.Name==invokeData.JsonParameter.MethodName&&m.GetParameters().FirstOrDefault(x => x.ParameterType==(typeof(Dictionary<string,string>)))!=null);
                        if (string.IsNullOrEmpty(invokeData.JsonParameter.MethodName)||mi==null)
                            return StatusCode(StatusCodes.Status417ExpectationFailed,new { result = $"未在服务类{classTypeName}中找到{invokeData.JsonParameter.MethodName}方法",success = false });
                        try
                        {
                            var paramObjs = new List<object> { invokeData.JsonParameter.Parameters };
                            if (!string.IsNullOrEmpty(invokeData.JsonParameter.SqlId))
                                paramObjs.Add(invokeData.JsonParameter.SqlId);

                            var result = mi.IsGenericMethodDefinition ?
                            mi.MakeGenericMethod(entityType).Invoke(instance,mi.GetParameters().Any() ? paramObjs.ToArray() : null) ://泛型方法调用
                            mi.Invoke(instance,mi.GetParameters().Any() ? paramObjs.ToArray() : null);
                            return StatusCode(StatusCodes.Status200OK,new { result,success = true });
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError,new { result = $"调用发生错误:{ex.Message}，请查看服务目录日志,提供数据:{JsonConvert.SerializeObject(invokeData)}",success = false });
                        }
                    }
                    return StatusCode(StatusCodes.Status416RangeNotSatisfiable,new { result = "服务配置错误",success = false });
                }
                return StatusCode(StatusCodes.Status416RangeNotSatisfiable,new { result = "未包含调用服务信息",success = false });
            }
            return StatusCode(StatusCodes.Status204NoContent,new { result = "未提交任何数据",success = false });
        }
    }
}
