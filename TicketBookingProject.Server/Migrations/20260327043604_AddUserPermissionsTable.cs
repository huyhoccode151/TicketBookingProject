using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBookingProject.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPermissionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    Effect = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1, comment: "1=allow, -1=deny (override role)"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_permissions_permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_permissions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_permissions_PermissionId",
                table: "user_permissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_user_permissions_UserId",
                table: "user_permissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_permissions_UserId_Effect",
                table: "user_permissions",
                columns: new[] { "UserId", "Effect" });

            migrationBuilder.CreateIndex(
                name: "IX_user_permissions_UserId_PermissionId",
                table: "user_permissions",
                columns: new[] { "UserId", "PermissionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_permissions");
        }
    }
}
