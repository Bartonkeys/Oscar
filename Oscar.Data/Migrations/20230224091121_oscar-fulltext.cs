using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class oscarfulltext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            const string fullTextCheck = "IF SERVERPROPERTY('IsFullTextInstalled') = 1 BEGIN ";

            migrationBuilder.Sql(
                sql: fullTextCheck + "CREATE FULLTEXT CATALOG oscarCatalog AS DEFAULT; END",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: fullTextCheck + "CREATE FULLTEXT INDEX ON WorksTitle(Title) KEY INDEX PK_WorksTitle; END",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: fullTextCheck + "CREATE FULLTEXT INDEX ON Actor(FirstName, LastName) KEY INDEX PK_Actor; END",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: fullTextCheck + "CREATE FULLTEXT INDEX ON Director(FirstName, LastName) KEY INDEX PK_Director; END",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: fullTextCheck + "CREATE FULLTEXT INDEX ON Producer(FirstName, LastName) KEY INDEX PK_Producer; END",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: fullTextCheck + "CREATE FULLTEXT INDEX ON ScreenWriter(FirstName, LastName) KEY INDEX PK_ScreenWriter; END",
                suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            const string fullTextCheck = "IF SERVERPROPERTY('IsFullTextInstalled') = 1 BEGIN ";

            migrationBuilder.Sql(sql: fullTextCheck + "DROP FULLTEXT INDEX ON Director; END", suppressTransaction: true);
            migrationBuilder.Sql(sql: fullTextCheck + "DROP FULLTEXT INDEX ON Producer; END", suppressTransaction: true);
            migrationBuilder.Sql(sql: fullTextCheck + "DROP FULLTEXT INDEX ON ScreenWriter; END", suppressTransaction: true);
            migrationBuilder.Sql(sql: fullTextCheck + "DROP FULLTEXT INDEX ON Actor; END", suppressTransaction: true);
            migrationBuilder.Sql(sql: fullTextCheck + "DROP FULLTEXT INDEX ON WorksTitle; END", suppressTransaction: true);
            migrationBuilder.Sql(sql: fullTextCheck + "DROP FULLTEXT CATALOG oscarCatalog; END", suppressTransaction: true);
        }
    }
}
