using System.Globalization;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ProductStorage.BLL;
using ProductStorage.BLL.Models;
using DomainProductType = ProductStorage.DAL.Models.ProductType;

namespace ProductStorage.Api.Grpc;

public class ProductsGrpcService(IProductService productService) : Products.ProductsBase
{
    private const int DefaultPageSize = 20;

    public override Task<CreateProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
    {
        var id = productService.Create(new CreateProductCommand(
            request.Name,
            (decimal)request.Price,
            request.Weight,
            (DomainProductType)request.Type,
            request.WarehouseNumber));

        return Task.FromResult(new CreateProductResponse { Id = id.ToString() });
    }

    public override Task<GetProductsResponse> GetProducts(GetProductsRequest request, ServerCallContext context)
    {
        var result = productService.GetList(new GetProductsQuery(
            request.HasCreatedDate ? ParseDate(request.CreatedDate) : null,
            request.HasType ? (DomainProductType)request.Type : null,
            request.HasWarehouseNumber ? request.WarehouseNumber : null,
            request.Page == 0 ? 1 : request.Page,
            request.PageSize == 0 ? DefaultPageSize : request.PageSize));

        var response = new GetProductsResponse
        {
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
        };
        response.Items.AddRange(result.Items.Select(ToReply));

        return Task.FromResult(response);
    }

    public override Task<ProductReply> GetProductById(GetProductByIdRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"Некорректный Id: \"{request.Id}\"."));

        return Task.FromResult(ToReply(productService.GetById(id)));
    }

    private static DateOnly ParseDate(string value)
    {
        if (!DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"Некорректная дата: \"{value}\", ожидается yyyy-MM-dd."));

        return date;
    }

    private static ProductReply ToReply(ProductModel product) => new()
    {
        Id = product.Id.ToString(),
        Name = product.Name,
        Price = (double)product.Price,
        Weight = product.Weight,
        Type = (ProductType)product.Type,
        CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(product.CreatedAt, DateTimeKind.Utc)),
        WarehouseNumber = product.WarehouseNumber,
    };
}
