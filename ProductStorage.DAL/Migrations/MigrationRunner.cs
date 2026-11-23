using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace ProductStorage.DAL.Migrations;

public static class MigrationRunner
{
    public static void RunMigrations(string connectionString)
    {
        using var services = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(CreateProductsTableMigration).Assembly).For.Migrations())
            .AddLogging(logging => logging.AddFluentMigratorConsole())
            .BuildServiceProvider();

        using var scope = services.CreateScope();
        scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
    }
}
