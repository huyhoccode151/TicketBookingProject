using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBookingProject.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingDetailsToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bd_event_seat",
                table: "booking_details");

            migrationBuilder.AlterColumn<int>(
                name: "event_seat_id",
                table: "booking_details",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ticket_type_id",
                table: "booking_details",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IDX_bd_ticket_type_id",
                table: "booking_details",
                column: "ticket_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_bd_event_seat",
                table: "booking_details",
                column: "event_seat_id",
                principalTable: "event_seats",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_bd_ticket_type",
                table: "booking_details",
                column: "ticket_type_id",
                principalTable: "ticket_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bd_event_seat",
                table: "booking_details");

            migrationBuilder.DropForeignKey(
                name: "FK_bd_ticket_type",
                table: "booking_details");

            migrationBuilder.DropIndex(
                name: "IDX_bd_ticket_type_id",
                table: "booking_details");

            migrationBuilder.DropColumn(
                name: "ticket_type_id",
                table: "booking_details");

            migrationBuilder.AlterColumn<int>(
                name: "event_seat_id",
                table: "booking_details",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_bd_event_seat",
                table: "booking_details",
                column: "event_seat_id",
                principalTable: "event_seats",
                principalColumn: "id");
        }
    }
}
