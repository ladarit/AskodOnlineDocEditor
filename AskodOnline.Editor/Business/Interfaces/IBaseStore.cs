using System.Collections.Generic;

namespace AskodOnline.Editor.Business.Interfaces
{
    public interface IBaseStore<TModel>
    {
        SynchronizedCollection<TModel> GetCollection { get; }

        TModel CreateRecordAndAddToCollection(object obj);

        void RemoveRecord(TModel obj);

        IList<TModel> Find(object id);

        TModel CreateRecord(object obj);

        TModel CreateRecordAndAddToCollection(SynchronizedCollection<TModel> collection, object obj);

        void RemoveRecord(SynchronizedCollection<TModel> collection, TModel obj);

        IList<TModel> Find(SynchronizedCollection<TModel> collection, TModel obj);

        IList<TModel> Find(SynchronizedCollection<TModel> collection, object id);

        IList<TModel> Find(SynchronizedCollection<TModel> collection, long counter);

    }
}