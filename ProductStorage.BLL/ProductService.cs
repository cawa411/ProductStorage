using FluentValidation;
using ProductStorage.BLL.Exceptions;
using ProductStorage.BLL.Models;
using ProductStorage.DAL;
using ProductStorage.DAL.Models;

namespace ProductStorage.BLL;

public class ProductService(
    IProductRepository repository,
    IValidator<CreateProductCommand> createValidator,
    IValidator<GetProductsQuery> listValidator) : IProductService
{
    public Guid Create(CreateProductCommand command)
    {
        createValidator.ValidateAndThrow(command);

        var product = new Product(
            Guid.NewGuid(),
            command.Name.Trim(),
            command.Price,
            command.Weight,
            command.Type,
            DateTime.UtcNow,
            command.WarehouseNumber);

        repository.Add(product);
        return product.Id;
    }

    public ProductModel GetById(Guid id)
    {
        var product = repository.GetById(id) ?? throw new ProductNotFoundException(id);
        return ToModel(product);
    }

    public PagedResult<ProductModel> GetList(GetProductsQuery query)
    {
        listValidator.ValidateAndThrow(query);

        var filter = new ProductFilter(query.CreatedDate, query.Type, query.WarehouseNumber);
        var (items, totalCount) = repository.Find(filter, (query.Page - 1) * query.PageSize, query.PageSize);

        return new PagedResult<ProductModel>(items.Select(ToModel).ToList(), totalCount, query.Page, query.PageSize);
    }

    private static ProductModel ToModel(Product product) => new(
        product.Id,
        product.Name,
        product.Price,
        product.Weight,
        product.Type,
        product.CreatedAt,
        product.WarehouseNumber);
}
