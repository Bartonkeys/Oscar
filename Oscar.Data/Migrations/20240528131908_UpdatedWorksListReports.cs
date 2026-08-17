using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class UpdatedWorksListReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Cleanup Obsolete tables
            /*
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientDetails]");
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientCataloguesDetails]");
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientProductionCountries]");
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientWorksList]");
            */
            migrationBuilder.Sql(SqlResources.V_ClientDetails);
            migrationBuilder.Sql(SqlResources.V_ClientCataloguesDetails);
            migrationBuilder.Sql(SqlResources.V_ClientWorksList);
            migrationBuilder.Sql(SqlResources.V_ClientWorkListOfRights);
            migrationBuilder.Sql(SqlResources.V_ClientProductionCountries);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientDetails]");
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientCataloguesDetails]");
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientWorksList]");
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientWorkListOfRights]");
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientProductionCountries]");
        }
    }
}
