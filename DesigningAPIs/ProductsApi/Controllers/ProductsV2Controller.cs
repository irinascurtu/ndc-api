using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProductsApi.Controllers
{
    //[Route("api/[controller]")]
    //[Route("api/products")]
    //[Route("api/v{version:apiVersion}/products")]
    [Route("api/products")]
    [Asp.Versioning.ApiVersion("2")]
    [ApiController]
    public class ProductsV2Controller : ControllerBase
    {
        public ProductsV2Controller()
        {

        }

        [HttpGet]
        [Produces("application/vnd.example.v1+json")] 
        public IActionResult GetProducts()
        {
            return Ok("Products V2");
        }
    }
}
