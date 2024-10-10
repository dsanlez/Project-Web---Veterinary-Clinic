using Microsoft.EntityFrameworkCore.Migrations;

namespace Project_Web___Veterinary_Clínic.Migrations
{
    public partial class TimeToAppointments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Time",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "Appointments");
        }
    }
}
