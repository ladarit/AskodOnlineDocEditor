using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AskodOnline.Editor.Business.Interfaces
{
    public interface IDataProvider<TEntity>
    {
	    Task<IList<TEntity>> GetRecordsAsync();

		Task<TEntity> GetRecordAsync(Expression<Func<TEntity, bool>> lambda);

	    Task<TEntity> GetRecordByIdAsync(string id);

		Task<TEntity> GetRecordByCounterAsync(long counter);

        Task<TEntity> GetRecordByRowIdAsync(string rowId);

        Task<TEntity> GetRecordByPropertyAsync(string propertyName, object propertyValue);

        Task<bool> UpdateRecordAsync(TEntity obj);

        Task<T> RunSqlQuery<T>(string sql);
    }
}