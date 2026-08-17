using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class other_names_changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OtherName_Clients_ClientId",
                table: "OtherName");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "OtherName",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherName_Clients_ClientId",
                table: "OtherName",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OtherName_Clients_ClientId",
                table: "OtherName");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "OtherName",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OtherName_Clients_ClientId",
                table: "OtherName",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
