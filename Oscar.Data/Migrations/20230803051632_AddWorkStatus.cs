using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class AddWorkStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "WorksStatus",
                columns: new[] { "Id", "CreationDate", "Description", "LastModified", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { -1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Any", null, "SEED", "Any" },
                    { 1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Active", null, "SEED", "Active" },
                    { 2, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Uncontrolled", null, "SEED", "Uncontrolled" },
                    { 3, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Incomplete", null, "SEED", "Incomplete" },
                    { 4, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Relinquished", null, "SEED", "Relinquished" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
