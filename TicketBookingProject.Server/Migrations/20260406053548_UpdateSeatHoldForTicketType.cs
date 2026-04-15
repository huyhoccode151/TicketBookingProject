using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBookingProject.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeatHoldForTicketType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "UQ_sh_event_seat_id",
                table: "seat_holds");

            migrationBuilder.AlterColumn<int>(
                name: "event_seat_id",
                table: "seat_holds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "booking_id",
                table: "seat_holds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                table: "seat_holds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ticket_type_id",
                table: "seat_holds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_seat_holds_ticket_type_id",
                table: "seat_holds",
                column: "ticket_type_id");

            migrationBuilder.CreateIndex(
                name: "UQ_sh_event_seat_id",
                table: "seat_holds",
                column: "event_seat_id",
                unique: true,
                filter: "[event_seat_id] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_seat_holds_ticket_types_ticket_type_id",
                table: "seat_holds",
                column: "ticket_type_id",
                principalTable: "ticket_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_seat_holds_ticket_types_ticket_type_id",
                table: "seat_holds");

            migrationBuilder.DropIndex(
                name: "IX_seat_holds_ticket_type_id",
                table: "seat_holds");

            migrationBuilder.DropIndex(
                name: "UQ_sh_event_seat_id",
                table: "seat_holds");

            migrationBuilder.DropColumn(
                name: "quantity",
                table: "seat_holds");

            migrationBuilder.DropColumn(
                name: "ticket_type_id",
                table: "seat_holds");

            migrationBuilder.AlterColumn<int>(
                name: "event_seat_id",
                table: "seat_holds",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "booking_id",
                table: "seat_holds",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "UQ_sh_event_seat_id",
                table: "seat_holds",
                column: "event_seat_id",
                unique: true);
        }
    }
}
