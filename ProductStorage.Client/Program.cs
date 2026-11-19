using Grpc.Core;
using ProductStorage.Client;

const string grpcAddress = "http://localhost:5101";
const string httpAddress = "http://localhost:5100";
const string usage = "Использование: ProductStorage.Client [grpc|http]  (без аргументов — оба)";

var mode = args.Length > 0 ? args[0] : "all";
if (mode is not ("all" or "grpc" or "http") || args.Length > 1)
{
    Console.WriteLine(usage);
    return 1;
}

try
{
    if (mode is "all" or "grpc")
        await new GrpcDemo(grpcAddress).RunAsync();

    if (mode is "all" or "http")
        await new HttpDemo(httpAddress).RunAsync();

    return 0;
}
catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable)
{
    Console.WriteLine($"Не удалось подключиться к gRPC ({grpcAddress}). Сначала запусти ProductStorage.Api.");
    return 1;
}
catch (HttpRequestException)
{
    Console.WriteLine($"Не удалось подключиться к HTTP ({httpAddress}). Сначала запусти ProductStorage.Api.");
    return 1;
}
