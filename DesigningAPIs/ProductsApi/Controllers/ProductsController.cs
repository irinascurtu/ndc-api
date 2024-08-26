using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.Data.Entities;
using ProductsApi.Models;
using ProductsApi.Service;

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductsController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetProducts(int? categoryId, int? page)
        {
            var products = await _productService.GetProductsAsync(categoryId, page);
            var list = products.ToList();
            return Ok(list);
            // TODO: add mappings and use the Model if needed
        }


        // GET: api/Products/{id}
        [HttpGet("{id}")]
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

    }
}
