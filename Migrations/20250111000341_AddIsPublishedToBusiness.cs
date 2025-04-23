using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingService.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPublishedToBusiness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Schedule_Businesses_BusinessId",
                table: "Businesses_Schedule");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Businesses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Schedule_Businesses_BusinessId",
                table: "Businesses_Schedule",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Schedule_Businesses_BusinessId",
                table: "Businesses_Schedule");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Businesses");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Schedule_Businesses_BusinessId",
                table: "Businesses_Schedule",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
