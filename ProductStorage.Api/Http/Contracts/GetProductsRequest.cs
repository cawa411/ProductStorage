using ProductStorage.DAL.Models;

namespace ProductStorage.Api.Http.Contracts;

public class GetProductsRequest
{
    public DateOnly? CreatedDate { get; init; }

    public ProductType? Type { get; init; }

    public int? WarehouseNumber { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}
