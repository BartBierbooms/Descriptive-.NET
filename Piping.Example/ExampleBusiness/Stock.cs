using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Example.ExampleBusiness
{
    public class Stock : IKey
    {
        public Stock(int iD, IList<StockLocation> locations, string name) {
            ID = iD;
            Locations = locations;
            Name = name;
        }

        public int ID { get; }
        public IList<StockLocation> Locations { get; }
        public string Name { get; }

        public int GetID()
        {
            return this.ID;
        }
    }
}
