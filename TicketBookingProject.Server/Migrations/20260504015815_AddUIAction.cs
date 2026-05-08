using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBookingProject.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddUIAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UiActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoutePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermissionRequired = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionType = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "nav"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UiActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UiActions_UiActions_ParentId",
                        column: x => x.ParentId,
                        principalTable: "UiActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UiActions_ActionKey",
                table: "UiActions",
                column: "ActionKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UiActions_ParentId",
                table: "UiActions",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UiActions");
        }
    }
}
