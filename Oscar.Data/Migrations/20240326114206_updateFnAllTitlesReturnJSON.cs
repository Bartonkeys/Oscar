using Microsoft.EntityFrameworkCore.Migrations;
using Oscar.Data.Scripts;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class updateFnAllTitlesReturnJSON : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(OnMusicViewModel.fnAllTitlesUpdate);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
