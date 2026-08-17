using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class addclientcsmentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientCustomServiceManager");

            migrationBuilder.DropTable(
                name: "ClientPreviousCustomServiceManager");

            migrationBuilder.DropTable(
                name: "CustomServiceManager");

            migrationBuilder.DropTable(
                name: "PreviousCustomServiceManager");

            migrationBuilder.CreateTable(
                name: "Operators",
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
                    table.PrimaryKey("PK_Operators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerServiceManager",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OperatorId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerServiceManager", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerServiceManager_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerServiceManager_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerServiceManager_ClientId",
                table: "CustomerServiceManager",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerServiceManager_OperatorId",
                table: "CustomerServiceManager",
                column: "OperatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerServiceManager");

            migrationBuilder.DropTable(
                name: "Operators");

            migrationBuilder.CreateTable(
                name: "CustomServiceManager",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomServiceManager", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreviousCustomServiceManager",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreviousCustomServiceManager", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientCustomServiceManager",
                columns: table => new
                {
                    ClientsId = table.Column<int>(type: "int", nullable: false),
                    CustomServiceManagersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientCustomServiceManager", x => new { x.ClientsId, x.CustomServiceManagersId });
                    table.ForeignKey(
                        name: "FK_ClientCustomServiceManager_Clients_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientCustomServiceManager_CustomServiceManager_CustomServiceManagersId",
                        column: x => x.CustomServiceManagersId,
                        principalTable: "CustomServiceManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_ClientCustomServiceManager_CustomServiceManagersId",
                table: "ClientCustomServiceManager",
                column: "CustomServiceManagersId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPreviousCustomServiceManager_PreviousCustomServiceManagersId",
                table: "ClientPreviousCustomServiceManager",
                column: "PreviousCustomServiceManagersId");
        }
    }
}
