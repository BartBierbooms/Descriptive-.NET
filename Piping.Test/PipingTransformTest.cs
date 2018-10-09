using Microsoft.VisualStudio.TestTools.UnitTesting;
using Piping.Test.TestData;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Test
{
    [TestClass]
    public class PipingTransformTest
    {

		[TestMethod]
		public void Join_PipeSegments()
		{

			var hondaSegment = Pipe.Init<Car, Dealer>(_ => new Dealer())
				.Then(c => c.SetMark(Car.HondaMark))
				.Then(d => d.Reputation = Dealer.eReputation.Good);

			var hondaDealerSegment = Pipe.Init<Car, Dealer>(() => new Dealer())
				.Then(c => c.DriveFast());

			var joinedPipe = hondaSegment.Join(hondaDealerSegment);

			var pipelineResult = (Option<Car, Dealer>)joinedPipe(new Car());

			Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
			Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
			Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Dealer));
			Assert.IsTrue(pipelineResult.SupplementVal.Reputation == Dealer.eReputation.Good);
			Assert.IsTrue(pipelineResult.Val.Speed== Car.SpeedFast);

        }

		[TestMethod]
		public void Join_PipeSegmentsError()
		{
			int nr = 0;
			int aLot = 20;

			var hondaSegment = Pipe.Init<Car, Dealer>(_ => new Dealer())
				.Then(c => c.SetMark(Car.HondaMark))
				.Then(d =>
				{
					d.Reputation = Dealer.eReputation.Good;
					var wontwork = aLot / nr;
				});

			var hondaDealerSegment = Pipe.Init<Car, Dealer>(() => new Dealer())
				.Then(c => c.DriveFast());

			var joinedPipe = hondaSegment.Join(hondaDealerSegment);
            var pipelineResult = (Option<Car, Dealer>)joinedPipe(new Car());

			Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Exception);
			Assert.IsTrue(pipelineResult.ExceptionVal is InvalidOperationException);
        }


        [TestMethod]
        public void Transform_CarAndDealerToPriceAndCustomer()
        {
            Price carToPrice(Car car)
            {
                var price = new Price();
                price.InitalPrice = car.Price;
                return price;
            }

            void chargeCustomer(Car car, Customer cust)
            {
                cust.BankAccount = cust.BankAccount - car.Price;
            }

            var payTime = Pipe.Init<Price, Customer>(_ => new Customer())
                .Then(c => c.BankAccount = 23000M);


            var carInit = Pipe.Init<Car, Dealer>(_ => new Dealer())
                .Then(c => c.SetMark(Car.HondaMark))
                .Then(d => d.Reputation = Dealer.eReputation.Good)
                .Then(carAndDealer => carAndDealer.SupplementVal.GiveDiscount(carAndDealer.Val))
                .Transform(carToPrice, payTime, chargeCustomer);

            var pipelineResult = (Option<Price, Customer>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Price));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Customer));
            Assert.IsTrue(pipelineResult.SupplementVal.BankAccount == 23000M - (Car.CARPRICE - Dealer.DISCOUNT));

        }

        [TestMethod]
        public void Transform_DealerAndCarToPriceAndCustomer()
        {
            Price carToPrice(Car car)
            {
                var price = new Price();
                price.InitalPrice = car.Price;
                return price;
            }

            void chargeCustomer(Car car, Customer cust)
            {
                cust.BankAccount = cust.BankAccount - car.Price;
            }

            var payTime = Pipe.Init<Price, Customer>(_ => new Customer())
                .Then(c => c.BankAccount = 23000M);


            var carInit = Pipe.Init<Dealer, Car>(_ => new Car())
                .Then(c => c.SetMark(Car.HondaMark))
                .Then(d => d.Reputation = Dealer.eReputation.Good)
                .Then(dealerAndCar => dealerAndCar.Val.GiveDiscount(dealerAndCar.SupplementVal))
                .Then(carToPrice, payTime, chargeCustomer);

            var pipelineResult = (Option<Price, Customer>)carInit(new Dealer());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Price));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Customer));
            Assert.IsTrue(pipelineResult.SupplementVal.BankAccount == 23000M - (Car.CARPRICE - Dealer.DISCOUNT));

        }

        [TestMethod]
        public void Transform_TransFormTheValFromCarToCustomer()
        {
            Customer ToCustomer(Car car)
            {
                return new Customer() { BankAccount = 23000M };
            }

            var carInit = Pipe.Init<Car, Dealer>(_ => new Dealer())
               .Then(c => c.SetMark(Car.HondaMark))
               .Then(d => d.Reputation = Dealer.eReputation.Good)
               .Then(carAndDealer => carAndDealer.SupplementVal.GiveDiscount(carAndDealer.Val))
               .Transform(ToCustomer);

            var pipelineResult = (Option<Customer, Dealer>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Customer));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Dealer));
            Assert.IsTrue(pipelineResult.Val.BankAccount == 23000M);

        }

        [TestMethod]
        public void Transform_TransFormTheSupplementedValFromDealerToCustomer()
        {
            Customer ToCustomer(Dealer dealer)
            {
                return new Customer() { BankAccount = 23000M };
            }

            var carInit = Pipe.Init<Car, Dealer>(_ => new Dealer())
               .Then(c => c.SetMark(Car.HondaMark))
               .Then(d => d.Reputation = Dealer.eReputation.Good)
               .Transform(ToCustomer);

            var pipelineResult = (Option<Car, Customer>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Customer));
            Assert.IsTrue(pipelineResult.SupplementVal.BankAccount == 23000M);

        }

        [TestMethod]
        public void Transform_TransFormTheValFromCarToCustomerAndInitializeNewSupplementedValue()
        {
            Customer ToCustomer(Car car)
            {
                return new Customer() { BankAccount = 23000M };
            }

            var carInit = Pipe.Init<Car, Dealer>(_ => new Dealer())
               .Then(c => c.SetMark(Car.HondaMark))
               .Then(d => d.Reputation = Dealer.eReputation.Good)
               .Transform(ToCustomer, () => new Price());

            var pipelineResult = (Option<Customer, Price>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Customer));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Price));
            Assert.IsTrue(pipelineResult.Val.BankAccount == 23000M);

        }

		[TestMethod]
        public void Transform_TransFormTheSupplementedFromDealerToCustomerAndInitializeNewSupplementedValue()
        {
            Customer ToCustomer(Dealer dealer)
            {
                return new Customer() { BankAccount = 23000M };
            }

            var carInit = Pipe.Init<Car, Dealer>(_ => new Dealer())
               .Then(c => c.SetMark(Car.HondaMark))
               .Then(d => d.Reputation = Dealer.eReputation.Good)
               .Transform(ToCustomer, () => new Price());

            var pipelineResult = (Option<Customer, Price>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Customer));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Price));
            Assert.IsTrue(pipelineResult.Val.BankAccount == 23000M);

        }

        [TestMethod]
        public void Transform_TransFormTheValFromCarToCustomerAndPrice()
        {
            Customer ToCustomer(Car Car)
            {
                return new Customer() { BankAccount = 23000M };
            }

            var priceInit = Pipe.Init<Customer, Price>(() => new Price())
                .Then(p => p.Discount = 10M)
                .Then(c => c.BankAccount = 20M);


            var carInit = Pipe.Init<Car, Dealer>(_ => new Dealer())
               .Then(c => c.SetMark(Car.HondaMark))
               .Then(d => d.Reputation = Dealer.eReputation.Good)
               .Transform(ToCustomer, priceInit);

            var pipelineResult = (Option<Customer, Price>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Customer));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Price));
            Assert.IsTrue(pipelineResult.Val.BankAccount == 20M);
            Assert.IsTrue(pipelineResult.SupplementVal.Discount == 10M);

        }

        [TestMethod]
        public void Transform_TransFormTheSupplementedValFromDealerToCustomerAndPrice()
        {
            Customer ToCustomer(Dealer dealer)
            {
                return new Customer() { BankAccount = 23000M };
            }

            var priceInit = Pipe.Init<Customer, Price>(() => new Price())
                .Then(p => p.Discount = 10M)
                .Then(c => c.BankAccount = 20M);

            var carInit = Pipe.Init<Car, Dealer>(_ => new Dealer())
               .Then(c => c.SetMark(Car.HondaMark))
               .Then(d => d.Reputation = Dealer.eReputation.Good)
               .Transform(ToCustomer, priceInit);

            var pipelineResult = (Option<Customer, Price>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Customer));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Price));
            Assert.IsTrue(pipelineResult.Val.BankAccount == 20M);
            Assert.IsTrue(pipelineResult.SupplementVal.Discount == 10M);

        }

    }
}
