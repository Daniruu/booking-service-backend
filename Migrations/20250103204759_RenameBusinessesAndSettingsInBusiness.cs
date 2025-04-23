using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookingService.Migrations
{
    /// <inheritdoc />
    public partial class RenameBusinessesAndSettingsInBusiness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Businesss_BusinessId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessImages_Businesss_BusinessId",
                table: "BusinessImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesss_Users_UserId",
                table: "Businesss");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Businesss_BusinessId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteBusinesses_Businesss_BusinessId",
                table: "FavoriteBusinesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Businesss_BusinessId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceGroups_Businesss_BusinessId",
                table: "ServiceGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Businesss_BusinessId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "Businesss_Schedule_TimeSlots");

            migrationBuilder.DropTable(
                name: "Businesss_Schedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Businesss",
                table: "Businesss");

            migrationBuilder.RenameTable(
                name: "Businesss",
                newName: "Businesses");

            migrationBuilder.RenameColumn(
                name: "Setting_BookingBufferTime",
                table: "Businesses",
                newName: "Settings_BookingBufferTime");

            migrationBuilder.RenameColumn(
                name: "Setting_AutoConfirmBookings",
                table: "Businesses",
                newName: "Settings_AutoConfirmBookings");

            migrationBuilder.RenameIndex(
                name: "IX_Businesss_UserId",
                table: "Businesses",
                newName: "IX_Businesses_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Businesses",
                table: "Businesses",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Businesses_Schedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    BusinessId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses_Schedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Businesses_Schedule_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Businesses_Schedule_TimeSlots",
                columns: table => new
                {
                    DayScheduleId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses_Schedule_TimeSlots", x => new { x.DayScheduleId, x.Id });
                    table.ForeignKey(
                        name: "FK_Businesses_Schedule_TimeSlots_Businesses_Schedule_DaySchedu~",
                        column: x => x.DayScheduleId,
                        principalTable: "Businesses_Schedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_Schedule_BusinessId",
                table: "Businesses_Schedule",
                column: "BusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Businesses_BusinessId",
                table: "Bookings",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Users_UserId",
                table: "Businesses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessImages_Businesses_BusinessId",
                table: "BusinessImages",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Businesses_BusinessId",
                table: "Employees",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteBusinesses_Businesses_BusinessId",
                table: "FavoriteBusinesses",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Businesses_BusinessId",
                table: "Reviews",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceGroups_Businesses_BusinessId",
                table: "ServiceGroups",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Businesses_BusinessId",
                table: "Services",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Businesses_BusinessId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Users_UserId",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessImages_Businesses_BusinessId",
                table: "BusinessImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Businesses_BusinessId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteBusinesses_Businesses_BusinessId",
                table: "FavoriteBusinesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Businesses_BusinessId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceGroups_Businesses_BusinessId",
                table: "ServiceGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Businesses_BusinessId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "Businesses_Schedule_TimeSlots");

            migrationBuilder.DropTable(
                name: "Businesses_Schedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Businesses",
                table: "Businesses");

            migrationBuilder.RenameTable(
                name: "Businesses",
                newName: "Businesss");

            migrationBuilder.RenameColumn(
                name: "Settings_BookingBufferTime",
                table: "Businesss",
                newName: "Setting_BookingBufferTime");

            migrationBuilder.RenameColumn(
                name: "Settings_AutoConfirmBookings",
                table: "Businesss",
                newName: "Setting_AutoConfirmBookings");

            migrationBuilder.RenameIndex(
                name: "IX_Businesses_UserId",
                table: "Businesss",
                newName: "IX_Businesss_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Businesss",
                table: "Businesss",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Businesss_Schedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BusinessId = table.Column<int>(type: "integer", nullable: true),
                    Day = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesss_Schedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Businesss_Schedule_Businesss_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Businesss_Schedule_TimeSlots",
                columns: table => new
                {
                    DayScheduleId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesss_Schedule_TimeSlots", x => new { x.DayScheduleId, x.Id });
                    table.ForeignKey(
                        name: "FK_Businesss_Schedule_TimeSlots_Businesss_Schedule_DaySchedule~",
                        column: x => x.DayScheduleId,
                        principalTable: "Businesss_Schedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Businesss_Schedule_BusinessId",
                table: "Businesss_Schedule",
                column: "BusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Businesss_BusinessId",
                table: "Bookings",
                column: "BusinessId",
                principalTable: "Businesss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessImages_Businesss_BusinessId",
                table: "BusinessImages",
                column: "BusinessId",
                principalTable: "Businesss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Businesss_Users_UserId",
                table: "Businesss",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Businesss_BusinessId",
                table: "Employees",
                column: "BusinessId",
                principalTable: "Businesss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteBusinesses_Businesss_BusinessId",
                table: "FavoriteBusinesses",
                column: "BusinessId",
                principalTable: "Businesss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Businesss_BusinessId",
                table: "Reviews",
                column: "BusinessId",
                principalTable: "Businesss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceGroups_Businesss_BusinessId",
                table: "ServiceGroups",
                column: "BusinessId",
                principalTable: "Businesss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Businesss_BusinessId",
                table: "Services",
                column: "BusinessId",
                principalTable: "Businesss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
