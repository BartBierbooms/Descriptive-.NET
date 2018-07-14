using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Example.ExampleBusiness
{
    public interface IKey
    {
        int GetID();
    }

    public interface IRepository<T> where T : IKey
    {

        T GetByID(int Id);
        void Save(T source);
    }

    public class Repository<T> : IRepository<T>
        where T : IKey
    {

        public T GetByID(int Id)
        {
            if (DBContext.Instance.Storage.TryGetValue((Id, typeof(T)), out object stored))
            {
                return (T)stored;
            }
            return default(T);
        }

        public void Save(T source)
        {
            int id = source.GetID();
            if (DBContext.Instance.Storage.ContainsKey((id, source.GetType())))
            {

                DBContext.Instance.Storage[(id, source.GetType())] = source;
            }
            else
            {
                DBContext.Instance.Storage.Add((id, source.GetType()), source);
            }
        }
    }

    public static class RepositoryExt
    {
        public static void Save<T>(this Repository<T> source, T val) where T : IKey
        {
            source.Save(val);
        }

        public static T Get<T>(this Repository<T> source, int key) where T : IKey
        {
            return source.Get(key);
        }

    }
}
