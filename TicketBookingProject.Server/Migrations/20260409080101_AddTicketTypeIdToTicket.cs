using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBookingProject.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketTypeIdToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "event_seat_id",
                table: "tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ticket_type_id",
                table: "tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IDX_tickets_ticket_type_id",
                table: "tickets",
                column: "ticket_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_ticket_type",
                table: "tickets",
                column: "ticket_type_id",
                principalTable: "ticket_types",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_ticket_type",
                table: "tickets");

            migrationBuilder.DropIndex(
                name: "IDX_tickets_ticket_type_id",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "ticket_type_id",
                table: "tickets");

            migrationBuilder.AlterColumn<int>(
                name: "event_seat_id",
                table: "tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
