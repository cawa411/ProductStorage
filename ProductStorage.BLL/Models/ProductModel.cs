using ProductStorage.DAL.Models;

namespace ProductStorage.BLL.Models;

public record ProductModel(
    Guid Id,
    string Name,
    decimal Price,
    double Weight,
    ProductType Type,
    DateTime CreatedAt,
    int WarehouseNumber);
