using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class addInConflictWorksStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "WorksStatus",
                columns: new[] { "Id", "CreationDate", "Description", "LastModified", "ModifiedBy", "Name" },
                values: new object[] { 5, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "InConflict", null, "SEED", "INCONFLICT" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
