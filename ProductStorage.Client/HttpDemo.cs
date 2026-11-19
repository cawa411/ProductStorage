using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using ProductStorage.Client.Http;

namespace ProductStorage.Client;

public class HttpDemo(string address)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task RunAsync()
    {
        Console.WriteLine($"=============== HTTP ({address}) ===============");

        using var http = new HttpClient { BaseAddress = new Uri(address) };

        Console.WriteLine();
        Console.WriteLine("Создаю товары:");
        var ids = new List<Guid>();
        foreach (var body in SampleProducts())
        {
            var response = await http.PostAsJsonAsync("api/products", body, JsonOptions);
            var created = await ReadOrPrintProblem<CreatedProduct>(response);
            if (created is null)
                continue;

            ids.Add(created.Id);
            Console.WriteLine($"  {body.Name} -> {created.Id} (HTTP {(int)response.StatusCode}, Location: {response.Headers.Location})");
        }

        if (ids.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Товар по Id:");
            var product = await ReadOrPrintProblem<ProductDto>(await http.GetAsync($"api/products/{ids[0]}"));
            if (product is not null)
                Print(product);
        }

        Console.WriteLine();
        Console.WriteLine("Товар по несуществующему Id:");
        await ReadOrPrintProblem<ProductDto>(await http.GetAsync($"api/products/{Guid.NewGuid()}"));

        Console.WriteLine();
        Console.WriteLine("Фильтр: вид = Продукты");
        await PrintPage(http, "api/products?type=Food");

        Console.WriteLine();
        Console.WriteLine("Фильтр: склад = 2, по 1 товару на странице, страница 2");
        await PrintPage(http, "api/products?warehouseNumber=2&page=2&pageSize=1");

        var today = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        Console.WriteLine();
        Console.WriteLine($"Фильтр: дата создания = {today}, вид = Продукты, склад = 2");
        await PrintPage(http, $"api/products?createdDate={today}&type=Food&warehouseNumber=2");

        Console.WriteLine();
        Console.WriteLine("Создание товара с ошибками:");
        var invalid = await http.PostAsJsonAsync("api/products", new CreateProductBody("", -10, 0, "Food", 0), JsonOptions);
        await ReadOrPrintProblem<CreatedProduct>(invalid);

        Console.WriteLine();
    }

    private static List<CreateProductBody> SampleProducts() =>
    [
        new("Телевизор", 39990m, 12.5, "Appliances", 1),
        new("Сыр", 320.50m, 0.3, "Food", 2),
        new("Яблоки", 129.90m, 1, "Food", 2),
        new("Пакеты", 99m, 0.2, "General", 3),
    ];

    private static async Task PrintPage(HttpClient http, string url)
    {
        var page = await ReadOrPrintProblem<ProductsPage>(await http.GetAsync(url));
        if (page is null)
            return;

        Console.WriteLine($"  найдено {page.TotalCount}, страница {page.Page}, размер страницы {page.PageSize}");
        foreach (var product in page.Items)
            Print(product);
    }

    private static async Task<T?> ReadOrPrintProblem<T>(HttpResponseMessage response) where T : class
    {
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<T>(JsonOptions);

        var problem = await response.Content.ReadFromJsonAsync<Problem>(JsonOptions);
        Console.WriteLine($"  HTTP {(int)response.StatusCode}: {problem?.Title} {problem?.Detail}".TrimEnd());

        foreach (var (field, messages) in problem?.Errors ?? [])
            foreach (var message in messages)
                Console.WriteLine($"    {field}: {message}");

        return null;
    }

    private static void Print(ProductDto product)
    {
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"  {product.Name} | {product.Price:F2} руб. | {product.Weight} кг | {product.Type} | склад {product.WarehouseNumber} | {product.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC"));
    }
}
