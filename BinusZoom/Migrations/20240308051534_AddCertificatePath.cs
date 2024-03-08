using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BinusZoom.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificatePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registration_Meeting_MeetingId",
                table: "Registration");

            migrationBuilder.AlterColumn<string>(
                name: "MeetingId",
                table: "Registration",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificatePath",
                table: "Meeting",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Registration_Meeting_MeetingId",
                table: "Registration",
                column: "MeetingId",
                principalTable: "Meeting",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registration_Meeting_MeetingId",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "CertificatePath",
                table: "Meeting");

            migrationBuilder.AlterColumn<string>(
                name: "MeetingId",
                table: "Registration",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Registration_Meeting_MeetingId",
                table: "Registration",
                column: "MeetingId",
                principalTable: "Meeting",
                principalColumn: "Id");
        }
    }
}
