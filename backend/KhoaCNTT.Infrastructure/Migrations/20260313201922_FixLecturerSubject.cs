using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhoaCNTT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixLecturerSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LecturerSubjects",
                table: "LecturerSubjects");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "LecturerSubjects");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LecturerSubjects",
                table: "LecturerSubjects",
                columns: new[] { "LecturerId", "SubjectCode" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LecturerSubjects",
                table: "LecturerSubjects");

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "LecturerSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LecturerSubjects",
                table: "LecturerSubjects",
                columns: new[] { "LecturerId", "SubjectId" });
        }
    }
}
