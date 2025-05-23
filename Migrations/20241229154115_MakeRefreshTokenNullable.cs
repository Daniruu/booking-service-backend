﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingService.Migrations
{
    /// <inheritdoc />
    public partial class MakeRefreshTokenNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesss_Schedule_Businesss_BusinessId",
                table: "Businesss_Schedule");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

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

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
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
