using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class SeedDuplicateWorksStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "WorksStatus",
                columns: new[] { "Id", "CreationDate", "Description", "LastModified", "ModifiedBy", "Name" },
                values: new object[] { 6, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Duplicate", null, "SEED", "DUPLICATE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
