using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class RemoveRightsCountryGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CountryGroupRight");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CountryGroupRight",
                columns: table => new
                {
                    CountryGroupsId = table.Column<int>(type: "int", nullable: false),
                    RightsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryGroupRight", x => new { x.CountryGroupsId, x.RightsId });
                    table.ForeignKey(
                        name: "FK_CountryGroupRight_CountryGroup_CountryGroupsId",
                        column: x => x.CountryGroupsId,
                        principalTable: "CountryGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CountryGroupRight_Rights_RightsId",
                        column: x => x.RightsId,
                        principalTable: "Rights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CountryGroupRight_RightsId",
                table: "CountryGroupRight",
                column: "RightsId");
        }
    }
}
