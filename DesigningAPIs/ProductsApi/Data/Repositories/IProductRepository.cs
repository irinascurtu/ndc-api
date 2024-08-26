﻿using ProductsApi.Data.Entities;

namespace ProductsApi.Data.Repositories
{
    public interface IProductRepository
    {
        Task<Product> AddProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<Product> GetProductAsync(int id);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<IQueryable<Product>> GetProductsAsync(int? categoryId, int? page);
        Task<bool> ProductExistsAsync(int id);
        Task<Product> UpdateProductAsync(Product product);
    }
}