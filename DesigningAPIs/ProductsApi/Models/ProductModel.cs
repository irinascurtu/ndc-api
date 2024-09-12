using System.ComponentModel.DataAnnotations;

namespace ProductsApi.Models
{
    public class ProductModel
    {

        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public decimal Price { get; set; }
        //public int Stock { get; set; }
        public string LongDescription { get; set; }
        public int CategoryId { get; set; }

        public decimal FullPrice
        {
            get { return Price * 1.19m; }
        }
    }
}
