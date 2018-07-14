using Piping.Example.ExampleBusiness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Example.ExampleTest
{
    public static class AssertDefine
    {
        public static int curAmount { get; set; }
        public static void AssertionOnSaveBasket(Repository<IKey> repo, Basket thebasket)
        {
            Serilog.Log.Information("Assert the basket saved in the storage is delivered");
            var savedBasket = new Repository<Basket>().GetByID(thebasket.ID);
            Assert.IsTrue(savedBasket.IsDelivered);
        }

        public static void AssertDecreaseOfStock(Basket testBasket, ProductStockLocation stockLocation)
        {
            Serilog.Log.Information("Assert reserveProduct decreases stock amount with one");
            Assert.IsTrue(curAmount - 1 == stockLocation.Amount);
        }

        public static void AssertTheBaskerIsStoredToDatabase(Basket testBasket, ProductStockLocation stockLocation)
        {
            Serilog.Log.Information("Assert the changed ProductStock is saved to the database");
            Assert.AreEqual(new Repository<ProductStockLocation>().GetByID(1).Amount, stockLocation.Amount);
        }

        public static void AssertProductIsReserved(Basket testBasket)
        {
            Serilog.Log.Information("Assert the changed ProductStock is saved to the database");
            Assert.AreEqual(new Repository<ProductStockLocation>().GetByID(1).Amount, 0);
        }

        public static void AssertPriceIsAdjustedBecausOfPremiumCustomerAndDeliveryInAnotherCountry(Basket testBasket)
        {
            Serilog.Log.Information("Assert as a foreign delivery address, I need to pay an extra fee, but as a premium customer i get a discount");
            var customerPrice = new Repository<Product>().GetByID(1).Price;
            Assert.AreEqual(testBasket.Result.Price, customerPrice + PriceService.ForeignCountryFee);
        }

    }
}
