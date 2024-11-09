using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ST10263027_PROG6212_POE.Migrations
{
    /// <inheritdoc />
    public partial class fixedError : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HumanResorce",
                table: "HumanResorce");

            migrationBuilder.RenameTable(
                name: "HumanResorce",
                newName: "HumanResource");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HumanResource",
                table: "HumanResource",
                column: "HumanResourcesID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HumanResource",
                table: "HumanResource");

            migrationBuilder.RenameTable(
                name: "HumanResource",
                newName: "HumanResorce");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HumanResorce",
                table: "HumanResorce",
                column: "HumanResourcesID");
        }
    }
}
