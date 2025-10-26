using FluentMigrator;

namespace Granum.Api.Migrations;

[Migration(001)]
public class InitialCreate : Migration
{
    public override void Up()
    {
        // Users table (TPH - Table Per Hierarchy for Customer and Contractor)
        Create.Table("Users")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("Discriminator").AsString(50).NotNullable(); // 'Customer' or 'Contractor'
    }

    public override void Down()
    {
        Delete.Table("Users");
    }
}
