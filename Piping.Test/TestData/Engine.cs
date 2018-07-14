using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Test.TestData
{
    public class Engine
    {
        public enum EFuelType
        {
            Diesel,
            Gasoline
        }

        public EFuelType Fuel { get; set; }
        public int HorsePower { get; set; }

        public Engine() { }
        public Engine(EFuelType Fuel, int horsePower)
        {
            this.Fuel = Fuel;
            HorsePower = horsePower;
        }
    }
}
