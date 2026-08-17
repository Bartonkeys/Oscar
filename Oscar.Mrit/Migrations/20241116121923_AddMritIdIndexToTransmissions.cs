using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Mrit.Migrations
{
    public partial class AddMritIdIndexToTransmissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transmissions_MritId",
                table: "Transmissions",
                column: "MritId",
                unique: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transmissions_MritId",
                table: "Transmissions");
        }
    }
}
