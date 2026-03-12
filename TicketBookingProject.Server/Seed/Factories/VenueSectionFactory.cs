using Bogus;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public static class VenueSectionFactory
{
    private static readonly string[] SectionNames =
    [
        "Khu A", "Khu B", "Khu C", "Khu VIP",
        "Khu Standing", "Tầng 1", "Tầng 2",
        "Khán đài Đông", "Khán đài Tây", "Khán đài Nam", "Khán đài Bắc",
        "Golden Circle", "Platinum", "Diamond",
    ];

    private static readonly Faker _faker = new("vi");

    public static List<VenueSection> CreateForVenue(int venueId, int count = 4)
    {
        var usedNames = new HashSet<string>();
        var result = new List<VenueSection>();

        while (result.Count < count)
        {
            var name = _faker.PickRandom(SectionNames);
            if (!usedNames.Add(name)) continue;

            result.Add(new VenueSection
            {
                VenueId = venueId,
                Name = name,
                Capacity = _faker.Random.Int(50, 5_000),
                CreatedAt = DateTime.UtcNow,
            });
        }

        return result;
    }
}
