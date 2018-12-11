using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json;
using SmartSql;

namespace AdminApp.Core.Util
{
    public static class ObjectExtension
    {
        public static object GetValue(this object obj,string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;
            if (obj.GetType()!=typeof(Dictionary<string,string>))
                return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
            return ((Dictionary<string,string>) obj).TryGetValue(propertyName,out var val) ? val : null; //兼容键值对字典传参
        }

        public static DbParameterCollection ToDbParameters(this List<Parameter> param)
        {
            var tempparam = new DbParameterCollection();
            param.ToList().ForEach(m =>
            {
                switch (m.Key)
                {
                    case "FILTER":
                        var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string,string>>(param.FirstOrDefault(x=>x.Key=="FILTER")?.Value);
                        filterDictionary.ToList().ForEach(f =>
                        {
                            tempparam.Add(f.Key,f.Value);
                        });
                        break;
                    case "PAGEMUN":
                    case "PAGESIZE":
                        tempparam.Add(new DbParameter
                        {
                            Name=m.Key,
                            DbType=DbType.Int32,
                            Value = m.Value
                        });
                        break;
                    case "AUTHORGANISEUNITID":
                        tempparam.Add(m.Key,m.Value.Split(','));
                        break;
                    default:
                        tempparam.Add(m.Key,m.Value);
                        break;
                }
            });
            return tempparam;
        }
    }
}