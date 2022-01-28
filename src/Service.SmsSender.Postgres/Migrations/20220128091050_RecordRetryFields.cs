using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.SmsSender.Postgres.Migrations
{
    public partial class RecordRetryFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcDate",
                schema: "smssender",
                table: "sent_history",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<string>(
                name: "ExternalMessageId",
                schema: "smssender",
                table: "sent_history",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                schema: "smssender",
                table: "sent_history",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RetryId",
                schema: "smssender",
                table: "sent_history",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "smssender",
                table: "sent_history",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalMessageId",
                schema: "smssender",
                table: "sent_history");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                schema: "smssender",
                table: "sent_history");

            migrationBuilder.DropColumn(
                name: "RetryId",
                schema: "smssender",
                table: "sent_history");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "smssender",
                table: "sent_history");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcDate",
                schema: "smssender",
                table: "sent_history",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
