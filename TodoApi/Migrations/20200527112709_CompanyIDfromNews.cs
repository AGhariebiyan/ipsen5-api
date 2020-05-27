using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GMAPI.Migrations
{
    public partial class CompanyIDfromNews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_Companies_CompanyId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_News_CompanyId",
                table: "News");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "News");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "News",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_News_CompanyId",
                table: "News",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Companies_CompanyId",
                table: "News",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
