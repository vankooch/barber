namespace Barber.IoT.Context.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;
    using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "iot");

            migrationBuilder.CreateTable(
                name: "device",
                schema: "iot",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    access_failed_count = table.Column<int>(nullable: false),
                    concurrency_stamp = table.Column<string>(nullable: false),
                    lockout_enabled = table.Column<bool>(nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(nullable: true),
                    name = table.Column<string>(maxLength: 512, nullable: false),
                    normalized_name = table.Column<string>(maxLength: 512, nullable: false),
                    password_hash = table.Column<string>(nullable: true),
                    security_stamp = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "device_activity",
                schema: "iot",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<int>(nullable: false),
                    date = table.Column<DateTime>(nullable: false),
                    device_id = table.Column<string>(nullable: false),
                    payload = table.Column<string>(nullable: true),
                    state = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_activity", x => x.id);
                    table.ForeignKey(
                        name: "FK_device_activity_device_device_id",
                        column: x => x.device_id,
                        principalSchema: "iot",
                        principalTable: "device",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "NameIndex",
                schema: "iot",
                table: "device",
                column: "normalized_name");

            migrationBuilder.CreateIndex(
                name: "DeviceIdIndex",
                schema: "iot",
                table: "device_activity",
                column: "device_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "device_activity",
                schema: "iot");

            migrationBuilder.DropTable(
                name: "device",
                schema: "iot");
        }
    }
}
