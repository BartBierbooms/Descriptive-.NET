using Piping.Example.ExampleBusiness.Scenario;
using Piping.Example.ExampleBusiness;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Example.ExampleTest
{
    public class Test
    {
        public static void TestMe(int choice)
        {
            Basket basket;
            DBContextExt.ResetDBContext();

            if (choice == 1)
            {
                Serilog.Log.Information("Test saveBasket. Test a changed basket is saved to the storage");

                basket = CreateTestData();
                basket.IsDelivered = true;

                var ret = BasketScenario.saveBasket
                    .Then(AssertDefine.AssertionOnSaveBasket)(basket);

                Serilog.Log.Information("-- End Test saveBasker");
            }
            if (choice == 2)
            {

                Serilog.Log.Information("Test reserveProduct: Reserve the product on thw nearest stock location, decrease stock amount and save to DB.");

                basket = CreateTestData();

                AssertDefine.curAmount = basket.DetermineStockLocation().Amount;
                Serilog.Log.Information("Determine current stock amount {StockLocation}", AssertDefine.curAmount);

                var ret1 = BasketScenario.reserveBasketProuct
                    .Then(AssertDefine.AssertDecreaseOfStock)
                    .Then(AssertDefine.AssertTheBaskerIsStoredToDatabase)
                    (basket);

                Serilog.Log.Information("-- End Test reserveProduct");
            }
            if (choice == 3)
            {

                var initBasketWithProductAndCustomer = Pipe.Init<Basket, Unit>(() => new Unit());

                var executeBasketOrder = initBasketWithProductAndCustomer
                    .Then(thebasket => BasketScenario.addDiscountForPremiumCustomers((thebasket.Product, thebasket.Customer)))
                    .Then(thebasket => BasketScenario.reserveAndPriceProduct(thebasket))
                        .Then(thebasket => AssertDefine.AssertProductIsReserved(thebasket))
                        .Then(thebasket => AssertDefine.AssertPriceIsAdjustedBecausOfPremiumCustomerAndDeliveryInAnotherCountry(thebasket))
                    .Then(thebasket => BasketScenario.saveBasket(thebasket));


                var pipeResult = executeBasketOrder(CreateTestData());

                switch (pipeResult)
                {
                    case Some<Basket, Unit> o:
                        Serilog.Log.Information("Scenario is been executed succesfull. Basket will be delivered on {Street}", o.Val.Customer.Address.Street);
                        break;
                    default:
                        Serilog.Log.Error("Something went wrong, check the logs.");
                        break;

                }
            }
        }

        public static Basket CreateTestData()
        {


            DBContextExt.ResetDBContext();

            Serilog.Log.Information("Create Basket with Honda Civic for customer nr. 1 using the stock Location: Web stock");
            var basket = new Basket(1);

            basket.AddProduct(DBContextExt.GetProduct(prod => prod.Name == "Honda Civic"));
            basket.UseStock(DBContextExt.GetStock(s => s.Name == "Web Stock"));
            basket.SetCustomer(DBContextExt.GetCustomer(c => c.Id == 1));

            Serilog.Log.Information("---TestData created.");
            return basket;
        }
    }
}
