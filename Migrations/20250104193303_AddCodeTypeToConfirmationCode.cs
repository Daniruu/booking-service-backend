using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingService.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeTypeToConfirmationCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Schedule_Businesses_BusinessId",
                table: "Businesses_Schedule");

            migrationBuilder.AddColumn<int>(
                name: "CodeType",
                table: "ConfirmationCodes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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
                name: "CodeType",
                table: "ConfirmationCodes");

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
