using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Piping.Test.TestData;
using System.Collections.Generic;
using System.Linq;

namespace Piping.Test
{
    [TestClass]
    public class PipingInitTest
    {
        [TestMethod]
        public void Init_SupplyingNullAsInitValue_ReturnsExceptionOption()
        {
            var pipeline = Pipe.Init<Car, Unit>();
            var pipelineResult = (Option<Car, Unit>)pipeline(default(Car));
            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Exception);
        }

        [TestMethod]
        public void Init_SupplingInvalidInitFunction_ReturnsExceptionOption()
        {
            Unit CreateInValidUnit() => throw new InvalidOperationException("Invalid Unit creation");

            var pipeline = Pipe.Init<Car, Unit>(CreateInValidUnit);
            var pipelineResult = (Option<Car, Unit>)pipeline(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Exception);
            Assert.IsTrue(pipelineResult.ExceptionVal.Message == "Invalid Unit creation");
        }

        [TestMethod]
        public void Init_FalslyValidateOnInit_ReturnsValidationOption()
        {

            var pipeline = Pipe.Init<CarWithIInvariant, Unit>();
            var pipelineResult = (Option<CarWithIInvariant, Unit>)pipeline(new CarWithIInvariant(false));

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Validation);
            Assert.IsTrue(pipelineResult.ValidationResult.ErrorMessage == CarWithIInvariant.InValidError);
        }

        [TestMethod]
        public void Init_InitWithPostProcessingCallsPostProcessing() {

            Log log = new Log();
            var postProcesses = new List<IValueAndSupplementExtension>();
            postProcesses.Add(log);

            var pipeline = Pipe.Init<Car, Unit>(postProcesses);

            var pipelineResult = (Option<Car, Unit>)pipeline(new Car());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
            Assert.IsTrue(log.Logs.Count == 1);
        }

		[TestMethod]
		public void Init_WithExplicitExecType()
		{

			Log log = new Log();
			var postProcesses = new List<IValueAndSupplementExtension>();
			postProcesses.Add(log);

			var pipeline = Pipe.Init<Car, Unit>(() => new Unit(), postProcesses, new None<Car, Unit>(null));

			var pipelineResult = (Option<Car, Unit>)pipeline(new Car());

			Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
			Assert.IsInstanceOfType(pipelineResult.Val, typeof(Car));
			Assert.IsTrue(log.Logs.Count == 1);
        }
    }
}
