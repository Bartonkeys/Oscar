using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class AddCrtcCodeField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE dbo.Works SET (SYSTEM_VERSIONING = OFF);");

            migrationBuilder.DropColumn(
                name: "CavcoCtcCode",
                table: "WorksImport");

            migrationBuilder.DropColumn(
                name: "CavcoCtcCode",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "CavcoCtcCode",
                table: "WorksHistory");

            migrationBuilder.AddColumn<string>(
                name: "CavcoCode",
                table: "WorksImport",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrtcCode",
                table: "WorksImport",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CavcoCode",
                table: "Works",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrtcCode",
                table: "Works",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CavcoCode",
                table: "WorksHistory",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrtcCode",
                table: "WorksHistory",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.Sql("ALTER TABLE dbo.Works SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.WorksHistory));");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE dbo.Works SET (SYSTEM_VERSIONING = OFF);");

            migrationBuilder.DropColumn(
                name: "CavcoCode",
                table: "WorksImport");

            migrationBuilder.DropColumn(
                name: "CrtcCode",
                table: "WorksImport");

            migrationBuilder.DropColumn(
                name: "CavcoCode",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "CrtcCode",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "CavcoCode",
                table: "WorksHistory");

            migrationBuilder.DropColumn(
                name: "CrtcCode",
                table: "WorksHistory");

            migrationBuilder.AddColumn<string>(
                name: "CavcoCtcCode",
                table: "WorksImport",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CavcoCtcCode",
                table: "Works",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CavcoCtcCode",
                table: "WorksHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql("ALTER TABLE dbo.Works SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.WorksHistory));");
        }
    }
}
