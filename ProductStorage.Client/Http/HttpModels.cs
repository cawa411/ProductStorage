namespace ProductStorage.Client.Http;

public record CreateProductBody(string Name, decimal Price, double Weight, string Type, int WarehouseNumber);

public record CreatedProduct(Guid Id);

public record ProductDto(Guid Id, string Name, decimal Price, double Weight, string Type, DateTime CreatedAt, int WarehouseNumber);

public record ProductsPage(List<ProductDto> Items, int TotalCount, int Page, int PageSize);

public record Problem(string? Title, string? Detail, Dictionary<string, string[]>? Errors);
