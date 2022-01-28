using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.SmsSender.Postgres.Migrations
{
    public partial class NullableFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ExternalMessageId",
                schema: "smssender",
                table: "sent_history",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ExternalMessageId",
                schema: "smssender",
                table: "sent_history",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
