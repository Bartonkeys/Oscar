using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class updatesp_GetWorksTitleByTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(SqlResources.sp_GetWorksTitleByTitle);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[sp_GetWorksTitleByTitle]");
        }
    }
}
