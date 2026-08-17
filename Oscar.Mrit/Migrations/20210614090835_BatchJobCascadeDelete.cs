using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Oscar.Mrit.Migrations
{
    public partial class BatchJobCascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_BatchJobs_BatchJobId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Transmissions_Matches_MatchId",
                table: "Transmissions");

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2021, 6, 14, 10, 8, 34, 817, DateTimeKind.Local).AddTicks(895));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2021, 6, 14, 10, 8, 34, 822, DateTimeKind.Local).AddTicks(9269));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreateDate",
                value: new DateTime(2021, 6, 14, 10, 8, 34, 822, DateTimeKind.Local).AddTicks(9326));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreateDate",
                value: new DateTime(2021, 6, 14, 10, 8, 34, 822, DateTimeKind.Local).AddTicks(9335));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreateDate",
                value: new DateTime(2021, 6, 14, 10, 8, 34, 822, DateTimeKind.Local).AddTicks(9341));

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_BatchJobs_BatchJobId",
                table: "Matches",
                column: "BatchJobId",
                principalTable: "BatchJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transmissions_Matches_MatchId",
                table: "Transmissions",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_BatchJobs_BatchJobId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Transmissions_Matches_MatchId",
                table: "Transmissions");

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2021, 6, 10, 13, 45, 26, 975, DateTimeKind.Local).AddTicks(7263));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2021, 6, 10, 13, 45, 26, 977, DateTimeKind.Local).AddTicks(5148));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreateDate",
                value: new DateTime(2021, 6, 10, 13, 45, 26, 977, DateTimeKind.Local).AddTicks(5181));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreateDate",
                value: new DateTime(2021, 6, 10, 13, 45, 26, 977, DateTimeKind.Local).AddTicks(5185));

            migrationBuilder.UpdateData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreateDate",
                value: new DateTime(2021, 6, 10, 13, 45, 26, 977, DateTimeKind.Local).AddTicks(5188));

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_BatchJobs_BatchJobId",
                table: "Matches",
                column: "BatchJobId",
                principalTable: "BatchJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transmissions_Matches_MatchId",
                table: "Transmissions",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
