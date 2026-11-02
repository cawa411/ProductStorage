using ProductStorage.DAL.Models;

namespace ProductStorage.BLL.Models;

public record GetProductsQuery(
    DateOnly? CreatedDate,
    ProductType? Type,
    int? WarehouseNumber,
    int Page,
    int PageSize);
