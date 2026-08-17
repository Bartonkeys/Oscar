using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Oscar.Mrit.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AltProductionTitle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AltProductionTitle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AltRecordTitles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AltRecordTitles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BatchJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchJobKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SimpleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alpha2Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Alpha3Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumericCode = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ISO639_2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ISO639_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnglishName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FrenchName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GermanName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Forename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiddleNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SimpleName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Territories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Territories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Works",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorksId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Works", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductionYear = table.Column<int>(type: "int", nullable: true),
                    SeasonNumber = table.Column<int>(type: "int", nullable: true),
                    EpisodeNumber = table.Column<int>(type: "int", nullable: true),
                    BatchJobId = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_BatchJobs_BatchJobId",
                        column: x => x.BatchJobId,
                        principalTable: "BatchJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonsOfInterest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: true),
                    PersonTypeId = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonsOfInterest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonsOfInterest_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonsOfInterest_PersonTypes_PersonTypeId",
                        column: x => x.PersonTypeId,
                        principalTable: "PersonTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AltProductionTitleMatch",
                columns: table => new
                {
                    AltProductionTitlesId = table.Column<int>(type: "int", nullable: false),
                    MatchesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AltProductionTitleMatch", x => new { x.AltProductionTitlesId, x.MatchesId });
                    table.ForeignKey(
                        name: "FK_AltProductionTitleMatch_AltProductionTitle_AltProductionTitlesId",
                        column: x => x.AltProductionTitlesId,
                        principalTable: "AltProductionTitle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AltProductionTitleMatch_Matches_MatchesId",
                        column: x => x.MatchesId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AltRecordTitleMatch",
                columns: table => new
                {
                    AltRecordTitlesId = table.Column<int>(type: "int", nullable: false),
                    MatchesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AltRecordTitleMatch", x => new { x.AltRecordTitlesId, x.MatchesId });
                    table.ForeignKey(
                        name: "FK_AltRecordTitleMatch_AltRecordTitles_AltRecordTitlesId",
                        column: x => x.AltRecordTitlesId,
                        principalTable: "AltRecordTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AltRecordTitleMatch_Matches_MatchesId",
                        column: x => x.MatchesId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyMatch",
                columns: table => new
                {
                    CompaniesId = table.Column<int>(type: "int", nullable: false),
                    MatchesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyMatch", x => new { x.CompaniesId, x.MatchesId });
                    table.ForeignKey(
                        name: "FK_CompanyMatch_Companies_CompaniesId",
                        column: x => x.CompaniesId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyMatch_Matches_MatchesId",
                        column: x => x.MatchesId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountryMatch",
                columns: table => new
                {
                    CountriesId = table.Column<int>(type: "int", nullable: false),
                    MatchesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryMatch", x => new { x.CountriesId, x.MatchesId });
                    table.ForeignKey(
                        name: "FK_CountryMatch_Countries_CountriesId",
                        column: x => x.CountriesId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CountryMatch_Matches_MatchesId",
                        column: x => x.MatchesId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreMatch",
                columns: table => new
                {
                    GenresId = table.Column<int>(type: "int", nullable: false),
                    MatchesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreMatch", x => new { x.GenresId, x.MatchesId });
                    table.ForeignKey(
                        name: "FK_GenreMatch_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreMatch_Matches_MatchesId",
                        column: x => x.MatchesId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LanguageMatch",
                columns: table => new
                {
                    LanguagesId = table.Column<int>(type: "int", nullable: false),
                    MatchesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageMatch", x => new { x.LanguagesId, x.MatchesId });
                    table.ForeignKey(
                        name: "FK_LanguageMatch_Languages_LanguagesId",
                        column: x => x.LanguagesId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LanguageMatch_Matches_MatchesId",
                        column: x => x.MatchesId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchWorks",
                columns: table => new
                {
                    MatchesId = table.Column<int>(type: "int", nullable: false),
                    WorksId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchWorks", x => new { x.MatchesId, x.WorksId });
                    table.ForeignKey(
                        name: "FK_MatchWorks_Matches_MatchesId",
                        column: x => x.MatchesId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchWorks_Works_WorksId",
                        column: x => x.WorksId,
                        principalTable: "Works",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MritId = table.Column<int>(type: "int", nullable: false),
                    TransmissionProductionTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransmissionEpisodeTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BroadcastDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BroadcastDuration = table.Column<int>(type: "int", nullable: false),
                    BroadcastLanguage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MatchId = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transmissions_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchPersonOfInterest",
                columns: table => new
                {
                    MatchesId = table.Column<int>(type: "int", nullable: false),
                    PersonOfInterestsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchPersonOfInterest", x => new { x.MatchesId, x.PersonOfInterestsId });
                    table.ForeignKey(
                        name: "FK_MatchPersonOfInterest_Matches_MatchesId",
                        column: x => x.MatchesId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchPersonOfInterest_PersonsOfInterest_PersonOfInterestsId",
                        column: x => x.PersonOfInterestsId,
                        principalTable: "PersonsOfInterest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TerritoryTransmission",
                columns: table => new
                {
                    TerritoriesId = table.Column<int>(type: "int", nullable: false),
                    TransmissionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerritoryTransmission", x => new { x.TerritoriesId, x.TransmissionsId });
                    table.ForeignKey(
                        name: "FK_TerritoryTransmission_Territories_TerritoriesId",
                        column: x => x.TerritoriesId,
                        principalTable: "Territories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TerritoryTransmission_Transmissions_TransmissionsId",
                        column: x => x.TransmissionsId,
                        principalTable: "Transmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PersonTypes",
                columns: new[] { "Id", "CreateDate", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2021, 6, 10, 13, 32, 21, 255, DateTimeKind.Local).AddTicks(7972), "Director" },
                    { 2, new DateTime(2021, 6, 10, 13, 32, 21, 257, DateTimeKind.Local).AddTicks(3860), "Actor" },
                    { 3, new DateTime(2021, 6, 10, 13, 32, 21, 257, DateTimeKind.Local).AddTicks(3898), "Producer" },
                    { 4, new DateTime(2021, 6, 10, 13, 32, 21, 257, DateTimeKind.Local).AddTicks(3903), "Creator" },
                    { 5, new DateTime(2021, 6, 10, 13, 32, 21, 257, DateTimeKind.Local).AddTicks(3905), "Writer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AltProductionTitleMatch_MatchesId",
                table: "AltProductionTitleMatch",
                column: "MatchesId");

            migrationBuilder.CreateIndex(
                name: "IX_AltRecordTitleMatch_MatchesId",
                table: "AltRecordTitleMatch",
                column: "MatchesId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMatch_MatchesId",
                table: "CompanyMatch",
                column: "MatchesId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryMatch_MatchesId",
                table: "CountryMatch",
                column: "MatchesId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreMatch_MatchesId",
                table: "GenreMatch",
                column: "MatchesId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageMatch_MatchesId",
                table: "LanguageMatch",
                column: "MatchesId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_BatchJobId",
                table: "Matches",
                column: "BatchJobId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchPersonOfInterest_PersonOfInterestsId",
                table: "MatchPersonOfInterest",
                column: "PersonOfInterestsId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchWorks_WorksId",
                table: "MatchWorks",
                column: "WorksId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonsOfInterest_PersonId",
                table: "PersonsOfInterest",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonsOfInterest_PersonTypeId",
                table: "PersonsOfInterest",
                column: "PersonTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TerritoryTransmission_TransmissionsId",
                table: "TerritoryTransmission",
                column: "TransmissionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Transmissions_MatchId",
                table: "Transmissions",
                column: "MatchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AltProductionTitleMatch");

            migrationBuilder.DropTable(
                name: "AltRecordTitleMatch");

            migrationBuilder.DropTable(
                name: "CompanyMatch");

            migrationBuilder.DropTable(
                name: "CountryMatch");

            migrationBuilder.DropTable(
                name: "GenreMatch");

            migrationBuilder.DropTable(
                name: "LanguageMatch");

            migrationBuilder.DropTable(
                name: "MatchPersonOfInterest");

            migrationBuilder.DropTable(
                name: "MatchWorks");

            migrationBuilder.DropTable(
                name: "TerritoryTransmission");

            migrationBuilder.DropTable(
                name: "AltProductionTitle");

            migrationBuilder.DropTable(
                name: "AltRecordTitles");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "PersonsOfInterest");

            migrationBuilder.DropTable(
                name: "Works");

            migrationBuilder.DropTable(
                name: "Territories");

            migrationBuilder.DropTable(
                name: "Transmissions");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "PersonTypes");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "BatchJobs");
        }
    }
}
