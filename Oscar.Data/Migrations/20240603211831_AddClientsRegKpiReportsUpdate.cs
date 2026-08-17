using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class AddClientsRegKpiReportsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(SqlResources.V_ClientDetails);
            migrationBuilder.Sql(SqlResources.V_ClientCataloguesDetails);
            migrationBuilder.Sql(SqlResources.V_ClientWorksList);
            migrationBuilder.Sql(SqlResources.V_ClientWorkListOfRights);
            migrationBuilder.Sql(SqlResources.V_ClientProductionCountries);
            migrationBuilder.Sql(SqlResources.V_ClientWorkYearlyStats);
            migrationBuilder.Sql(SqlResources.V_ClientWorkStatsByProductionYear);
            migrationBuilder.Sql(SqlResources.sp_GetClientWorkYearlyStats);
            migrationBuilder.Sql(SqlResources.sp_GetClientWorkProductionYearlyStats);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientDetails]");
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientCataloguesDetails]");
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientWorksList]");
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientWorkListOfRights]");
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientProductionCountries]");
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientWorkYearlyStats]");
            migrationBuilder.Sql("DROP VIEW [dbo].[V_ClientWorkStatsByProductionYear]");
            migrationBuilder.Sql("DROP PROCEDURE [sp_GetClientWorkYearlyStats]");
            migrationBuilder.Sql("DROP PROCEDURE [sp_GetClientWorkProductionYearlyStats]");
        }
    }
}
