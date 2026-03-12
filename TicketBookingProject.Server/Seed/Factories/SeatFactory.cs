using Bogus;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public static class SeatFactory
{
    private static readonly Faker _faker = new("vi");

    /// <summary>
    /// Generates a full row × column grid of seats for a section.
    /// </summary>
    public static List<Seat> CreateGrid(
        int venueId,
        int sectionId,
        int rows,
        int seatsPerRow,
        SeatType seatType = SeatType.Normal)
    {
        var seats = new List<Seat>();

        for (int r = 0; r < rows; r++)
        {
            var row = ((char)('A' + r)).ToString();
            for (int num = 1; num <= seatsPerRow; num++)
            {
                seats.Add(new Seat
                {
                    VenueId = venueId,
                    SectionId = sectionId,
                    Row = row,
                    SeatNumber = num.ToString(),
                    SeatType = seatType,
                    CreatedAt = DateTime.UtcNow,
                });
            }
        }

        return seats;
    }

    /// <summary>
    /// Creates random seats for standing / mixed sections.
    /// </summary>
    public static List<Seat> CreateRandom(int venueId, int sectionId, int count,
        SeatType seatType = SeatType.Standing)
    {
        var used = new HashSet<(string, string)>();
        var result = new List<Seat>();

        while (result.Count < count)
        {
            var row = ((char)_faker.Random.Int('A', 'Z')).ToString();
            var num = _faker.Random.Int(1, 100).ToString();

            if (!used.Add((row, num))) continue;

            result.Add(new Seat
            {
                VenueId = venueId,
                SectionId = sectionId,
                Row = row,
                SeatNumber = num,
                SeatType = seatType,
                CreatedAt = DateTime.UtcNow,
            });
        }

        return result;
    }
}