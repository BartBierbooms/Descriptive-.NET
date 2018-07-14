using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Example.ExampleBusiness
{
    public class StockLocation : IKey
    {
        public StockLocation() { }
        public StockLocation(int Id, Address stockAddress, string name, int distance)
        {
            this.Id = Id;
            StockAddress = stockAddress;
            Name = name;
            Distance = distance;
        }

        public int Id { get; }
        public Address StockAddress { get; }
        public string Name { get; }
        public int Distance { get; }

        public int GetID()
        {
            return this.Id;
        }
    }
}
