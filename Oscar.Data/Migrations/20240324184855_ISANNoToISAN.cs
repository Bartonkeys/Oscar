using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class ISANNoToISAN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "OnMusicMatches",
                type: "varchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            //migrationBuilder.InsertData(
            //    table: "OnMusicMatchStatuses",
            //    columns: new[] { "Id", "Name" },
            //    values: new object[] { 1, "Success" });

            //migrationBuilder.InsertData(
            //    table: "OnMusicMatchStatuses",
            //    columns: new[] { "Id", "Name" },
            //    values: new object[] { 2, "Error" });

            //migrationBuilder.InsertData(
            //    table: "OnMusicMatchStatuses",
            //    columns: new[] { "Id", "Name" },
            //    values: new object[] { 3, "Duplicate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OnMusicMatchStatuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OnMusicMatchStatuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OnMusicMatchStatuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "OnMusicMatches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
