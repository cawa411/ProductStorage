using ProductStorage.DAL.Models;

namespace ProductStorage.BLL.Models;

public record CreateProductCommand(
    string Name,
    decimal Price,
    double Weight,
    ProductType Type,
    int WarehouseNumber);
