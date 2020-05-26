using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GMAPI.Migrations
{
    public partial class newEventsModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "Accounts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EventName = table.Column<string>(nullable: false),
                    EventLocationName = table.Column<string>(nullable: false),
                    EventLocationStreet = table.Column<string>(nullable: false),
                    EventLocationPostalCode = table.Column<string>(nullable: false),
                    EventLocationRegion = table.Column<string>(nullable: false),
                    EventLocationCountry = table.Column<string>(nullable: false),
                    EventDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_EventId",
                table: "Accounts",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Event_EventId",
                table: "Accounts",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Event_EventId",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_EventId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Accounts");
        }
    }
}
