using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class Add_MerlinSociety : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MerlinSociety",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerlinId = table.Column<int>(type: "int", nullable: false),
                    Merlin_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Merlin_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MRIT_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MRIT_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AGICOA_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AGICOA_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AGICOAGmbh_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AGICOAGmbh_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ROVI_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ROVI_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TVCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Countries_CR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Countries_BT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Countries_EC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmJus_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmJus_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScreenRights_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScreenRights_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PROCIBEL_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PROCIBEL_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EGEDA_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EGEDA_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FILMKOPI_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FILMKOPI_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FRF_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FRF_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PROCIREP_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PROCIREP_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SIAE_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SIAE_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SACEM_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SACEM_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SEKAM_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SEKAM_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SUISSIMAGE_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SUISSIMAGE_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VAM_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VAM_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VGF_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VGF_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VFF_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VFF_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GWFF_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GWFF_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZAPA_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZAPA_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NORWACO_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NORWACO_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VIDEMA_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VIDEMA_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ANGOA_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ANGOA_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gedipe_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gedipe_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    APA_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    APA_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Conductor_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Conductor_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UPFAR_ARGOA_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UPFAR_ARGOA_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PRD_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PRD_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LITA_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LITA_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CMC_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CMC_ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerlinSociety", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MerlinSociety");
        }
    }
}
