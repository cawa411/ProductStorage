using System.Globalization;
using Grpc.Core;
using Grpc.Net.Client;
using ProductStorage.Api.Grpc;

namespace ProductStorage.Client;

public class GrpcDemo(string address)
{
    public async Task RunAsync()
    {
        Console.WriteLine($"=============== gRPC ({address}) ===============");

        using var channel = GrpcChannel.ForAddress(address);
        var client = new Products.ProductsClient(channel);

        Console.WriteLine();
        Console.WriteLine("Создаю товары:");
        var ids = new List<string>();
        foreach (var request in SampleProducts())
        {
            var response = await client.CreateProductAsync(request);
            ids.Add(response.Id);
            Console.WriteLine($"  {request.Name} -> {response.Id}");
        }

        Console.WriteLine();
        Console.WriteLine("Товар по Id:");
        Print(await client.GetProductByIdAsync(new GetProductByIdRequest { Id = ids[0] }));

        Console.WriteLine();
        Console.WriteLine("Товар по несуществующему Id:");
        await ShowError(() => client.GetProductByIdAsync(new GetProductByIdRequest { Id = Guid.NewGuid().ToString() }).ResponseAsync);

        Console.WriteLine();
        Console.WriteLine("Фильтр: вид = Продукты");
        PrintPage(await client.GetProductsAsync(new GetProductsRequest { Type = ProductType.Food }));

        Console.WriteLine();
        Console.WriteLine("Фильтр: склад = 2, по 1 товару на странице, страница 2");
        PrintPage(await client.GetProductsAsync(new GetProductsRequest { WarehouseNumber = 2, Page = 2, PageSize = 1 }));

        var today = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        Console.WriteLine();
        Console.WriteLine($"Фильтр: дата создания = {today}, вид = Продукты, склад = 2");
        PrintPage(await client.GetProductsAsync(new GetProductsRequest
        {
            CreatedDate = today,
            Type = ProductType.Food,
            WarehouseNumber = 2,
        }));

        Console.WriteLine();
        Console.WriteLine("Создание товара с ошибками:");
        await ShowError(() => client.CreateProductAsync(new CreateProductRequest
        {
            Name = "",
            Price = -10,
            Weight = 0,
            Type = ProductType.Food,
            WarehouseNumber = 0,
        }).ResponseAsync);

        Console.WriteLine();
    }

    private static List<CreateProductRequest> SampleProducts() =>
    [
        new() { Name = "Чайник", Price = 2499.90, Weight = 1.2, Type = ProductType.Appliances, WarehouseNumber = 1 },
        new() { Name = "Хлеб", Price = 45, Weight = 0.4, Type = ProductType.Food, WarehouseNumber = 2 },
        new() { Name = "Молоко", Price = 89.50, Weight = 1, Type = ProductType.Food, WarehouseNumber = 2 },
        new() { Name = "Стиральный порошок", Price = 399, Weight = 3, Type = ProductType.HouseholdChemicals, WarehouseNumber = 3 },
    ];

    private static async Task ShowError(Func<Task> call)
    {
        try
        {
            await call();
            Console.WriteLine("  ожидалась ошибка, но запрос прошёл");
        }
        catch (RpcException e) when (e.StatusCode != StatusCode.Unavailable)
        {
            Console.WriteLine($"  {e.StatusCode}: {e.Status.Detail}");
        }
    }

    private static void PrintPage(GetProductsResponse page)
    {
        Console.WriteLine($"  найдено {page.TotalCount}, страница {page.Page}, размер страницы {page.PageSize}");
        foreach (var product in page.Items)
            Print(product);
    }

    private static void Print(ProductReply product)
    {
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"  {product.Name} | {product.Price:F2} руб. | {product.Weight} кг | {product.Type} | склад {product.WarehouseNumber} | {product.CreatedAt.ToDateTime():yyyy-MM-dd HH:mm:ss} UTC"));
    }
}
