using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingService.Migrations
{
    /// <inheritdoc />
    public partial class MakeRefreshExpiryTimeNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesss_Schedule_Businesss_BusinessId",
                table: "Businesss_Schedule");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "RefreshExpiryTime",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

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

            migrationBuilder.AlterColumn<DateTime>(
                name: "RefreshExpiryTime",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Businesss_Schedule_Businesss_BusinessId",
                table: "Businesss_Schedule",
                column: "BusinessId",
                principalTable: "Businesss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
