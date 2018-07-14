using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Example.ExampleBusiness
{
    public static class StockService
    {
        public static ProductStockLocation GetNearestProductLocation(Product product, Stock stock)
        {

            var firstProductStock = (from psl in DBContextExt.GetProductStockLocations()
                                     from l in stock.Locations
                                     where psl.Location.Name == l.Name && psl.Amount > 0
                                     orderby l.Distance descending
                                     select psl).FirstOrDefault();

            if (firstProductStock == null)
                return default(ProductStockLocation);

            return firstProductStock;
        }

    }
}
