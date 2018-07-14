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
    public class PipingIInvariantTest
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
                .Then(c => c.Mark = Car.HondaMark)
                .Iff(CarExt.IsHonda)
                    .Then(CarExt.DriveFast)
                .Iff(CarExt.IsToyota)
                    .Then(CarExt.DriveSlow)
                 .EndIff()
                 .Then(c => c.Val.SetError())
                 .Then(CarExt.Park);

            var pipelineResult = (Option<Unit, CarWithIInvariant>)carInit(new Unit());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Validation);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(CarWithIInvariant));
            Assert.IsTrue(pipelineResult.SupplementVal.Speed == Car.SpeedFast);
            Assert.IsFalse(pipelineResult.SupplementVal.Parked);
            Assert.IsTrue(pipelineResult.ValidationResult.ErrorMessage == CarWithIInvariant.InValidError);

        }
    }
}
