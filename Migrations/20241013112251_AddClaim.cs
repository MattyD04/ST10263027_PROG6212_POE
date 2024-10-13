using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ST10263027_PROG6212_POE.Migrations
{
    /// <inheritdoc />
    public partial class AddClaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    ClaimID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimNum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LecturerID = table.Column<int>(type: "int", nullable: false),
                    CoordinatorID = table.Column<int>(type: "int", nullable: true),
                    ManagerID = table.Column<int>(type: "int", nullable: true),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClaimStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.ClaimID);
                    table.ForeignKey(
                        name: "FK_Claims_AcademicManagers_ManagerID",
                        column: x => x.ManagerID,
                        principalTable: "AcademicManagers",
                        principalColumn: "ManagerID");
                    table.ForeignKey(
                        name: "FK_Claims_Lecturers_LecturerID",
                        column: x => x.LecturerID,
                        principalTable: "Lecturers",
                        principalColumn: "LecturerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Claims_ProgrammeCoordinators_CoordinatorID",
                        column: x => x.CoordinatorID,
                        principalTable: "ProgrammeCoordinators",
                        principalColumn: "CoordinatorID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claims_CoordinatorID",
                table: "Claims",
                column: "CoordinatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_LecturerID",
                table: "Claims",
                column: "LecturerID");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ManagerID",
                table: "Claims",
                column: "ManagerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claims");
        }
    }
}
