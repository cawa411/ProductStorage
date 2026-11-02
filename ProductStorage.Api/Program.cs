using System.Globalization;
using System.Text.Json.Serialization;
using FluentValidation;
using ProductStorage.Api.Grpc;
using ProductStorage.Api.Grpc.Interceptors;
using ProductStorage.Api.Http.Middleware;
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

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGrpcService<ProductsGrpcService>();
app.MapGrpcReflectionService();

app.Run();
