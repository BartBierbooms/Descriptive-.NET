using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Example.ExampleBusiness
{
    public static class PriceService
    {
        public const decimal ForeignCountryFee = 12.0M;

        public static decimal AddTransportPriceForForeignAddress(Address stockLocation, Address customer) {

            if (stockLocation.Country != customer.Country)
                return ForeignCountryFee;

            return 0.0M;
        }
    }
}
