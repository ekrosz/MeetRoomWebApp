using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MeetRoomWebApp.Migrations
{
    public partial class updateTypeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionDurationInMinutes",
                table: "Sessions");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "SessionDuration",
                table: "Sessions",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionDuration",
                table: "Sessions");

            migrationBuilder.AddColumn<int>(
                name: "SessionDurationInMinutes",
                table: "Sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
