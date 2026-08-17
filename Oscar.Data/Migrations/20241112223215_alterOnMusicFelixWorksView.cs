using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class alterOnMusicFelixWorksView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(SqlResources.V_AlterClientWorksList);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
