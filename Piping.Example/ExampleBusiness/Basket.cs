using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Example.ExampleBusiness
{

    public class Basket : IKey
    {

        public Customer Customer { get; set; }
        public Product Product { get; set; }
        public int ID { get; }
        public Stock Stock { get; private set; }
        public BasketResult Result { get; private set; }
        public bool IsDelivered { get; set; }

        public Basket() {
            Result = new BasketResult();
        }

        public Basket(int iD) : this()
        {
            ID = iD;
        }

        public void UseStock(Stock stock) {
            Serilog.Log.Information("Adding stock to Basket: {Stock}", stock.Name);
            this.Stock = stock;
        }

        public void AddProduct(Product product) {
            Serilog.Log.Information("Adding product to Basket: {Prouct}", product.Name);
            this.Product = product;
        }

        public void SetCustomer(Customer customer)
        {
            Serilog.Log.Information("Adding customer (Id) on Basket: {Customer}", customer.Id);
            this.Customer = customer;
        }

        public int GetID()
        {
            return ID;
        }

    }

    public static class BasketExt {

        public static ProductStockLocation DetermineStockLocation(this Basket source) {
            return StockService.GetNearestProductLocation(source.Product, source.Stock);
        }

    }
}
