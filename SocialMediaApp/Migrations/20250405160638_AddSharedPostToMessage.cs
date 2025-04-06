using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMediaApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSharedPostToMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SharedPostId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SharedPostId",
                table: "Messages",
                column: "SharedPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Posts_SharedPostId",
                table: "Messages",
                column: "SharedPostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Posts_SharedPostId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SharedPostId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SharedPostId",
                table: "Messages");
        }
    }
}
