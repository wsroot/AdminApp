using AdminApp.Core.Dao;
using AdminApp.Core.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminApp.Core.Service
{
    public sealed class ServiceBase<TFEntity>
    {
        private readonly DataAccess<TFEntity> _dao;
        public ServiceBase(string scopeName = null)
        {
            _dao=new DataAccess<TFEntity>(scopeName);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public string Save(List<Parameter> parameters,string sqlId = DefaultSqlId.IsExist+","+DefaultSqlId.Update+","+DefaultSqlId.Insert)
        {
            var entityString = parameters.FirstOrDefault(x => x.Key==PostDataParamKeys.Entity)?.Value;
            var currentUserId = parameters.FirstOrDefault(x => x.Key==PostDataParamKeys.LoginUserId)?.Value;
            if (!string.IsNullOrEmpty(entityString))
                throw new Exception("保存时未传递实体信息");
            var entity = (TFEntity) JsonConvert.DeserializeObject(entityString,typeof(TFEntity));

            var idPro = entity.GetType().GetProperty("Id");
            if (idPro!=null&&entity.GetValue("Id")==null)
                idPro.SetValue(entity,IdHelper.CreateId());

            var sqlIds = new string [3];
            if (string.IsNullOrEmpty(sqlId)||sqlId.Split(',').Length!=3)
            {
                sqlIds [0]=DefaultSqlId.IsExist;
                sqlIds [1]=DefaultSqlId.Update;
                sqlIds [2]=DefaultSqlId.Insert;
            }
            else
            {
                sqlIds=sqlId.Split(',');
            }

            if (_dao.IsExist(entity,sqlIds [0]))
            {
                var modifieddatePro = entity.GetType().GetProperty("ModifiedTime");
                if (modifieddatePro!=null&&entity.GetValue("ModifiedTime")==null)
                    modifieddatePro.SetValue(entity,DateTime.Now);


                var modifiedbyPro = entity.GetType().GetProperty("ModifiedBy");
                if (modifiedbyPro!=null&&entity.GetValue("ModifiedBy")==null)
                    modifiedbyPro.SetValue(entity,currentUserId);
                if (idPro!=null&&_dao.Update(entity,sqlIds [1])>0)
                    return idPro.GetValue(entity).ToString();
                return Guid.Empty.ToString("N");

            }

            var createdatePro = entity.GetType().GetProperty("CreatedTime");
            if (createdatePro!=null&&entity.GetValue("CreatedTime")==null)
                createdatePro.SetValue(entity,DateTime.Now);


            var createdPro = entity.GetType().GetProperty("CreatedBy");
            if (createdPro!=null&&entity.GetValue("CreatedBy")==null)
                createdPro.SetValue(entity,currentUserId);
            _dao.Insert(entity,sqlIds [2]);
            return idPro!=null ? idPro.GetValue(entity).ToString() : Guid.Empty.ToString("N");
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public string Insert(List<Parameter> parameters,string sqlId = DefaultSqlId.Insert)
        {
            var entityString = parameters.FirstOrDefault(x => x.Key==PostDataParamKeys.Entity)?.Value;
            var currentUserId = parameters.FirstOrDefault(x => x.Key==PostDataParamKeys.LoginUserId)?.Value;
            if (!string.IsNullOrEmpty(entityString))
                throw new Exception("保存时未传递实体信息");
            var entity = (TFEntity) JsonConvert.DeserializeObject(entityString,typeof(TFEntity));

            var idPro = entity.GetType().GetProperty("Id");
            if (idPro!=null&&entity.GetValue("Id")==null)
                idPro.SetValue(entity,IdHelper.CreateId());

            var createdatePro = entity.GetType().GetProperty("CreatedTime");
            if (createdatePro!=null&&entity.GetValue("CreatedTime")==null)
                createdatePro.SetValue(entity,DateTime.Now);


            var createdPro = entity.GetType().GetProperty("CreatedBy");
            if (createdPro!=null&&entity.GetValue("CreatedBy")==null)
                createdPro.SetValue(entity,currentUserId);
            _dao.Insert(entity,sqlId);
            return idPro!=null ? idPro.GetValue(entity).ToString() : Guid.Empty.ToString("N");
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public bool Update(List<Parameter> parameters,string sqlId = DefaultSqlId.Update)
        {
            var entityString = parameters.FirstOrDefault(x => x.Key==PostDataParamKeys.Entity)?.Value;
            var currentUserId = parameters.FirstOrDefault(x => x.Key==PostDataParamKeys.LoginUserId)?.Value;
            if (!string.IsNullOrEmpty(entityString))
                throw new Exception("保存时未传递实体信息");
            var entity = (TFEntity) JsonConvert.DeserializeObject(entityString,typeof(TFEntity));

            var createdatePro = entity.GetType().GetProperty("ModifiedTime");
            if (createdatePro!=null&&entity.GetValue("ModifiedTime")==null)
                createdatePro.SetValue(entity,DateTime.Now);


            var createdPro = entity.GetType().GetProperty("ModifiedBy");
            if (createdPro!=null&&entity.GetValue("ModifiedBy")==null)
                createdPro.SetValue(entity,currentUserId);

            return _dao.Update(entity,sqlId)>0;
        }

        /// <summary>
        /// 数据是否已存在
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public bool IsExist(List<Parameter> parameters,string sqlId = DefaultSqlId.IsExist)
        {
            var entityString = parameters.FirstOrDefault(x => x.Key==PostDataParamKeys.Entity)?.Value;
            if (!string.IsNullOrEmpty(entityString))
                throw new Exception("保存时未传递实体信息");
            var entity = (TFEntity) JsonConvert.DeserializeObject(entityString,typeof(TFEntity));
            return _dao.IsExist(entity,sqlId);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public bool Delete(List<Parameter> parameters,string sqlId = DefaultSqlId.Delete)
        {
            return _dao.Delete(parameters.ToDbParameters(),sqlId)>0;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public PageInfo<TFEntity> GetListByPage(List<Parameter> parameters,
            string sqlId = DefaultSqlId.GetListByPage+","+DefaultSqlId.GetRecord)
        {
            var sqlIds = new string [2];
            if (string.IsNullOrEmpty(sqlId)||sqlId.Split(',').Length!=2)
            {
                sqlIds [0]=DefaultSqlId.GetListByPage;
                sqlIds [1]=DefaultSqlId.GetRecord;
            }
            else
            {
                sqlIds=sqlId.Split(',');
            }
            var pageData = _dao.GetListByPage(parameters.ToDbParameters(),sqlIds [0]);
            var records = _dao.GetRecord(parameters.ToDbParameters(),sqlIds [1]);
            return new PageInfo<TFEntity>
            {
                page=parameters.GetValue("PAGENUM").ToString(),
                records=records.ToString(),
                rows=pageData,
                total=(records/Convert.ToInt32(parameters.GetValue("PAGESIZE"))+
                         (records%Convert.ToInt32(parameters.GetValue("PAGESIZE"))==0 ? 0 : 1)).ToString()
            };
        }

        public int GetRecord(List<Parameter> parameters,string sqlId = DefaultSqlId.GetRecord)
        {
            return _dao.GetRecord(parameters.ToDbParameters(),sqlId);
        }

        /// <summary>
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public IEnumerable<TFEntity> GetList(List<Parameter> parameters,
            string sqlId = DefaultSqlId.GetList)
        {
            return _dao.GetList(parameters.ToDbParameters(),sqlId);
        }

        /// <summary>
        ///     查询实体
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public TFEntity GetEntity(List<Parameter> parameters,string sqlId = DefaultSqlId.GetEntity)
        {
            return _dao.GetEntity(parameters.ToDbParameters(),sqlId);
        }
    }
}