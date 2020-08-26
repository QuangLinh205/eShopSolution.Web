using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopSolution.Application.Catalogs.Products;
using eShopSolution.ViewModel.Catalog.ProductImages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IPublicProductService _publicProductService;
        private readonly IManagerProductService _managerProductService;

        public ProductsController(IPublicProductService publicProductService, IManagerProductService managerProductService)
        {
            _publicProductService = publicProductService;
            _managerProductService = managerProductService;
        }

        [HttpGet("{languageId}")] //http://5001/api/product/public-pagging
        public async Task<IActionResult> GetAllPagging(string languageId, [FromQuery] GetPublicProductPaginhRequest request) //[FromQuery] tất cả tham số đều lấy từ query ra
        {
            var products = await _publicProductService.GetAllByCategoryId(languageId, request);
            return Ok(products);
        }

        [HttpGet("{productId}/{languageId}")] //http://5001/api/product/1
        public async Task<IActionResult> GetByProductIdVSlanguageId(int productId, string languageId)
        {
            var product = await _managerProductService.GetById(productId, languageId);
            if (product == null)
                return BadRequest("khong tim thay id");
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productId = await _managerProductService.Create(request);
            if (productId == 0)
                return BadRequest();

            var product = await _managerProductService.GetById(productId, request.LanguageId);

            return CreatedAtAction(nameof(GetByProductIdVSlanguageId), new { id = productId }, product);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ProductUpdateRequest request)
        {
            var result = await _managerProductService.Update(request);
            if (result == 0)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var result = await _managerProductService.Delete(productId);
            if (result == 0)
                return BadRequest();
            return Ok();
        }

        [HttpPatch("price/{ProductId}/{newPrice}")]
        public async Task<IActionResult> UpdatePrice(int ProductId, decimal newPrice)
        {
            var updatePrice = await _managerProductService.UpdatePrice(ProductId, newPrice);
            if (updatePrice == true)
                return Ok();
            return BadRequest("nônnonono");
        }

        [HttpPatch("stoke/{ProductId}/{addedQuantity}")]
        public async Task<IActionResult> UpdateStoke(int ProductId, int addedQuantity)
        {
            var updateStoke = await _managerProductService.UpdateStoke(ProductId, addedQuantity);
            if (updateStoke)
                return Ok();
            return BadRequest();
        }

        [HttpPatch("addViewCount/{productId}")]
        public async Task AddViewCount(int productId)
        {
            await _managerProductService.AddViewCount(productId);
        }

        // images
        [HttpGet("{productId}/images/{imageId}")]
        public async Task<IActionResult> GetByProductIdVSimageId(int productId, int imageId)
        {
            var image = await _managerProductService.GetImageById(imageId);
            if (image == null)
                return BadRequest("khong tim thay id");
            return Ok(image);
        }

        [HttpPost("{productId}/image")]
        public async Task<IActionResult> CreateImage(int productId, [FromForm] ProductImageCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var imageId = await _managerProductService.AddImage(productId, request); // hàm AddImage trả về return productImage.Id;
            if (imageId == 0)
                return BadRequest();
            var getImage = await _managerProductService.GetImageById(imageId);
            return CreatedAtAction(nameof(GetByProductIdVSimageId), new { id = imageId }, imageId);
        }

        [HttpPut("image/{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromForm] ProductImageUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _managerProductService.UpdateImage(imageId, request); // hàm AddImage trả về return productImage.Id;
            if (result == 0)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("image/{imageId}")]
        public async Task<IActionResult> RemoveImage(int imageId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _managerProductService.RemoveImage(imageId);
            if (result == 0)
                return BadRequest();
            return Ok();
        }
    }
}