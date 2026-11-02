using ProductStorage.BLL.Models;

namespace ProductStorage.BLL;

public interface IProductService
{
    Guid Create(CreateProductCommand command);

    ProductModel GetById(Guid id);

    PagedResult<ProductModel> GetList(GetProductsQuery query);
}
