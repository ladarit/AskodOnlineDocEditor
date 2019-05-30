using System.Collections.Generic;
using System.Linq;
using AskodOnline.Editor.Business.Interfaces;
using AskodOnline.Editor.Models;

namespace AskodOnline.Editor.Business.Store
{
    public abstract class BaseStore<TModel> : IBaseStore<TModel>
        where TModel : BaseModel, new()
    {
        public abstract SynchronizedCollection<TModel> GetCollection { get; }

        public abstract TModel CreateRecordAndAddToCollection(object obj);

        public abstract void RemoveRecord(TModel obj);   



        public abstract IList<TModel> Find(object id);

        public virtual TModel CreateRecord(object id)
        {
            return new TModel { Id = (string)id };
        }

        public virtual TModel CreateRecordAndAddToCollection(SynchronizedCollection<TModel> collection, object obj)
        {
            var record = CreateRecord(obj);
            AddToCollection(collection, record);
            return record;
        }

        public virtual void AddToCollection(SynchronizedCollection<TModel> collection, TModel obj)
        {
            collection.Add(obj);
        }

        public virtual void RemoveRecord(SynchronizedCollection<TModel> collection, TModel obj)
        {
            collection.Remove(obj);
        }

        public virtual IList<TModel> Find(SynchronizedCollection<TModel> collection, TModel obj)
        {
            return collection.Where(t => t.Equals(obj)).ToList();
        }

        public virtual IList<TModel> Find(SynchronizedCollection<TModel> collection, object id)
        {
            return collection.Where(t => t.Id.Equals(id)).ToList();
        }

        public virtual IList<TModel> Find(SynchronizedCollection<TModel> collection, long counter)
        {
            return collection.Where(t => t.Counter.Equals(counter)).ToList();
        }
    }
}