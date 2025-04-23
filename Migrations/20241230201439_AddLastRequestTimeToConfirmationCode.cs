using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingService.Migrations
{
    /// <inheritdoc />
    public partial class AddLastRequestTimeToConfirmationCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesss_Schedule_Businesss_BusinessId",
                table: "Businesss_Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_ConfirmationCodes_Users_UserId",
                table: "ConfirmationCodes");

            migrationBuilder.DropIndex(
                name: "IX_ConfirmationCodes_UserId",
                table: "ConfirmationCodes");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ConfirmationCodes");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ConfirmationCodes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastRequestTime",
                table: "ConfirmationCodes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddForeignKey(
                name: "FK_Businesss_Schedule_Businesss_BusinessId",
                table: "Businesss_Schedule",
                column: "BusinessId",
                principalTable: "Businesss",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesss_Schedule_Businesss_BusinessId",
                table: "Businesss_Schedule");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "ConfirmationCodes");

            migrationBuilder.DropColumn(
                name: "LastRequestTime",
                table: "ConfirmationCodes");

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ConfirmationCodes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationCodes_UserId",
                table: "ConfirmationCodes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesss_Schedule_Businesss_BusinessId",
                table: "Businesss_Schedule",
                column: "BusinessId",
                principalTable: "Businesss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConfirmationCodes_Users_UserId",
                table: "ConfirmationCodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
