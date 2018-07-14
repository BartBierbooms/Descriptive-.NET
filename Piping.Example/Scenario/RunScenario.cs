using Piping.Example.ExampleTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Example.ExampleBusiness.Scenario
{
    public static class RunScenario
    {
        public static void Run()
        {

            DBContextExt.ResetDBContext();

            var initBasketWithProductAndCustomer = Pipe.Init<Basket, Unit>(() => new Unit());

            Mail FromBasketToMail(Basket basket)
            {
                return new Mail() { MailMessage = "You will get your product: " + basket.Product.Name };
            }

            void FromBasket(Basket basket, OrderResult result)
            {
                result.DeliveryAddress = basket.Customer.Address;
            }


            var executeBasketOrder = initBasketWithProductAndCustomer
              .Then(basket => BasketScenario.addDiscountForPremiumCustomers((basket.Product, basket.Customer)))
              .Then(basket => BasketScenario.reserveAndPriceProduct(basket))
              .Then(basket => BasketScenario.saveBasket(basket))
              .TransForm(FromBasketToMail, Pipe.Init<Mail, OrderResult>(() => new OrderResult()), FromBasket);

            var pipeResult = executeBasketOrder(Test.CreateTestData());

            switch (pipeResult)
            {
                case Some<Mail, OrderResult> o:
                    Serilog.Log.Information("Scenario is been executed succesfull. Basket will be delivered on {Street}. Mail {Mail}", o.SupplementVal.DeliveryAddress.Street, o.Val.MailMessage);
                    break;
                case SomeException<Mail, OrderResult> o:
                    Serilog.Log.Error("Scenario is been executed unsuccesfull with exception: {Exception}", o.ExceptionVal.Message);
                    break;
                case None<Mail, OrderResult> o:
                    Serilog.Log.Information("Scenario encountered a null somewhere");
                    break;
                case Validation<Mail, OrderResult> o:
                    Serilog.Log.Information("Scenario encountered a validation or invariant constraint {Constraint}", o.ValidationResult.ErrorMessage);
                    break;
                default:
                    Console.WriteLine("There is no default. The pattern should be fully matched. Maybe you misMatched the typing");
                    break;
            }

        }
    }
}
