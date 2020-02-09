﻿// <auto-generated />
namespace Barber.IoT.Context.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "io_t");

            migrationBuilder.CreateTable(
                name: "device",
                schema: "io_t",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    access_failed_count = table.Column<int>(nullable: false),
                    concurrency_stamp = table.Column<string>(nullable: false),
                    lockout_enabled = table.Column<bool>(nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(nullable: true),
                    name = table.Column<string>(maxLength: 256, nullable: false),
                    normalized_name = table.Column<string>(maxLength: 256, nullable: false),
                    password_hash = table.Column<string>(nullable: true),
                    security_stamp = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "NameIndex",
                schema: "io_t",
                table: "device",
                column: "normalized_name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "device",
                schema: "io_t");
        }
    }
}
