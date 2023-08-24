using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inforce_.NET_Task_Moskvichev_Bogdan.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUrlManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Urls",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Urls",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Urls");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Urls");
        }
    }
}
