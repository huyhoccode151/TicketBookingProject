using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBookingProject.Server.Migrations
{
    /// <inheritdoc />
    public partial class SwitchUQToIndexSeatPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa unique index cũ
            migrationBuilder.DropUniqueConstraint(
                name: "UQ_seat_position",
                table: "seats");

            // Tạo index mới (không unique)
            migrationBuilder.CreateIndex(
                name: "IDX_seat_position",
                table: "seats",
                columns: new[] { "venue_id", "row", "seat_number" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_seat_position",
                table: "seats");

            // Tạo lại unique index cũ
            migrationBuilder.CreateIndex(
                name: "UQ_seat_position",
                table: "seats",
                columns: new[] { "venue_id", "row", "seat_number" },
                unique: true);
        }
    }
}
