using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GMAPI.Migrations
{
    public partial class AddEventDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Event_EventId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_EventId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Accounts");

            migrationBuilder.AddColumn<string>(
                name: "EventDescription",
                table: "Event",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventDescription",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Accounts");

            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "Accounts",
                type: "uuid",
                nullable: true);

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
    }
}
