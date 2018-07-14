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
    public class PipingNestingTest
    {
        [TestMethod]
        public void Nest_TwoPipings()
        {

            var engineInit = Pipe.Init<Engine, Unit>(_ => new Unit())
                   .Then(e => e.Fuel = Engine.EFuelType.Gasoline);

            var enginePowerInit = Pipe.Init<Engine, Unit>(_ => new Unit())
                   .Then(e => e.HorsePower = 145);

            var combinedEngine = engineInit.Then(i => enginePowerInit(i));

            var pipelineResult = (Option<Engine, Unit>)combinedEngine(new Engine());

            Assert.IsInstanceOfType(pipelineResult.Val, typeof(Engine));
            Assert.IsInstanceOfType(pipelineResult.SupplementVal, typeof(Unit));
            Assert.IsTrue(pipelineResult.Val.Fuel== Engine.EFuelType.Gasoline);
            Assert.IsTrue(pipelineResult.Val.HorsePower == 145);

        }

       
    }
}
