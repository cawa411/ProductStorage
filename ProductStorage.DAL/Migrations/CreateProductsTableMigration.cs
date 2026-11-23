using FluentMigrator;

namespace ProductStorage.DAL.Migrations;

[Migration(202611230001)]
public sealed class CreateProductsTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("products")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("price").AsDecimal(18, 2).NotNullable()
            .WithColumn("weight").AsCustom("double precision").NotNullable()
            .WithColumn("type").AsInt32().NotNullable()
            .WithColumn("created_at").AsCustom("timestamp with time zone").NotNullable()
            .WithColumn("warehouse_number").AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("products");
    }
}
