using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhoaCNTT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSizeFromNewsResource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_Admins_CreatedById",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "NewsResource");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "News",
                newName: "CreatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_News_CreatedById",
                table: "News",
                newName: "IX_News_CreatedBy");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "NewsResource",
                type: "nvarchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Admins_CreatedBy",
                table: "News",
                column: "CreatedBy",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_Admins_CreatedBy",
                table: "News");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "News",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_News_CreatedBy",
                table: "News",
                newName: "IX_News_CreatedById");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "NewsResource",
                type: "nvarchar(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)");

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "NewsResource",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_News_Admins_CreatedById",
                table: "News",
                column: "CreatedById",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
