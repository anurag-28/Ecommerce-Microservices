using Catalog.Application.Commands;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Specs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.API.Controllers
{
    public class CatalogController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CatalogController> _logger;
        public CatalogController(IMediator mediator, ILogger<CatalogController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]/{id}", Name = "GetProductById")]
        [ProducesResponseType(typeof(ProductResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProductResponse>> GetProductById (string id)
        {
            _logger.LogInformation("Fetching product details for ProductId: {ProductId}", id);
            var query = new GetProductByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result == null)
            {
                _logger.LogWarning("Product not found for ProductId: {ProductId}", id);
                return NotFound();
            }

            _logger.LogInformation("Product retrieved successfully for ProductId: {ProductId}", id);
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]/{productName}", Name = "GetProductsByProductName")]
        [ProducesResponseType(typeof(IList<ProductResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<ProductResponse>>> GetProductsByProductName(string productName)
        {
            _logger.LogInformation("Fetching products with ProductName containing: {ProductName}", productName);
            var query = new GetProductByNameQuery(productName);
            var result = await _mediator.Send(query);
            if (result == null || !result.Any())
            {
                _logger.LogWarning("No products found for ProductName: {ProductName}", productName);
                return NotFound();
            }
            _logger.LogInformation($"Product with {productName} fetched");
            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllProducts")]
        [ProducesResponseType(typeof(Pagination<ProductResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Pagination<ProductResponse>>> GetAllProducts([FromQuery] CatalogSpecParams catalogSpecParams)
        {
            _logger.LogInformation("Fetching all products with pagination: PageIndex={PageIndex}, PageSize={PageSize}",
                catalogSpecParams.PageIndex, catalogSpecParams.PageSize);
            var query = new GetAllProductsQuery(catalogSpecParams);
            var result = await _mediator.Send(query);
            if (result == null || result.Data == null || !result.Data.Any())
            {
                _logger.LogWarning("No products found in catalog for current filters.");
                return NotFound();
            }

            _logger.LogInformation("Retrieved {Count} products successfully (PageIndex={PageIndex})", result.Count, catalogSpecParams.PageIndex);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllBrands")]
        [ProducesResponseType(typeof(IList<BrandResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<BrandResponse>>> GetAllBrands()
        {
            _logger.LogInformation("Fetching all brands.");
            var query = new GetAllBrandsQuery();
            var result = await _mediator.Send(query);

            if (result == null || !result.Any())
            {
                _logger.LogWarning("No brands found in catalog.");
                return NotFound();
            }
            _logger.LogInformation("{Count} brands fetched successfully.", result.Count);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllTypes")]
        [ProducesResponseType(typeof(IList<TypesResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<TypesResponse>>> GetAllTypes()
        {
            _logger.LogInformation("Fetching all product types.");
            var query = new GetAllTypesQuery();
            var result = await _mediator.Send(query);
            if (result == null || !result.Any())
            {
                _logger.LogWarning("No product types found in catalog.");
                return NotFound();
            }
            _logger.LogInformation("{Count} product types fetched successfully.", result.Count);
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]/{brandName}", Name = "GetProductsByBrandName")]
        [ProducesResponseType(typeof(IList<ProductResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<ProductResponse>>> GetProductsByBrandName(string brandName)
        {
            _logger.LogInformation("Fetching products for Brand: {BrandName}", brandName);

            var query = new GetProductByBrandQuery(brandName);
            var result = await _mediator.Send(query);

            if (result == null || !result.Any())
            {
                _logger.LogWarning("No products found for Brand: {BrandName}", brandName);
                return NotFound();
            }

            _logger.LogInformation("{Count} products retrieved successfully for Brand: {BrandName}", result.Count, brandName);
            return Ok(result);
        }

        [HttpPost]
        [Route("CreateProduct")]
        [ProducesResponseType(typeof(ProductResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] CreateProductCommand command)
        {
            _logger.LogInformation("Creating a new product with Name: {ProductName}, BrandId: {BrandId}, TypeId: {TypeId}",
                command.Name, command.Brands, command.Types);

            var result = await _mediator.Send(command);

            _logger.LogInformation("Product created successfully with Id: {ProductId}", result?.Id);
            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateProduct")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> UpdateProduct([FromBody] UpdateProductCommand command)
        {
            _logger.LogInformation("Updating product with Id: {ProductId}", command.Id);

            var result = await _mediator.Send(command);

            if (result)
                _logger.LogInformation("Product updated successfully with Id: {ProductId}", command.Id);
            else
                _logger.LogWarning("Failed to update product. ProductId: {ProductId} not found.", command.Id);

            return Ok(result);
        }

        [HttpDelete]
        [Route( "{id}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteProduct(string id)
        {
            _logger.LogInformation("Deleting product with Id: {ProductId}", id);

            var command = new DeleteProductCommand(id);
            var result = await _mediator.Send(command);

            if (result)
                _logger.LogInformation("Product deleted successfully with Id: {ProductId}", id);
            else
                _logger.LogWarning("Failed to delete product. ProductId: {ProductId} not found.", id);

            return Ok(result);
        }
    }
}
