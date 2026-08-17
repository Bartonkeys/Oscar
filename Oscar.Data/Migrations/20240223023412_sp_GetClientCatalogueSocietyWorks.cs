using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class sp_GetClientCatalogueSocietyWorks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RegistrationConfiguration",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegistrationDate",
                value: new DateTime(2024, 1, 31, 23, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql(SqlResources.sp_GetClientCatalogueSocietyWorks);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RegistrationConfiguration",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegistrationDate",
                value: new DateTime(2024, 1, 31, 22, 30, 50, 704, DateTimeKind.Local).AddTicks(9071));

            migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_GetClientCatalogueSocietyWorks]");
        }
    }
}
