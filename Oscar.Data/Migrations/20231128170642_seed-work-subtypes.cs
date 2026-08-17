using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class seedworksubtypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "WorksSubType",
                columns: new[] { "Id", "CreationDate", "Description", "LastModified", "ModifiedBy", "Name" },
                values: new object[] { 11, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Drama", null, "SEED", "DR" });

            migrationBuilder.InsertData(
                table: "WorksSubType",
                columns: new[] { "Id", "CreationDate", "Description", "LastModified", "ModifiedBy", "Name" },
                values: new object[] { 12, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Entertainment", null, "SEED", "EN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WorksSubType",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "WorksSubType",
                keyColumn: "Id",
                keyValue: 12);
        }
    }
}
