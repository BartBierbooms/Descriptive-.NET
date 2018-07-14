using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Example.ExampleBusiness
{
    public class ProductStockLocation : IKey
    {
        public ProductStockLocation() { }

        public ProductStockLocation(int id, int amount, Product product, StockLocation location) {
            Id = id;
            Amount = amount;
            Product = product;
            Location = location;
        }

        public int Id { get; }
        public int Amount { get; set; }
        public Product Product { get; }
        public StockLocation Location { get; }

        public int GetID()
        {
            return this.Id;
        }
    }


}
