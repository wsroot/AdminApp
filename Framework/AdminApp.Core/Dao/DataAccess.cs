using System.Collections.Generic;
using System.Data;
using AdminApp.Core.Util;
using SmartSql;
using SmartSql.Abstractions;
using SmartSql.Abstractions.DbSession;

namespace AdminApp.Core.Dao
{
    public class DataAccess<TEntity>
    {
        public DataAccess(string scopeName = null)
        {
            SqlMapper = MapperContainer.Instance.GetSqlMapper();
            Scope = scopeName ?? typeof(TEntity).Name;
        }

        public ISmartSqlMapper SqlMapper { get; }
        public string Scope { get; protected set; }

        #region Read

        public virtual TEntity GetEntity(object paramObj, string sqlId = DefaultSqlId.GetEntity,
            DataSourceChoice sourceChoice = DataSourceChoice.Read)
        {
            return SqlMapper.QuerySingle<TEntity>(new RequestContext
            {
                DataSourceChoice = sourceChoice,
                Scope = Scope,
                SqlId = sqlId,
                Request = paramObj
            });
        }

        public virtual IEnumerable<TEntity> GetList(object paramObj, string sqlId = DefaultSqlId.GetList,
            DataSourceChoice sourceChoice = DataSourceChoice.Read)
        {
            return SqlMapper.Query<TEntity>(new RequestContext
            {
                DataSourceChoice = sourceChoice,
                Scope = Scope,
                SqlId = sqlId,
                Request = paramObj
            });
        }

        public virtual IEnumerable<TEntity> GetListByPage(object paramObj, string sqlId = DefaultSqlId.GetListByPage,
            DataSourceChoice sourceChoice = DataSourceChoice.Read)
        {
            return SqlMapper.Query<TEntity>(new RequestContext
            {
                DataSourceChoice = sourceChoice,
                Scope = Scope,
                SqlId = sqlId,
                Request = paramObj
            });
        }

        public virtual int GetRecord(object paramObj, string sqlId = DefaultSqlId.GetRecord,
            DataSourceChoice sourceChoice = DataSourceChoice.Read)
        {
            return SqlMapper.QuerySingle<int>(new RequestContext
            {
                DataSourceChoice = sourceChoice,
                Scope = Scope,
                SqlId = sqlId,
                Request = paramObj
            });
        }

        public virtual bool IsExist(object paramObj, string sqlId = DefaultSqlId.IsExist,
            DataSourceChoice sourceChoice = DataSourceChoice.Read)
        {
            return SqlMapper.QuerySingle<int>(new RequestContext
            {
                DataSourceChoice = sourceChoice,
                Scope = Scope,
                SqlId = sqlId,
                Request = paramObj
            }) > 0;
        }

        #endregion

        #region Write

        public virtual TPrimary Insert<TPrimary>(TEntity entity, string sqlId = DefaultSqlId.Insert)
        {
            var paramObj = new RequestContext
            {
                Scope = Scope,
                SqlId = sqlId,
                Request = entity
            };
            var isNoneIdentity = typeof(TPrimary) == typeof(NoneIdentity);
            if (!isNoneIdentity)
                return SqlMapper.ExecuteScalar<TPrimary>(paramObj);

            SqlMapper.Execute(paramObj);
            return default(TPrimary);
        }

        public virtual void Insert(TEntity entity, string sqlId = DefaultSqlId.Insert)
        {
            Insert<NoneIdentity>(entity, sqlId);
        }

        public virtual int Delete(object paramObj, string sqlId = DefaultSqlId.Delete)
        {
            return SqlMapper.Execute(new RequestContext
            {
                Scope = Scope,
                SqlId = sqlId,
                Request = paramObj
            });
        }

        public virtual int Update(TEntity entity, string sqlId = DefaultSqlId.Update)
        {
            return DynamicUpdate(entity, sqlId);
        }

        public virtual int DynamicUpdate(object entity, string sqlId = DefaultSqlId.Update)
        {
            return SqlMapper.Execute(new RequestContext
            {
                Scope = Scope,
                SqlId = sqlId,
                Request = entity
            });
        }

        #endregion

        #region Transaction

        /// <summary>
        ///     开启事务
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnectionSession BeginTransaction()
        {
            return SqlMapper.BeginTransaction();
        }

        public virtual IDbConnectionSession BeginTransaction(IsolationLevel isolationLevel)
        {
            return SqlMapper.BeginTransaction(isolationLevel);
        }

        /// <summary>
        ///     提交事务
        /// </summary>
        public virtual void CommitTransaction()
        {
            SqlMapper.CommitTransaction();
        }

        /// <summary>
        ///     回滚事务
        /// </summary>
        public virtual void RollbackTransaction()
        {
            SqlMapper.RollbackTransaction();
        }

        #endregion
    }
}