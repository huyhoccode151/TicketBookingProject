using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBookingProject.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddCancelReasonIntoEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cancel_reason",
                table: "events",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cancel_reason",
                table: "events");
        }
    }
}
