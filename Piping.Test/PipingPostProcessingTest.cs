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
    public class PipingPostProcessingTest
    {
        [TestMethod]
        public void PostProcessing_Success()
        {
            var postProcessing = new List<IValueAndSupplementExtension>();
            var log = new Log();
            postProcessing.Add(log);

            var carInit = Pipe.Init<Unit, Car>(_ => new Car(), postProcessing)
                    .Then(c => c.Mark = Car.HondaMark);

            var pipelineResult = (Option<Unit, Car>)carInit(new Unit());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Car));
            Assert.IsTrue(pipelineResult.SupplementVal.Mark == Car.HondaMark);

            Assert.IsTrue(log.Logs.Count == 1);
        }

        [TestMethod]
        public void PostProcessing_NoneWhenException()
        {
            void ProduceException() {
                throw new Exception("s");
            }

            var postProcessing = new List<IValueAndSupplementExtension>();
            var log = new Log();
            postProcessing.Add(log);

            var carInit = Pipe.Init<Unit, Car>(_ => new Car(), postProcessing)
                    .Then(c =>
                    {
                        ProduceException();
                        c.Mark = Car.HondaMark;
                        return c;
                    });

            var pipelineResult = (Option<Unit, Car>)carInit(new Unit());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Exception);

            Assert.IsTrue(log.Logs.Count == 0);

        }

        [TestMethod]
        public void PostProcessing_SuccessMultple()
        {
            var postProcessing = new List<IValueAndSupplementExtension>();
            var log = new Log();
            var log2 = new Log();

            postProcessing.Add(log);
            postProcessing.Add(log2);

            var carInit = Pipe.Init<Unit, Car>(_ => new Car(), postProcessing)
                    .Then(c => c.Mark = Car.HondaMark);

            var pipelineResult = (Option<Unit, Car>)carInit(new Unit());

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Car));
            Assert.IsTrue(pipelineResult.SupplementVal.Mark == Car.HondaMark);

            Assert.IsTrue(log.Logs.Count == 1);
            Assert.IsTrue(log2.Logs.Count == 1);
        }

        [TestMethod]
        public void PostProcessing_SuccessOnValAndSupplemendedVal()
        {
            var postProcessing = new List<IValueAndSupplementExtension>();
            var log = new Log();
            var supplementedLog = new Log();

            postProcessing.Add(log);

            var carInit = Pipe.Init<Log, Car>(_ => new Car(), postProcessing)
                   .Then(c => c.Mark = Car.HondaMark)
                   .Then(carAndLog=> carAndLog.SupplementVal.setLogTitle("My title"));

            var pipelineResult = (Option<Log, Car>)carInit(supplementedLog);

            Assert.IsTrue(pipelineResult.GetOptionType == OptionType.Some);
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Car));
            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Log));
            Assert.IsTrue(pipelineResult.SupplementVal.Mark == Car.HondaMark);

            Assert.IsTrue(log.Logs.Count == 4);

            Assert.IsTrue(pipelineResult.Val.LogTitle == "My title");

        }
    }
}