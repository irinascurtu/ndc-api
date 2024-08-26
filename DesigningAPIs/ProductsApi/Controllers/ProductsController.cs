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
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products);
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

            //ADD the check for ID
            //test with bad data
            if (!ModelState.IsValid ) {
                return BadRequest(ModelState);
            }

            var productToUpdate = _mapper.Map<Product>(modelToUpdate);
            // PRoductModel->Product
            // Update the Product
            var updatedProduct = await _productService.UpdateProductAsync(productToUpdate);

            return Ok(updatedProduct);
        }
    }
}
