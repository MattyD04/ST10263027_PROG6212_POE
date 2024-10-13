using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ST10263027_PROG6212_POE.Migrations
{
    /// <inheritdoc />
    public partial class AddProgrammeCoordinator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgrammeCoordinators",
                columns: table => new
                {
                    CoordinatorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoordinatorNum = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammeCoordinators", x => x.CoordinatorID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgrammeCoordinators");
        }
    }
}
