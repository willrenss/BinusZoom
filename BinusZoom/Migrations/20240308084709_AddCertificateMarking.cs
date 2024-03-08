using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BinusZoom.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificateMarking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EligibleForCertificate",
                table: "Registration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "hasSendCertificateToAll",
                table: "Meeting",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EligibleForCertificate",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "hasSendCertificateToAll",
                table: "Meeting");
        }
    }
}
