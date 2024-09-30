using Microsoft.EntityFrameworkCore.Migrations;

namespace Project_Web___Veterinary_Clínic.Migrations
{
    public partial class Speciality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Specialty",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Specialty",
                table: "AspNetUsers");
        }
    }
}
