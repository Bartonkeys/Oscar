using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Oscar.Mrit.Migrations
{
    public partial class AddProdIdAndRecordId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductionId",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecordId",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2021, 6, 10, 13, 45, 26, 975, DateTimeKind.Local).AddTicks(7263));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2021, 6, 10, 13, 45, 26, 977, DateTimeKind.Local).AddTicks(5148));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreateDate",
                value: new DateTime(2021, 6, 10, 13, 45, 26, 977, DateTimeKind.Local).AddTicks(5181));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreateDate",
                value: new DateTime(2021, 6, 10, 13, 45, 26, 977, DateTimeKind.Local).AddTicks(5185));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreateDate",
                value: new DateTime(2021, 6, 10, 13, 45, 26, 977, DateTimeKind.Local).AddTicks(5188));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductionId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "RecordId",
                table: "Matches");

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2021, 6, 10, 13, 32, 21, 255, DateTimeKind.Local).AddTicks(7972));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2021, 6, 10, 13, 32, 21, 257, DateTimeKind.Local).AddTicks(3860));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreateDate",
                value: new DateTime(2021, 6, 10, 13, 32, 21, 257, DateTimeKind.Local).AddTicks(3898));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreateDate",
                value: new DateTime(2021, 6, 10, 13, 32, 21, 257, DateTimeKind.Local).AddTicks(3903));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreateDate",
                value: new DateTime(2021, 6, 10, 13, 32, 21, 257, DateTimeKind.Local).AddTicks(3905));
        }
    }
}
