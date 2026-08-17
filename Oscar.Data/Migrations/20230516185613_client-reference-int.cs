using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oscar.Data.Migrations
{
    public partial class clientreferenceint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE dbo.Clients SET (SYSTEM_VERSIONING = OFF);" 
                                 + "ALTER TABLE[ClientsHistory] ALTER COLUMN[ClientReference] int NULL;" 
                                 + "ALTER TABLE[Clients] ALTER COLUMN[ClientReference] int NULL;" 
                + "ALTER TABLE dbo.Clients SET(SYSTEM_VERSIONING = ON);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE dbo.Clients SET (SYSTEM_VERSIONING = OFF);" 
                                 + "ALTER TABLE[ClientsHistory] ALTER COLUMN[ClientReference] nvarchar(max) NULL;"
                                 + "ALTER TABLE[Clients] ALTER COLUMN[ClientReference] nvarchar(max NULL;"
                                 + "ALTER TABLE dbo.Clients SET(SYSTEM_VERSIONING = ON);");
        }
    }
}
