using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class ClientWorksList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(SqlResources.V_ClientDetails);
            migrationBuilder.Sql(SqlResources.V_ClientCataloguesDetails);
            migrationBuilder.Sql(SqlResources.V_ClientWorksList);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientDetails]");
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientCataloguesDetails]");
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientWorksList]");
        }
    }
}
