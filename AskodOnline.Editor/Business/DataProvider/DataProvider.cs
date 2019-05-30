using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AskodOnline.Editor.Business.Interfaces;
using AskodOnline.Editor.Business.Store;
using AskodOnline.Editor.Helpers;
using NHibernate;

namespace AskodOnline.Editor.Business.DataProvider
{
    public class DataProvider<TEntity>: IDataProvider<TEntity> where TEntity : class
    {
        protected readonly log4net.ILog Log = Log4Net.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ISession Session => DbConnectionUnit.Current.Session;

        public async Task<TEntity> GetRecordByCounterAsync(long counter)
        {
            var lambda = LambdaExpressionsHelper<TEntity>.CreateLambdaExpression("Counter", counter);
            return await GetRecordAsync(lambda);
        }

	    public async Task<TEntity> GetRecordByIdAsync(string id)
	    {
		    var lambda = LambdaExpressionsHelper<TEntity>.CreateLambdaExpression("id", id);
		    return await GetRecordAsync(lambda);
	    }

		public async Task<T> RunSqlQuery<T>(string sql)
        {
            return await Task.FromResult(Session.CreateSQLQuery(sql).UniqueResult<T>());
        }

        public async Task<TEntity> GetRecordByRowIdAsync(string rowId)
        {
            var lambda = LambdaExpressionsHelper<TEntity>.CreateLambdaExpression("RowId", rowId);
            return await GetRecordAsync(lambda);
        }

        public async Task<TEntity> GetRecordByPropertyAsync(string propertyName, object propertyValue)
        {
            var lambda = LambdaExpressionsHelper<TEntity>.CreateLambdaExpression(propertyName, propertyValue);
            return await GetRecordAsync(lambda);
        }

	    public async Task<IList<TEntity>> GetRecordsAsync()
	    {
		    try
		    {
			    Session.Clear();
				return await Task.FromResult(Session.Query<TEntity>().ToList());
		    }
		    catch (HibernateException ex)
		    {
			    Log.Error(ex);
			    return null;
		    }
		    catch (Exception e)
		    {
			    Log.Error(e);
			    return null;
		    }
	    }

		public async Task<TEntity> GetRecordAsync(Expression<Func<TEntity, bool>> lambda)
        {
            try
            {
                Session.Clear();
                return await Task.FromResult(Session.Query<TEntity>().FirstOrDefault(lambda));
            }
            catch (HibernateException ex)
            {
                Log.Error(ex);
                return null;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public async Task<bool> UpdateRecordAsync(TEntity obj)
        {
            try
            {
                Session.Clear();
                Session.Update(obj);
                return await Task.FromResult(true);
            }
            catch (HibernateException ex)
            {
                Log.Error(ex);
                return await Task.FromResult(false);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return await Task.FromResult(false);
            }
        }
    }
}