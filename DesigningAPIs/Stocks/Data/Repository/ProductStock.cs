using Microsoft.EntityFrameworkCore;

namespace Stocks.Data.Repository
{
    [Keyless]

    public class ProductStock
    {
        public int ProductId { get; set; }
        public int Stock { get; set; }
    }
}
