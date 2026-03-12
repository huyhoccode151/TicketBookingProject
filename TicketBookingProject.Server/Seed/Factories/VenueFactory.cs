using Bogus;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public static class VenueFactory
{
    private static readonly string[] Provinces =
    [
        "Hà Nội", "Hồ Chí Minh", "Đà Nẵng", "Hải Phòng",
        "Cần Thơ", "Huế", "Nha Trang", "Vũng Tàu",
    ];

    private static readonly string[] VenueTypes =
    [
        "Nhà hát", "Sân vận động", "Trung tâm Hội nghị",
        "Cung Văn hoá", "Arena", "Amphitheater",
    ];

    private static readonly Faker<Venue> _faker = new Faker<Venue>("vi")
        .RuleFor(v => v.Name, f => $"{f.PickRandom(VenueTypes)} {f.Company.CompanyName()}")
        .RuleFor(v => v.Province, f => f.PickRandom(Provinces))
        .RuleFor(v => v.AddressDetail, f => f.Address.StreetAddress())
        .RuleFor(v => v.Latitude, f => (decimal)f.Address.Latitude(8.5, 23.5))
        .RuleFor(v => v.Longitude, f => (decimal)f.Address.Longitude(102.0, 109.5))
        .RuleFor(v => v.Capacity, f => f.Random.Int(500, 50_000))
        .RuleFor(v => v.CreatedAt, f => f.Date.Past(3).ToUniversalTime())
        .RuleFor(v => v.UpdatedAt, (f, v) => v.CreatedAt);

    public static Venue Create(Action<Venue>? overrides = null)
    {
        var venue = _faker.Generate();
        overrides?.Invoke(venue);
        return venue;
    }

    public static List<Venue> CreateMany(int count, Action<Venue>? overrides = null)
        => Enumerable.Range(0, count).Select(_ => Create(overrides)).ToList();
}
