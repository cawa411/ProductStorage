namespace ProductStorage.DAL.Models;

public record ProductFilter(DateOnly? CreatedDate, ProductType? Type, int? WarehouseNumber);
