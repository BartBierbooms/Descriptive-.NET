using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Test.TestData
{
    public class Storage
    {
        private List<object> state = new List<object>();
        public List<object> State => state;

        public Storage() {
        }

        public void AddToState(object val) {
            state.Add(val);
        }

        public Storage AddValToState(object val)
        {
            state.Add(val);
            return this;
        }


    }
    public static class StorageExt {

        public static Storage AddToState(this Storage source, IValueAndSupplement<Car, Storage> input)
        {
            source.AddToState(input.Val);
            return source;
        }

        public static Storage SetOnStorage(this Storage source, Car state)
        {
            source.AddToState(state);
            return source;
        }

        public static Car SetOnStorageAndReturnState(this Storage source, Car state)
        {
            source.AddToState(state);
            return state;
        }
    }
}
