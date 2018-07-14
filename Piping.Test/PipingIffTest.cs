using Microsoft.VisualStudio.TestTools.UnitTesting;
using Piping.Test.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Test
{
    [TestClass]
    public class PipingIffTest
    {
        [TestMethod]
        public void iff_ExecuteFirstPredicate()
        {
            var carInit = Pipe.Init<Car, Unit>(_ => new Unit())
                    .Then(c => c.Mark = Car.HondaMark)
                    .Iff(CarExt.IsHonda)
                        .Then(CarExt.DriveFast);

            var pipelineResult = (Option<Car, Unit>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Unit));
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
            Assert.IsTrue(pipelineResult.Val.Speed== Car.SpeedFast);
        }

        [TestMethod]
        public void iff_DontExecuteFirstPredicate()
        {
            var carInit = Pipe.Init<Car, Unit>(_ => new Unit())
                    .Then(c => c.Mark = Car.ToyotaMark)
                    .Iff(CarExt.IsHonda)
                        .Then(CarExt.DriveFast);

            var pipelineResult = (Option<Car, Unit>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Unit));
            Assert.IsTrue(pipelineResult.Val.Speed == Car.NoSpeed);
            Assert.IsTrue(pipelineResult.Val.Mark == Car.ToyotaMark);
        }

        [TestMethod]
        public void iff_ExecuteSecondPredicate()
        {
            var carInit = Pipe.Init<Car, Unit>(_ => new Unit())
                    .Then(c => c.Mark = Car.ToyotaMark)
                    .Iff(CarExt.IsHonda)
                        .Then(CarExt.DriveFast)
                    .Iff(CarExt.IsToyota)
                        .Then(CarExt.DriveSlow);

            var pipelineResult = (Option<Car, Unit>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Unit));
            Assert.IsTrue(pipelineResult.Val.Speed == Car.SpeedSlow);
            Assert.IsTrue(pipelineResult.Val.Mark == Car.ToyotaMark);
        }

        [TestMethod]
        public void iff_ExecuteFirstPredicateAndContinueAfterEndIf()
        {
            var carInit = Pipe.Init<Car, Unit>(_ => new Unit())
                .Then(c => c.Mark = Car.HondaMark)
                .Iff(CarExt.IsHonda)
                    .Then(CarExt.DriveFast)
                    .Then(CarExt.DriveFar)
                .EndIff()
                .Then(CarExt.Park);

            var pipelineResult = (Option<Car, Unit>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Unit));
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
            Assert.IsTrue(pipelineResult.Val.Speed == Car.SpeedFast);
            Assert.IsTrue(pipelineResult.Val.NeedFuel);
            Assert.IsTrue(pipelineResult.Val.Parked);
        }

        [TestMethod]
        public void iff_ExecuteSecondPredicateAndContinueAfterEndIf()
        {
            var carInit = Pipe.Init<Car, Unit>(_ => new Unit())
                    .Then(c => c.Mark = Car.ToyotaMark)
                    .Iff(CarExt.IsHonda)
                        .Then(CarExt.DriveFast)
                    .Iff(CarExt.IsToyota)
                        .Then(CarExt.DriveSlow)
                        .Then(CarExt.DriveFar)
                     .EndIff()
                     .Then(CarExt.Park);

            var pipelineResult = (Option<Car, Unit>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Unit));
            Assert.IsTrue(pipelineResult.Val.Speed == Car.SpeedSlow);
            Assert.IsTrue(pipelineResult.Val.NeedFuel);
            Assert.IsTrue(pipelineResult.Val.Mark == Car.ToyotaMark);
            Assert.IsTrue(pipelineResult.Val.Parked);

        }

        [TestMethod]
        public void iff_NoPredicateMatchContinueExecutingAfterEndIf()
        {
            var carInit = Pipe.Init<Car, Unit>(_ => new Unit())
                    .Then(c => c.Mark = "Unknown Mark")
                    .Iff(CarExt.IsHonda)
                        .Then(CarExt.DriveFast)
                    .Iff(CarExt.IsToyota)
                        .Then(CarExt.DriveSlow)
                     .EndIff()
                     .Then(CarExt.Park);

            var pipelineResult = (Option<Car, Unit>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Unit));
            Assert.IsTrue(pipelineResult.Val.Speed == Car.NoSpeed);
            Assert.IsTrue(pipelineResult.Val.Parked);

        }

        [TestMethod]
        public void iff_ExecuteNoPredicateMatchReturnErrorWithStrongEndIfF()
        {
            var carInit = Pipe.Init<Car, Unit>(_ => new Unit())
                    .Then(c => c.Mark = "Unknown Mark")
                    .Iff(CarExt.IsHonda)
                        .Then(CarExt.DriveFast)
                    .Iff(CarExt.IsToyota)
                        .Then(CarExt.DriveSlow)
                     .EndIffStrong()
                     .Then(CarExt.Park);

            var pipelineResult = (Option<Car, Unit>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Exception);
            Assert.IsTrue(pipelineResult.ExceptionVal.Message.Length > 0);

        }

        [TestMethod]
        public void iff_NestedIif()
        {
            var carInit = Pipe.Init<Car, Unit>(_ => new Unit())
                    .Then(c => c.Mark = Car.HondaMark)
                    .Iff(CarExt.IsHonda)
                        .Then(CarExt.DriveFast)
                            .Iff(c => c.Speed == Car.NoSpeed)
                                .Then(c => c.Price = 1000M)
                            .Iff(c => c.Speed == Car.SpeedFast)
                                .Then(c => c.Price = 2000M)
                            .EndIff()
                            .Then(CarExt.Park)
                    .EndIff()
                    .Then(c => c.Val.NeedFuel = true);


            var pipelineResult = (Option<Car, Unit>)carInit(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Unit));
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
            Assert.IsTrue(pipelineResult.Val.Speed == Car.SpeedFast);
            Assert.IsTrue(pipelineResult.Val.Price == 2000M);
            Assert.IsTrue(pipelineResult.Val.NeedFuel);
            Assert.IsTrue(pipelineResult.Val.Parked);

        }


    }
}
