using ProductStorage.DAL.Models;

namespace ProductStorage.DAL;

public interface IProductRepository
{
    void Add(Product product);

    Product? GetById(Guid id);

    (List<Product> Items, int TotalCount) Find(ProductFilter filter, int skip, int take);
}
