using ProductStorage.DAL.Models;

namespace ProductStorage.Api.Http.Contracts;

public record CreateProductRequest(
    string Name,
    decimal Price,
    double Weight,
    ProductType Type,
    int WarehouseNumber);
