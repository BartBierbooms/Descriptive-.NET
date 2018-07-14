using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Example.ExampleBusiness
{
    public class DBContext
    {

        public static DBContext Instance => GetInstance();
        private static DBContext instance = null;

        private IList<Product> Products { get; set; }
        private IList<Address> Addresses { get; set; }
        private IList<Stock> Stocks { get; set; }
        private IList<StockLocation> StockLocations { get; set; }
        private IList<ProductStockLocation> ProductStockLocations { get; set; }
        private IList<Customer> Customers { get; set; }

        public Dictionary<(int, Type), object> Storage = new Dictionary<(int, Type), object>();

        public static void Reset() {
            DBContext.instance = null;
        }

        private static DBContext GetInstance()
        {
            if (instance == null)
            {
                instance = new DBContext();
                instance.Products = new List<Product>(new[]
                {
                    new Product(1, "Honda Civic", 20000M),
                    new Product(2, "Toyota Auris", 18000M)
                });
                instance.Addresses = new List<Address>(new[] {
                    new Address(1, "Lintweg 2", "Culemborg", "1235 HJ", "NL"),
                    new Address(2, "Wegenbree 12", "Vlissingen", "8976 HJ", "NL"),
                    new Address(3, "6 High street", "London", "CHG 8", "UK"),
                    new Address(4, "7 Penny Lane", "Reading", "HGF 67", "UK")
                });
                instance.StockLocations = new List<StockLocation>(new[]
                {
                    new StockLocation(1, instance.Addresses.ElementAt(3), "Long Distance", 120),
                    new StockLocation(2, instance.Addresses.ElementAt(2), "Middle Distance", 10)
                });
                instance.Stocks = new List<Stock>( new[] { new Stock(1, instance.StockLocations, "Web Stock") });
                instance.ProductStockLocations = new List<ProductStockLocation>(new[] {
                    new ProductStockLocation(1, 2, instance.Products.ElementAt(0), instance.StockLocations.ElementAt(0)),
                    new ProductStockLocation(2, 0, instance.Products.ElementAt(0), instance.StockLocations.ElementAt(1)),
                    new ProductStockLocation(3, 20, instance.Products.ElementAt(1), instance.StockLocations.ElementAt(0)),
                    new ProductStockLocation(4, 122, instance.Products.ElementAt(1), instance.StockLocations.ElementAt(0))
                });
                instance.Customers = new List<Customer>(new[] { new Customer(1, instance.Addresses.ElementAt(1), Customer.ECustomerType.Premium) });
                SetUpStorage();
            }
            return instance;
        }

        private static void SetUpStorage()
        {

            Serilog.Log.Information("Fill storage with example data");

            foreach (var address in DBContext.Instance.Addresses)
            {
                DBContext.Instance.Storage.Add((((IKey)address).GetID(), typeof(Address)), address);
            }

            foreach (var product in DBContext.Instance.Products)
            {
                DBContext.Instance.Storage.Add((((IKey)product).GetID(), typeof(Product)), product);
            }

            foreach (var productStockLocation in DBContext.Instance.ProductStockLocations)
            {
                DBContext.Instance.Storage.Add((((IKey)productStockLocation).GetID(), typeof(ProductStockLocation)), productStockLocation);
            }

            foreach (var stockLocation in DBContext.Instance.StockLocations)
            {
                DBContext.Instance.Storage.Add((((IKey)stockLocation).GetID(), typeof(StockLocation)), stockLocation);
            }

            foreach (var stock in DBContext.Instance.Stocks)
            {
                DBContext.Instance.Storage.Add((((IKey)stock).GetID(), typeof(Stock)), stock);
            }

            foreach (var customer in DBContext.Instance.Customers)
            {
                DBContext.Instance.Storage.Add((((IKey)customer).GetID(), typeof(Customer)), customer);
            }

        }


    }
    public static class DBContextExt
    {

        public static void ResetDBContext()
        {
            Serilog.Log.Information("Reset DBContext");
            DBContext.Reset();
        }

        public static IEnumerable<ProductStockLocation> GetProductStockLocations() {
            return GetAll<ProductStockLocation>().Select(x => x.Value).Cast<ProductStockLocation>();
        }

        public static Product GetProduct(Func<Product, bool> predicate)
        {
            Serilog.Log.Information("Getting Product from storage");
            return GetAll<Product>().Select(x => x.Value).Cast<Product>().First(predicate);
        }

        private static IEnumerable<KeyValuePair<(int, Type), object>> GetAll<T>() {
            return DBContext.Instance.Storage.Where(x => x.Value is T);
        }

        public static Stock GetStock(Func<Stock, bool> predicate)
        {
            Serilog.Log.Information("Getting Stock from storage");
            return GetAll<Stock>().Select(x => x.Value).Cast<Stock>().First(predicate);
        }

        public static Customer GetCustomer(Func<Customer, bool> predicate)
        {
            Serilog.Log.Information("Getting Customer from storage");
            return GetAll<Customer>().Select(x => x.Value).Cast<Customer>().First(predicate);
        }

    }
}
