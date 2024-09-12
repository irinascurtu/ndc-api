﻿using ProductsApi.Data.Entities;
using ProductsApi.Data.Extensions;

namespace ProductsApi.Data.Repositories
{
    public interface IProductRepository
    {
        Task<Product> AddProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<Product> GetProductAsync(int id);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<IQueryable<Product>> GetProductsAsync(int? category);
        Task<IEnumerable<ProductStock>> GetProductStocksAsync(List<int> productIds);
        Task<bool> ProductExistsAsync(int id);
        Task<Product> UpdateProductAsync(Product product);
    }
}