using Microsoft.EntityFrameworkCore.Migrations;

namespace Project_Web___Veterinary_Clínic.Migrations
{
    public partial class updatefields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AspNetUsers_DonoId",
                table: "Animals");

            migrationBuilder.RenameColumn(
                name: "DonoId",
                table: "Animals",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Animals_DonoId",
                table: "Animals",
                newName: "IX_Animals_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AspNetUsers_OwnerId",
                table: "Animals",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AspNetUsers_OwnerId",
                table: "Animals");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Animals",
                newName: "DonoId");

            migrationBuilder.RenameIndex(
                name: "IX_Animals_OwnerId",
                table: "Animals",
                newName: "IX_Animals_DonoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AspNetUsers_DonoId",
                table: "Animals",
                column: "DonoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
