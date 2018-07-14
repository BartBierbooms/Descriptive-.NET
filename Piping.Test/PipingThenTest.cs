using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Piping.Test.TestData;

namespace Piping.Test
{
    [TestClass]
    public class PipingThenTest
    {

        [TestMethod]
        public void Then_SuccessFull_WithLambda()
        {

            var carInit = Pipe.Init<Unit, Car>(_ => new Car())
                    .Then(c => c.Mark = Car.HondaMark);

            var pipelineResult = (Option<Unit, Car>)carInit(new Unit());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Car));
            Assert.IsTrue(pipelineResult.SupplementVal.Mark == Car.HondaMark);

        }

        [TestMethod]
        public void Then_SuccessFull_WithCallOnInstance()
        {

            var carInit = Pipe.Init<Unit, Car>(_ => new Car())
                    .Then(c => c.SetMark(Car.HondaMark));

            var pipelineResult = (Option<Unit, Car>)carInit(new Unit());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Car));
            Assert.IsTrue(pipelineResult.SupplementVal.Mark == Car.HondaMark);

        }

        [TestMethod]
        public void Then_SuccessFull_WithCallOnExtension()
        {

            var carInit = Pipe.Init<Unit, Car>(_ => new Car())
                    .Then(CarExt.DriveFast);

            var pipelineResult = (Option<Unit, Car>)carInit(new Unit());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Car));
            Assert.IsTrue(pipelineResult.SupplementVal.Speed == Car.SpeedFast);

        }

        [TestMethod]
        public void Then_PipingStopsOnException()
        {

            var carInit = Pipe.Init<Unit, Car>(_ => new Car())
                    .Then(c => c.SetMark("Unknown mark causing validations error"))
                    .Then(CarExt.Validate)
                    .Then(CarExt.DriveFast);

            var pipelineResult = (Option<Unit, Car>)carInit(new Unit());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Exception);
            Assert.IsTrue(pipelineResult.ExceptionVal.Message.Length > 0);
            Assert.IsNull(pipelineResult.SupplementVal);
        }

        [TestMethod]
        public void Then_SuccessFull_ActionOnVal()
        {

            var carInit = Pipe.Init<Car, Unit>(_ => new Unit())
                    .Then(c => c.SetMark(Car.HondaMark));

            var pipelineResult = (Option<Car, Unit>)carInit(new Car());

            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
            Assert.IsTrue(pipelineResult.Val.IsHonda());

        }

        [TestMethod]
        public void Then_SuccessFull_ActionOnSupplementedVal()
        {

            var carInit = Pipe.Init<Unit, Car>(_ => new Car())
                    .Then(car => car.SetMark(Car.HondaMark));

            var pipelineResult = (Option<Unit, Car>)carInit(new Unit());
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Car));
            Assert.IsTrue(pipelineResult.SupplementVal.IsHonda());

        }

        [TestMethod]
        public void Then_SuccessFull_ActionOnValAndSupplementedVal()
        {
            var carInit = Pipe.Init<Storage, Car>(_ => new Car())
                    .Then(c => c.SetMark(Car.HondaMark))
                    .Then(carAndStorage => CarExt.SetOnStorage(carAndStorage.Val, carAndStorage.SupplementVal));

            var pipelineResult = (Option<Storage, Car>)carInit(new Storage());
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Car));
            Assert.IsTrue(pipelineResult.SupplementVal.IsHonda());


        }

        [TestMethod]
        public void Then_SuccessFull_ActionOnSupplementedValAndVal()
        {
            var carInit = Pipe.Init<Car, Storage>(_ => new Storage())
                    .Then(c => c.SetMark(Car.HondaMark))
                    .Then(carAndStorage => StorageExt.SetOnStorage(carAndStorage.Val, carAndStorage.SupplementVal));

            var pipelineResult = (Option<Car, Storage>)carInit(new Car());
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Storage));
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
            Assert.IsTrue(pipelineResult.Val.IsHonda());
            Assert.IsInstanceOfType(pipelineResult.SupplementVal.State.First(), typeof(Car));
            Assert.IsTrue(((Car)pipelineResult.SupplementVal.State.First()).IsHonda());

        }

        [TestMethod]
        public void Then_SuccessFull_FuncOnVal()
        {
            var carInit = Pipe.Init<Unit, Car>(_ => new Car())
                    .Then(c => c.SetParked(true));

            var pipelineResult = (Option<Unit, Car>)carInit(new Unit());

            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Car));
            Assert.IsTrue(pipelineResult.SupplementVal.Parked);

        }

        [TestMethod]
        public void Then_SuccessFull_FuncOnSupplementedValAndVal()
        {
            var carInit = Pipe.Init<Storage, Car>(_ => new Car())
                    .Then(c => c.Val.SetParked(true, c.SupplementVal));

            var pipelineResult = (Option<Storage, Car>)carInit(new Storage());

            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Car));
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Storage));
            Assert.IsTrue(pipelineResult.SupplementVal.Parked);
            Assert.IsInstanceOfType(pipelineResult.Val.State.First(), typeof(Car));
            Assert.IsTrue(((Car)pipelineResult.Val.State.First()).Parked);

        }

        [TestMethod]
        public void Then_SuccessFull_FuncOnValueAndSupplementedStateValueExt()
        {

            var carInit = Pipe.Init<Storage, Car>(_ => new Car())
                    .Then(c => c.SetMark(Car.HondaMark))
                    .Then(CarExt.Validate)
                    .Then(carAndStorage => StorageExt.AddToState(carAndStorage.SupplementVal, carAndStorage))
                    .Then(CarExt.DriveFast);

            var pipelineResult = (Option<Storage, Car>)carInit(new Storage());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Car));
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Storage));

            Assert.IsTrue(pipelineResult.Val.State.Count == 1);
            Assert.IsInstanceOfType(pipelineResult.Val.State.First(), typeof(Car));
            Assert.IsTrue(((Car)pipelineResult.Val.State.First()).Speed == Car.SpeedFast);
        }

        [TestMethod]
        public void Then_SuccessFull_FuncOnValueAndSupplementedStateStateExt()
        {

            var carInit = Pipe.Init<Car, Storage>(_ => new Storage())
                    .Then(c => c.SetMark(Car.HondaMark))
                    .Then(CarExt.Validate)
                    .Then(StorageAndCar => CarExt.AddCarToState(StorageAndCar.SupplementVal, StorageAndCar))
                    .Then(CarExt.DriveFast);

            var pipelineResult = (Option<Car, Storage>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Storage));
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));

            Assert.IsTrue(pipelineResult.SupplementVal.State.Count == 1);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal.State.First(), typeof(Car));
            Assert.IsTrue(((Car)pipelineResult.SupplementVal.State.First()).Speed == Car.SpeedFast);
        }

        [TestMethod]
        public void Then_SuccessFull_FuncOnSupplementedStateToValState()
        {

            var carInit = Pipe.Init<Storage, Car>(_ => new Car())
                    .Then(c => c.SetMark(Car.HondaMark))
                    .Then(CarExt.Validate)
                    .Then(x => StorageExt.SetOnStorageAndReturnState(x.SupplementVal, x.Val))
                    .Then(CarExt.DriveFast);

            var pipelineResult = (Option<Storage, Car>)carInit(new Storage());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Car));
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Storage));

            Assert.IsTrue(pipelineResult.Val.State.Count == 1);
            Assert.IsInstanceOfType(pipelineResult.Val.State.First(), typeof(Car));
            Assert.IsTrue(((Car)pipelineResult.Val.State.First()).Speed == Car.SpeedFast);
        }

        [TestMethod]
        public void Then_SuccessFull_FuncOnState()
        {

            var carInit = Pipe.Init<Car, Storage>(_ => new Storage())
                    .Then(c => c.SetMark(Car.HondaMark))
                    .Then(x => x.Val.AddValToState(x.SupplementVal))
                    .Then(CarExt.DriveFast);

            var pipelineResult = (Option<Car, Storage>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Storage));
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));

            Assert.IsTrue(pipelineResult.SupplementVal.State.Count == 1);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal.State.First(), typeof(Car));
            Assert.IsTrue(((Car)pipelineResult.SupplementVal.State.First()).Speed == Car.SpeedFast);

        }

        [TestMethod]
        public void Then_SuccessFull_OnActionOnValAndSupplementedVal()
        {
            var carInit = Pipe.Init<Car, Dealer>(_ => new Dealer())
                    .Then(c => c.Mark = Car.HondaMark)
                    .Then(c => c.Price = 20000M)
                    .Then(carAndDealer => carAndDealer.SupplementVal.GiveDiscount(carAndDealer.Val));

            var pipelineResult = (Option<Car, Dealer>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Dealer));
            Assert.IsTrue(pipelineResult.Val.Price == 20000M - Dealer.DISCOUNT);

        }

        [TestMethod]
        public void Then_SuccessFull_OnActionOnValAndSupplementedVal2()
        {

            void GiveDiscount(Car car, Dealer dealer) {
                car.Price = car.Price - Dealer.DISCOUNT;
            }

            var carInit = Pipe.Init<Car, Dealer>(_ => new Dealer())
                  .Then(c => c.Mark = Car.HondaMark)
                  .Then(c => c.Price = Car.CARPRICE)
                  .Then(GiveDiscount);

            var pipelineResult = (Option<Dealer, Car>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Dealer));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Car));
            Assert.IsTrue(pipelineResult.SupplementVal.Price == Car.CARPRICE - Dealer.DISCOUNT);

        }

        [TestMethod]
        public void Then_SuccessFull_OnActionOnValAndSupplementedVal3()
        {

            void GiveDiscount(Dealer dealer, Car car)
            {
                car.Price = car.Price - Dealer.DISCOUNT;
            }

            var carInit = Pipe.Init<Car, Dealer>(_ => new Dealer())
                  .Then(c => c.Mark = Car.HondaMark)
                  .Then(c => c.Price = 20000M)
                  .Then(GiveDiscount);

            var pipelineResult = (Option<Car, Dealer>)carInit(new Car());
            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Dealer));
            Assert.IsTrue(pipelineResult.Val.Price == 20000M - Dealer.DISCOUNT);

        }

        [TestMethod]
        public void Then_SuccessFull_OnCarFunction()
        {

            Car ToCar(Dealer dealer, Car car)
            {
                car.Price = Car.CARPRICE;
                dealer.GiveDiscount(car);
                return car;
            }

            var carInit = Pipe.Init<Car, Dealer>(_ => new Dealer())
                  .Then(x => ToCar(x.SupplementVal, x.Val));

            var pipelineResult = (Option<Car, Dealer>)carInit(new Car());
            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Dealer));
            Assert.IsTrue(pipelineResult.Val.Price == Car.CARPRICE - Dealer.DISCOUNT);

        }

        [TestMethod]
        public void Then_SuccessFull_DealerFunction()
        {

            Dealer ToDealer(Car car)
            {
                var dealer = new Dealer();
                car.Price = car.Price - Dealer.DISCOUNT;
                return dealer;
            }

            var carInit = Pipe.Init<Car, Dealer>(_ => new Dealer())
                  .Then(c => c.Mark = Car.HondaMark)
                  .Then(c => c.Price = 20000M)
                  .Then(c=> ToDealer(c.SupplementVal));

            var pipelineResult = (Option<Car, Dealer>)carInit(new Car());
            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Dealer));
            Assert.IsTrue(pipelineResult.Val.Price == 20000M - Dealer.DISCOUNT);

        }


    }
}
