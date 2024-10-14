using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ST10263027_PROG6212_POE.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordtoProgrammeCoordinator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "ProgrammeCoordinators",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "ProgrammeCoordinators");
        }
    }
}
