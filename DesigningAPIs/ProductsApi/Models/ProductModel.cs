﻿using System.ComponentModel.DataAnnotations;

namespace ProductsApi.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string LongDescription { get; set; }
        public int CategoryId { get; set; }
    }
}
