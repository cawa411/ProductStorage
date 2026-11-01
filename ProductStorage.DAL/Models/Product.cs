namespace ProductStorage.DAL.Models;

public record Product(
    Guid Id,
    string Name,
    decimal Price,
    double Weight,
    ProductType Type,
    DateTime CreatedAt,
    int WarehouseNumber);
