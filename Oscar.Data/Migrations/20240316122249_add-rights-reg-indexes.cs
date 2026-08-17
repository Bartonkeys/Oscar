using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class addrightsregindexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(SqlResources.Indexes);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
