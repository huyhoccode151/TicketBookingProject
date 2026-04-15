using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBookingProject.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityToBookingDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "quantity",
                table: "booking_details",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quantity",
                table: "booking_details");
        }
    }
}
