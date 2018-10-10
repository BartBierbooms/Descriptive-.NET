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
    public class PipingIValidTest
    {

        [TestMethod]
        public void Init_FalslyValidateOnInitWithCallBack_ReturnsException()
        {

            CarWithIInvariant CreateCar() => new CarWithIInvariant(false);

            var pipeline = Pipe.Init<CarWithIInvariant, Unit>(() => new Unit());

            var pipelineResult = (Option<CarWithIInvariant, Unit>)pipeline(CreateCar());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Validation);

            Assert.IsTrue(pipelineResult.ValidationResult.ErrorMessage == CarWithIInvariant.InValidError);
        }

        [TestMethod]
        public void CheckValidation()
        {

            var carInit = Pipe.Init<Unit, CarWithIInvariant>(_ => new CarWithIInvariant())
                .Then(c => c.Mark = "Honda")
                .Iff(CarExt.IsHonda)
                    .Then(c => CarExt.DriveFast(c))
                .Iff(CarExt.IsToyota)
                    .Then(c => CarExt.DriveSlow(c))
                 .EndIff()
                 .Then((CarWithIInvariant c) => c.SetError())
                 .Then(c => CarExt.Park(c));

            var pipelineResult = (Option<Unit, CarWithIInvariant>)carInit(new Unit());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Validation);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(CarWithIInvariant));
            Assert.IsTrue(pipelineResult.SupplementVal.Speed == Car.SpeedFast);
            Assert.IsFalse(pipelineResult.SupplementVal.Parked);
            Assert.IsTrue(pipelineResult.ValidationResult.ErrorMessage == CarWithIInvariant.InValidError);

        }
    }
}
