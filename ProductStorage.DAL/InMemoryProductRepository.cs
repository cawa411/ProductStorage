using System.Collections.Concurrent;
using ProductStorage.DAL.Models;

namespace ProductStorage.DAL;

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _products = new();

    public void Add(Product product)
    {
        if (!_products.TryAdd(product.Id, product))
            throw new InvalidOperationException($"Товар с Id {product.Id} уже существует.");
    }

    public Product? GetById(Guid id) => _products.GetValueOrDefault(id);

    public (List<Product> Items, int TotalCount) Find(ProductFilter filter, int skip, int take)
    {
        var matched = _products.Values
            .Where(p => filter.CreatedDate is null || DateOnly.FromDateTime(p.CreatedAt) == filter.CreatedDate)
            .Where(p => filter.Type is null || p.Type == filter.Type)
            .Where(p => filter.WarehouseNumber is null || p.WarehouseNumber == filter.WarehouseNumber)
            .OrderBy(p => p.CreatedAt)
            .ThenBy(p => p.Id)
            .ToList();

        return (matched.Skip(skip).Take(take).ToList(), matched.Count);
    }
}
