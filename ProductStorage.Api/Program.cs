using System.Globalization;
using FluentValidation;
using ProductStorage.Api.Grpc;
using ProductStorage.Api.Grpc.Interceptors;
using ProductStorage.BLL;
using ProductStorage.BLL.Validators;
using ProductStorage.DAL;

ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("ru");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>(ServiceLifetime.Singleton);

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<LoggingInterceptor>();
    options.Interceptors.Add<ErrorHandlingInterceptor>();
});
builder.Services.AddGrpcReflection();

var app = builder.Build();

app.MapGrpcService<ProductsGrpcService>();
app.MapGrpcReflectionService();

app.Run();
