using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class addsocietyinconflicts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SocietyId",
                table: "Conflict",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conflict_SocietyId",
                table: "Conflict",
                column: "SocietyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conflict_Society_SocietyId",
                table: "Conflict",
                column: "SocietyId",
                principalTable: "Society",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conflict_Society_SocietyId",
                table: "Conflict");

            migrationBuilder.DropIndex(
                name: "IX_Conflict_SocietyId",
                table: "Conflict");

            migrationBuilder.DropColumn(
                name: "SocietyId",
                table: "Conflict")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ConflictHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null);
        }
    }
}
