using Microsoft.AspNetCore.Mvc;
using ProductStorage.Api.Http.Contracts;
using ProductStorage.BLL;
using ProductStorage.BLL.Models;

namespace ProductStorage.Api.Http.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CreateProductResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<CreateProductResponse> Create(CreateProductRequest request)
    {
        var id = productService.Create(new CreateProductCommand(
            request.Name,
            request.Price,
            request.Weight,
            request.Type,
            request.WarehouseNumber));

        return CreatedAtAction(nameof(GetById), new { id }, new CreateProductResponse(id));
    }

    [HttpGet]
    [ProducesResponseType<PagedResult<ProductModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<PagedResult<ProductModel>> GetList([FromQuery] GetProductsRequest request)
    {
        return productService.GetList(new GetProductsQuery(
            request.CreatedDate,
            request.Type,
            request.WarehouseNumber,
            request.Page,
            request.PageSize));
    }

    [HttpGet("{id}")]
    [ProducesResponseType<ProductModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<ProductModel> GetById(Guid id)
    {
        return productService.GetById(id);
    }
}
