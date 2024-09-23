using Microsoft.EntityFrameworkCore.Migrations;

namespace Project_Web___Veterinary_Clínic.Migrations
{
    public partial class FieldRenamedtoDono : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AspNetUsers_UserId",
                table: "Animals");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Animals",
                newName: "DonoId");

            migrationBuilder.RenameIndex(
                name: "IX_Animals_UserId",
                table: "Animals",
                newName: "IX_Animals_DonoId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Rooms",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AspNetUsers_DonoId",
                table: "Animals",
                column: "DonoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AspNetUsers_DonoId",
                table: "Animals");

            migrationBuilder.RenameColumn(
                name: "DonoId",
                table: "Animals",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Animals_DonoId",
                table: "Animals",
                newName: "IX_Animals_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AspNetUsers_UserId",
                table: "Animals",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
