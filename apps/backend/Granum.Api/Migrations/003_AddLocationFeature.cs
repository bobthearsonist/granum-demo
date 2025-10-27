using FluentMigrator;

namespace Granum.Api.Migrations;

[Migration(003)]
public class AddLocationFeature : Migration
{
    public override void Up()
    {
        Create.Table("LocationFeatures")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("LocationId").AsInt32().NotNullable()
            .WithColumn("FeatureType").AsString(50).NotNullable()
            .WithColumn("Measurement").AsFloat().NotNullable()
            .WithColumn("Unit").AsString(50).NotNullable()
            .WithColumn("Description").AsString(500).Nullable();

        Create.ForeignKey("FK_LocationFeatures_ServiceLocations_LocationId")
            .FromTable("LocationFeatures").ForeignColumn("LocationId")
            .ToTable("ServiceLocations").PrimaryColumn("Id");
    }

    public override void Down()
    {
        Delete.ForeignKey("FK_LocationFeatures_ServiceLocations_LocationId").OnTable("LocationFeatures");
        Delete.Table("LocationFeatures");
    }
}
