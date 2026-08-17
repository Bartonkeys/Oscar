using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class AddiMaestroclientfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IMaestroGroupPayeeCode",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IMaestroGroupPayeeName",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "RegistrationConfiguration",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "REGISTRATIONBATCH");

            migrationBuilder.UpdateData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: -1,
                column: "Name",
                value: "ANY");

            migrationBuilder.UpdateData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "ACTIVE");

            migrationBuilder.UpdateData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "UNCONTROLLED");

            migrationBuilder.UpdateData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "INCOMPLETE");

            migrationBuilder.UpdateData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "RELINQUISHED");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IMaestroGroupPayeeCode",
                table: "Clients")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ClientsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null);

            migrationBuilder.DropColumn(
                name: "IMaestroGroupPayeeName",
                table: "Clients")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ClientsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null);

            migrationBuilder.UpdateData(
                table: "RegistrationConfiguration",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "RegistrationBatch");

            migrationBuilder.UpdateData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: -1,
                column: "Name",
                value: "Any");

            migrationBuilder.UpdateData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Active");

            migrationBuilder.UpdateData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Uncontrolled");

            migrationBuilder.UpdateData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Incomplete");

            migrationBuilder.UpdateData(
                table: "WorksStatus",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Relinquished");
        }
    }
}
