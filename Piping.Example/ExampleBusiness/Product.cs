using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Example.ExampleBusiness
{
    public class Product : IKey
    {
        public Product(int iD, string Name, decimal price)
        {
            ID = iD;
            this.Name = Name;
            Price = price;
        }

        public int ID { get; }
        public string Name { get; }
        public decimal Price { get; set; }
        public IList<StockLocation> StockLocations { get; set; }

        public int GetID()
        {
            return this.ID;
        }
    }
}
