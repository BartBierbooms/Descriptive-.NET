using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Piping;
using Piping.Example.ExampleTest;
using static Piping.Pipe;

namespace Piping.Example.ExampleBusiness.Scenario
{
    public static class BasketScenario
    {

        public static ToValueSupplementValue<Basket, Repository<IKey>, Basket> saveBasket =
            Pipe.Init<Basket, Repository<IKey>>(() => new Repository<IKey>())
                .Then((obj, repo) => repo.Save(obj));

        public static ToValueSupplementValue<Basket, Basket, ProductStockLocation> reserveBasketProuct =
            Pipe.Init<Basket, ProductStockLocation>(basket => basket.DetermineStockLocation())
            .Iff((ProductStockLocation location) => location == null)
                .Then(basket => basket.Result.ProductIsOnStock = false)
            .Else()
                .Then(basket => basket.Result.ProductIsOnStock = true)
                .Then(stockLoc => stockLoc.Amount--)
                .Then(stockLoc => new Repository<ProductStockLocation>().Save(stockLoc))
            .EndIffStrong();

        /// <summary>
        /// Example of nesting. nest reserveBasketProuct!
        /// </summary>
        public static ToValueSupplementValue<Basket, ProductStockLocation, Basket> reserveBasketProuctAndAddTransportCost =
            Pipe.Init(reserveBasketProuct)
                .Then((basket, psl) =>
                {
                    basket.Result.Price = basket.Product.Price
                        + PriceService.AddTransportPriceForForeignAddress(psl.Location.StockAddress, basket.Customer.Address);
                });

        public static ToValueSupplementValue<(Product product, Customer customer), (Product product, Customer customer), Unit> addDiscountForPremiumCustomers =
            Pipe.Init<(Product product, Customer customer), Unit>()
                .Iff(prodcustomer => prodcustomer.customer.CustomerType == Customer.ECustomerType.Premium)
                    .Then(prodcustomer => prodcustomer.product.Price = prodcustomer.product.Price - 250.0m);

        /// <summary>
        /// Example of double Nesting
        /// </summary>
        public static ToValueSupplementValue<Basket, Basket, ProductStockLocation> reserveAndPriceProduct =
            reserveBasketProuct.Then(basket => reserveBasketProuctAndAddTransportCost(basket));

    }
}
