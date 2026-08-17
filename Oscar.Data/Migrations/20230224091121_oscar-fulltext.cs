using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class oscarfulltext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                sql: "CREATE FULLTEXT CATALOG oscarCatalog AS DEFAULT;",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: "CREATE FULLTEXT INDEX ON WorksTitle(Title) KEY INDEX PK_WorksTitle;",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: "CREATE FULLTEXT INDEX ON Actor(FirstName, LastName) KEY INDEX PK_Actor;",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: "CREATE FULLTEXT INDEX ON Director(FirstName, LastName) KEY INDEX PK_Director;",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: "CREATE FULLTEXT INDEX ON Producer(FirstName, LastName) KEY INDEX PK_Producer",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: "CREATE FULLTEXT INDEX ON ScreenWriter(FirstName, LastName) KEY INDEX PK_ScreenWriter;",
                suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(sql: "DROP FULLTEXT INDEX ON Director", suppressTransaction: true);
            migrationBuilder.Sql(sql: "DROP FULLTEXT INDEX ON Producer", suppressTransaction: true);
            migrationBuilder.Sql(sql: "DROP FULLTEXT INDEX ON ScreenWriter", suppressTransaction: true);
            migrationBuilder.Sql(sql: "DROP FULLTEXT INDEX ON Actor", suppressTransaction: true);
            migrationBuilder.Sql(sql: "DROP FULLTEXT INDEX ON WorksTitle", suppressTransaction: true);
            migrationBuilder.Sql(sql: "DROP FULLTEXT CATALOG oscarCatalog", suppressTransaction: true);
        }
    }
}
