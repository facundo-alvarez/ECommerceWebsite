using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class ApplicationUserBirthdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BirthDate",
                table: "AspNetUsers",
                newName: "Birthdate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Birthdate",
                table: "AspNetUsers",
                newName: "BirthDate");
        }
    }
}
