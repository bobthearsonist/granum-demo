using FluentMigrator;

namespace Granum.Api.Migrations;

[Migration(002)]
public class AddServiceLocation : Migration
{
    public override void Up()
    {
        Create.Table("ServiceLocations")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("CustomerId").AsInt32().NotNullable()
            .WithColumn("Name").AsString(255).NotNullable()
            .WithColumn("Address").AsString(500).NotNullable();

        Create.ForeignKey("FK_ServiceLocations_Users_CustomerId")
            .FromTable("ServiceLocations").ForeignColumn("CustomerId")
            .ToTable("Users").PrimaryColumn("Id");
    }

    public override void Down()
    {
        Delete.ForeignKey("FK_ServiceLocations_Users_CustomerId").OnTable("ServiceLocations");
        Delete.Table("ServiceLocations");
    }
}
