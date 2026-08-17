using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class AddPreviousCustomServiceManagers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                $"DROP VIEW [dbo].[vw_Works];\r\nGO\r\n\r\nDROP VIEW [dbo].[vw_WorksWithCountry];\r\nGO\r\n\r\nDROP FUNCTION [dbo].[getWorksByCountry]");

            migrationBuilder.DropForeignKey(
                name: "FK_Works_CustomServiceManager_CustomServiceManagerId",
                table: "Works");

            migrationBuilder.DropIndex(
                name: "IX_Works_CustomServiceManagerId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "CustomServiceManagerId",
                table: "Works")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "WorksHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null);

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "CustomServiceManager");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "CustomServiceManager",
                newName: "FullName");

            migrationBuilder.CreateTable(
                name: "PreviousCustomServiceManager",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreviousCustomServiceManager", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientPreviousCustomServiceManager",
                columns: table => new
                {
                    ClientsId = table.Column<int>(type: "int", nullable: false),
                    PreviousCustomServiceManagersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientPreviousCustomServiceManager", x => new { x.ClientsId, x.PreviousCustomServiceManagersId });
                    table.ForeignKey(
                        name: "FK_ClientPreviousCustomServiceManager_Clients_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientPreviousCustomServiceManager_PreviousCustomServiceManager_PreviousCustomServiceManagersId",
                        column: x => x.PreviousCustomServiceManagersId,
                        principalTable: "PreviousCustomServiceManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientPreviousCustomServiceManager_PreviousCustomServiceManagersId",
                table: "ClientPreviousCustomServiceManager",
                column: "PreviousCustomServiceManagersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientPreviousCustomServiceManager");

            migrationBuilder.DropTable(
                name: "PreviousCustomServiceManager");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "CustomServiceManager",
                newName: "LastName");

            migrationBuilder.AddColumn<int>(
                name: "CustomServiceManagerId",
                table: "Works",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "CustomServiceManager",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Works_CustomServiceManagerId",
                table: "Works",
                column: "CustomServiceManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_CustomServiceManager_CustomServiceManagerId",
                table: "Works",
                column: "CustomServiceManagerId",
                principalTable: "CustomServiceManager",
                principalColumn: "Id");

            migrationBuilder.Sql($"CREATE VIEW [dbo].[vw_Works] WITH SCHEMABINDING\r\nAS\r\nSELECT dbo.Works.[Id]\r\n      ,dbo.Works.[WorksStatus]\r\n      ,dbo.Works.[Discriminator]\r\n      ,dbo.Works.[AgicoaWorksReference]\r\n      ,dbo.Works.[CavcoCtcCode]\r\n      ,dbo.Works.[CreationDate]\r\n      ,dbo.Works.[FirstBroadcastYear]\r\n      ,dbo.Works.[GeneralNotes]\r\n      ,dbo.Works.[GenreId]\r\n      ,dbo.Works.[IMaestroWorkCode]\r\n      ,dbo.Works.[Isan]\r\n      ,dbo.Works.[LastModified]\r\n      ,dbo.Works.[SeasonId]\r\n      ,dbo.Works.[SeriesId]\r\n      ,dbo.Works.[ProductionYear]\r\n      ,dbo.Works.[Episode_SeriesId]\r\n      ,dbo.Works.[DurationMinutes]\r\n      ,dbo.Works.[CustomServiceManagerId]\r\n      ,dbo.Works.[Number]\r\n      ,dbo.Works.[WorksImportRequestId]\r\n      ,dbo.Works.[ColourFormat]\r\n      ,dbo.Works.[AS400RefNo]\r\n      ,dbo.Works.[CompactRef]\r\n      ,dbo.Works.[WorksTypeId]\r\n      ,dbo.Works.[GenreSubTypeId]\r\n\t  ,0 AS CountriesId\r\n\t  ,dbo.Works.[CommissionedWorkStatus] \r\n\t  ,dbo.Works.[ModifiedBy] \r\n\t  ,dbo.Works.[PeriodStart] \r\n\t  ,dbo.Works.[PeriodEnd]\r\n\t  ,dbo.Works.[WorksSubTypeId]\r\nFROM  dbo.Works \r\nGO\r\n\r\n\r\nCREATE UNIQUE CLUSTERED INDEX IX_VWorks\r\n\tON vw_Works\r\n\t ([Id], [WorksStatus], [Discriminator], [CreationDate] ,[FirstBroadcastYear],\r\n\t [GenreId], [LastModified], [SeasonId], [SeriesId], [ProductionYear],\r\n\t [Episode_SeriesId], [DurationMinutes], [CustomServiceManagerId], [Number], [WorksImportRequestId],\r\n\t [ColourFormat], [AS400RefNo], [CompactRef], [WorksTypeId], [GenreSubTypeId], CountriesId, \r\n\t [CommissionedWorkStatus], [PeriodStart], [PeriodEnd], [WorksSubTypeId]);\r\n\r\n\t GO\r\n\r\n\r\nCREATE VIEW [dbo].[vw_WorksWithCountry] WITH SCHEMABINDING\r\nAS\r\nSELECT dbo.Works.[Id]\r\n      ,dbo.Works.[WorksStatus]\r\n      ,dbo.Works.[Discriminator]\r\n      ,dbo.Works.[AgicoaWorksReference]\r\n      ,dbo.Works.[CavcoCtcCode]\r\n      ,dbo.Works.[CreationDate]\r\n      ,dbo.Works.[FirstBroadcastYear]\r\n      ,dbo.Works.[GeneralNotes]\r\n      ,dbo.Works.[GenreId]\r\n      ,dbo.Works.[IMaestroWorkCode]\r\n      ,dbo.Works.[Isan]\r\n      ,dbo.Works.[LastModified]\r\n      ,dbo.Works.[SeasonId]\r\n      ,dbo.Works.[SeriesId]\r\n      ,dbo.Works.[ProductionYear]\r\n      ,dbo.Works.[Episode_SeriesId]\r\n      ,dbo.Works.[DurationMinutes]\r\n      ,dbo.Works.[CustomServiceManagerId]\r\n      ,dbo.Works.[Number]\r\n      ,dbo.Works.[WorksImportRequestId]\r\n      ,dbo.Works.[ColourFormat]\r\n      ,dbo.Works.[AS400RefNo]\r\n      ,dbo.Works.[CompactRef]\r\n      ,dbo.Works.[WorksTypeId]\r\n      ,dbo.Works.[GenreSubTypeId]\r\n\t  ,dbo.CountryWorks.CountriesId\r\n\t  ,dbo.Works.[CommissionedWorkStatus] \r\n\t  ,dbo.Works.[ModifiedBy] \r\n\t  ,dbo.Works.[PeriodStart] \r\n\t  ,dbo.Works.[PeriodEnd]\r\n\t  ,dbo.Works.[WorksSubTypeId]\r\nFROM  dbo.Works INNER JOIN\r\n         dbo.CountryWorks ON dbo.Works.Id = dbo.CountryWorks.WorksId\r\n\r\nGO\r\n\r\n\r\nCREATE UNIQUE CLUSTERED INDEX IX_VWorksCountry \r\n\tON vw_WorksWithCountry\r\n\t ([Id], [WorksStatus], [Discriminator], [CreationDate] ,[FirstBroadcastYear],\r\n\t [GenreId], [LastModified], [SeasonId], [SeriesId], [ProductionYear],\r\n\t [Episode_SeriesId], [DurationMinutes], [CustomServiceManagerId], [Number], [WorksImportRequestId],\r\n\t [ColourFormat], [AS400RefNo], [CompactRef], [WorksTypeId], [GenreSubTypeId], CountriesId, \r\n\t [CommissionedWorkStatus], [PeriodStart], [PeriodEnd], [WorksSubTypeId]);\r\n\r\nGO\r\n\r\nCREATE FUNCTION [dbo].[getWorksByCountry]\r\n(\t\r\n\t@CountryID INT\r\n)\r\nRETURNS TABLE \r\nAS\r\nRETURN \r\n(\r\nwith ids as (\r\n  select Id from [vw_Works] with (index (IX_VWorks))\r\n  except\r\n  select Id from [vw_WorksWithCountry] with (index (IX_VWorksCountry))\r\n)\r\nselect a.Id AS Id\r\n      ,WorksStatus\r\n      ,Discriminator\r\n      ,AgicoaWorksReference\r\n      ,CavcoCtcCode\r\n      ,CreationDate\r\n      ,FirstBroadcastYear\r\n      ,GeneralNotes\r\n      ,GenreId\r\n      ,IMaestroWorkCode\r\n      ,Isan\r\n      ,LastModified\r\n      ,SeasonId\r\n      ,SeriesId\r\n      ,ProductionYear\r\n      ,Episode_SeriesId\r\n      ,DurationMinutes\r\n      ,CustomServiceManagerId\r\n      ,Number\r\n      ,WorksImportRequestId\r\n      ,ColourFormat\r\n      ,AS400RefNo\r\n      ,CompactRef\r\n      ,WorksTypeId\r\n      ,GenreSubTypeId\r\n\t  ,CountriesId\r\n      ,CommissionedWorkStatus, ModifiedBy, PeriodStart, PeriodEnd, WorksSubTypeId from [vw_Works] a join ids b on a.Id = b.Id\r\nWHERE (@CountryID = 0 OR CountriesId = @CountryID) \r\nunion\r\nselect Id\r\n      ,WorksStatus\r\n      ,Discriminator\r\n      ,AgicoaWorksReference\r\n      ,CavcoCtcCode\r\n      ,CreationDate\r\n      ,FirstBroadcastYear\r\n      ,GeneralNotes\r\n      ,GenreId\r\n      ,IMaestroWorkCode\r\n      ,Isan\r\n      ,LastModified\r\n      ,SeasonId\r\n      ,SeriesId\r\n      ,ProductionYear\r\n      ,Episode_SeriesId\r\n      ,DurationMinutes\r\n      ,CustomServiceManagerId\r\n      ,Number\r\n      ,WorksImportRequestId\r\n      ,ColourFormat\r\n      ,AS400RefNo\r\n      ,CompactRef\r\n      ,WorksTypeId\r\n      ,GenreSubTypeId\r\n\t  ,CountriesId\r\n      ,CommissionedWorkStatus, ModifiedBy, PeriodStart, PeriodEnd, WorksSubTypeId  from [vw_WorksWithCountry]\r\nWHERE (@CountryID = 0 OR CountriesId = @CountryID) \t\t\r\n)\r\n");
        }
    }
}
