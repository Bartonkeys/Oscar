using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Oscar.Data.Scripts;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class AddOnMusicView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(OnMusicViewModel.fnActors_str);

            migrationBuilder.Sql(OnMusicViewModel.fnAllTitles);

            migrationBuilder.Sql(OnMusicViewModel.fnDirectors_str);

            migrationBuilder.Sql(OnMusicViewModel.fnProducers_str);

            migrationBuilder.Sql(OnMusicViewModel.fnProductionCompanies_str);

            migrationBuilder.Sql(OnMusicViewModel.vmOnMusicFelixWorks);

            migrationBuilder.CreateTable(
                name: "OnMusicMatchStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnMusicMatchStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnMusicMatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorksId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "varchar(max)", nullable: true),
                    OnMusicMatchStatusId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnMusicMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnMusicMatches_OnMusicMatchStatuses_OnMusicMatchStatusId",
                        column: x => x.OnMusicMatchStatusId,
                        principalTable: "OnMusicMatchStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnMusicMatches_OnMusicMatchStatusId",
                table: "OnMusicMatches",
                column: "OnMusicMatchStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION [fnActors_str]");
            migrationBuilder.Sql(@"DROP FUNCTION [fnAllTitles]");
            migrationBuilder.Sql(@"DROP FUNCTION [fnDirectors_str]");
            migrationBuilder.Sql(@"DROP FUNCTION [fnProducers_str]");
            migrationBuilder.Sql(@"DROP FUNCTION [fnProductionCompanies_str]");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS [vw_OnMusic_Felix_Works]");

            migrationBuilder.DropTable(
                name: "OnMusicMatches");

            migrationBuilder.DropTable(
                name: "OnMusicMatchStatuses");
        }
    }
}
