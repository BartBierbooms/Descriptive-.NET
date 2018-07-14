using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Test.TestData
{
    public class Dealer
    {
        public const Decimal DISCOUNT = 1000M; 
        public enum eReputation {
            Good,
            Bad
        }

        public string Name { get; set; }
        public eReputation Reputation { get; set; }

        public void GiveDiscount(Car car) {
            car.Price = car.Price - DISCOUNT;
        }
    }
}
