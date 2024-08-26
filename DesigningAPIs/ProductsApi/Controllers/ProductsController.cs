using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using ProductsApi.Data.Entities;
using ProductsApi.Models;
using ProductsApi.Service;
using System.Text.Json;

namespace ProductsApi.Controllers
{
    //[Route("api/[controller]")]
    //[Route("api/products")]
    [Route("api/v{version:apiVersion}/products")]
    [Asp.Versioning.ApiVersion("1")]
    [Asp.Versioning.AdvertiseApiVersions("1")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        private readonly IMemoryCache memoryCache;
        private readonly ILogger<ProductsController> logger;
        private const string LimitedStockProductsKey = "LSPC";

        private readonly IDistributedCache distributedCache;
        private const string OverstockedProductsKey = "OSPK";

        public ProductsController(IProductService productService,
            IMapper mapper,
            ILogger<ProductsController> logger,
            IMemoryCache memoryCache,
            IDistributedCache distributedCache
            )
        {
            _productService = productService;
            _mapper = mapper;
            this.logger = logger;
            this.memoryCache = memoryCache;
            this.distributedCache = distributedCache;
        }


        [HttpGet]
        [Produces("application/vnd.example.v1+json")]
        public async Task<IActionResult> GetProducts(int? categoryId, int? page)
        {
            var products = await _productService.GetProductsAsync();
            // var products = await _productService.GetProductsAsync(categoryId, page);
            var list = products.ToList();
            return Ok(list);
            // TODO: add mappings and use the Model if needed
        }


        // GET: api/Products/{id}
        [HttpGet("{id}")]
        [ResponseCache(Duration = 12, // Cache-Control: max-age=5
          Location = ResponseCacheLocation.Any, // Cache-Control: public
          VaryByHeader = "User-Agent" // Vary: User-Agent
          )]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/{id}
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutProduct(int id, Product product)
        //{
        //    if (id != product.Id)
        //    {
        //        return BadRequest();
        //    }

        //    try
        //    {
        //        await _productService.UpdateProductAsync(product);
        //    }
        //    catch
        //    {
        //        if (!await _productService.ProductExistsAsync(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductModel modelToUpdate)
        {

            if (id != modelToUpdate.Id)
            {
                return BadRequest();
            }


            //ADD the check for ID
            //this is redundant the platform does the checks
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productToUpdate = _mapper.Map<Product>(modelToUpdate);
            // PRoductModel->Product
            // Update the Product
            var updatedProduct = await _productService.UpdateProductAsync(productToUpdate);

            return Ok(updatedProduct);
        }


        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(ProductModel productModel)
        {

            var productToAdd = _mapper.Map<Product>(productModel);

            var createdProduct = await _productService.AddProductAsync(productToAdd);

            return CreatedAtAction("GetProduct", new { id = createdProduct.Id }, createdProduct);
        }



        // DELETE: api/Products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productService.DeleteProductAsync(id);
            return NoContent();
        }

        [HttpHead("{id}")]
        public async Task<IActionResult> CheckIfExists(int id)
        {

            var exists = await _productService.ProductExistsAsync(id);

            HttpContext.Response.Headers.Add("Exists", exists.ToString());

            if (exists == false)
            {
                return NotFound();
            }

            return Ok(exists);
        }


        // GET: api/products/limitedstock
        [HttpGet]
        [Route("limitedstock")]
        [Produces(typeof(Product[]))]
        public async Task<IEnumerable<Product>> GetLimitedStockProducts()
        {
            // Try to get the cached value.
            if (!memoryCache.TryGetValue(LimitedStockProductsKey, out Product[]? cachedValue))
            {


                // If the cached value is not found, get the value from the database.
                var products = await _productService.GetProductsAsync();
                cachedValue = products.Where(p => p.Stock <= 30)
                  .ToArray();


                MemoryCacheEntryOptions cacheEntryOptions = new()
                { //AbsoluteExpiration = DateTimeOffset.UtcNow,
                    SlidingExpiration = TimeSpan.FromSeconds(12),
                    Size = cachedValue?.Length
                };

                memoryCache.Set(LimitedStockProductsKey, cachedValue, cacheEntryOptions);
            }
            MemoryCacheStatistics? stats = memoryCache.GetCurrentStatistics();
            logger.LogInformation($"Memory cache. Total hits: {stats?
                  .TotalHits}. Estimated size: {stats?.CurrentEstimatedSize}.");

            return cachedValue ?? Enumerable.Empty<Product>();
        }

        // GET: api/products/overstocked
        [HttpGet]
        [Route("overstocked")]
        [Produces(typeof(Product[]))]
        public async Task<IEnumerable<Product>> GetOverStockedProducts()
        {
            // Try to get the cached value.
            byte[]? cachedValueBytes = distributedCache.Get(OverstockedProductsKey);
            Product[]? cachedValue = null;
            if (cachedValueBytes is null)
            {
                cachedValue = GetOverStockedProductsFromDb();
            }
            else
            {
                cachedValue = JsonSerializer
                  .Deserialize<Product[]?>(cachedValueBytes);
                if (cachedValue is null)
                {
                    cachedValue = GetOverStockedProductsFromDb();
                }
            }
            return cachedValue ?? Enumerable.Empty<Product>();
        }

        private Product[]? GetOverStockedProductsFromDb()
        {
            // If the cached value is not found, get the value from the database.
            var products = _productService.GetProductsAsync().Result;
            var cachedValue = products.Where(p => p.Stock > 100)
                .ToArray();


            DistributedCacheEntryOptions cacheEntryOptions = new()
            {
                // Allow readers to reset the cache entry's lifetime.
                SlidingExpiration = TimeSpan.FromSeconds(5),

                // Set an absolute expiration time for the cache entry.
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20)

            };

            byte[]? cachedValueBytes =
              JsonSerializer.SerializeToUtf8Bytes(cachedValue);

            distributedCache.Set(OverstockedProductsKey, cachedValueBytes, cacheEntryOptions);

            return cachedValue;
        }

    }
}
