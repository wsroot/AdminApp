using System;
using System.Collections.Generic;
using AdminApp.Core.Dao;
using AdminApp.Core.Util;

namespace AdminApp.Core.Service
{
    public class ServiceContext<TFEntity>
    {
        public ServiceContext(string scopeName = null)
        {
            Dao = new DataAccess<TFEntity>(scopeName);
        }

        private DataAccess<TFEntity> Dao { get; }

        /// <summary>
        ///     保存
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isExistSqlId"></param>
        /// <param name="updateSqlId"></param>
        /// <param name="insertSqlId"></param>
        /// <returns></returns>
        public bool Save(TFEntity entity, string isExistSqlId = DefaultSqlId.IsExist,
            string updateSqlId = DefaultSqlId.Update, string insertSqlId = DefaultSqlId.Insert)
        {
            if (Dao.IsExist(entity, isExistSqlId)) return Dao.Update(entity, updateSqlId) > 0;
            Dao.Insert(entity, insertSqlId);
            return true;
        }
        /// <summary>
        ///     新增
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="insertSqlId"></param>
        /// <returns></returns>
        public bool Insert(TFEntity entity,string insertSqlId = DefaultSqlId.Insert)
        {
            Dao.Insert(entity,insertSqlId);
            return true;
        }

        /// <summary>
        ///     更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateSqlId"></param>
        /// <returns></returns>
        public bool Update(TFEntity entity,string updateSqlId = DefaultSqlId.Update)
        {
            return Dao.Update(entity, updateSqlId) > 0;
        }

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="obj">对象参数</param>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public bool Delete(object obj, string sqlId = DefaultSqlId.Delete)
        {
            return Dao.Delete(obj, sqlId) > 0;
        }

        /// <summary>
        ///     根据ID删除
        /// </summary>
        /// <param name="id">此属性访问必须全大写</param>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public bool Delete(string id, string sqlId = DefaultSqlId.Delete)
        {
            return Dao.Delete(new {ID = id}, sqlId) > 0;
        }

        /// <summary>
        ///     根据动态参数查询
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pageSqlId"></param>
        /// <param name="recordSqlId"></param>
        /// <returns></returns>
        public PageInfo<TFEntity> GetListByPage(object obj = null, string pageSqlId = DefaultSqlId.GetListByPage,
            string recordSqlId = DefaultSqlId.GetRecord)
        {
            if (obj == null)
                obj = new {PAGENUM = 1, PAGESIZE = 20};
            var pageData = Dao.GetListByPage(obj, pageSqlId);
            var records = Dao.GetRecord(obj, recordSqlId);
            return new PageInfo<TFEntity>
            {
                page = obj.GetValue("PAGENUM").ToString(),
                records = records.ToString(),
                rows = pageData,
                total = (records / Convert.ToInt32(obj.GetValue("PAGESIZE")) +
                         (records % Convert.ToInt32(obj.GetValue("PAGESIZE")) == 0 ? 0 : 1)).ToString()
            };
        }

        /// <summary>
        ///     根据动态参数获取记录数
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public virtual int GetRecord(object obj = null, string sqlId = DefaultSqlId.GetRecord)
        {
            return Dao.GetRecord(obj, sqlId);
        }

        /// <summary>
        ///     根据动态参数获取列表
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public IEnumerable<TFEntity> GetList(object obj = null, string sqlId = DefaultSqlId.GetList)
        {
            return Dao.GetList(obj, sqlId);
        }

        /// <summary>
        ///     根据动态参数获取实体
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public TFEntity GetEntity(object obj, string sqlId = DefaultSqlId.GetEntity)
        {
            return Dao.GetEntity(obj, sqlId);
        }

        /// <summary>
        ///     查询实体
        /// </summary>
        /// <param name="id">此属性访问必须全大写</param>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public TFEntity GetEntity(string id, string sqlId = DefaultSqlId.GetEntity)
        {
            return Dao.GetEntity(new {Id = id}, sqlId);
        }
    }
}