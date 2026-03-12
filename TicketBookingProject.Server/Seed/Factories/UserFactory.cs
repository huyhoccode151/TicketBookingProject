using Bogus;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public static class UserFactory
{
    private static readonly Faker<User> _faker = new Faker<User>("vi")
        .RuleFor(u => u.Username, f => f.Internet.UserName())
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Firstname, f => f.Name.FirstName())
        .RuleFor(u => u.Lastname, f => f.Name.LastName())
        .RuleFor(u => u.Password, f => BCrypt.Net.BCrypt.HashPassword("password"))
        .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
        .RuleFor(u => u.Status, f => UserStatus.Active)
        .RuleFor(u => u.LoginType, f => f.PickRandom<LoginType>())
        .RuleFor(u => u.EmailVerifiedAt, f => f.Date.Past(1).ToUniversalTime())
        .RuleFor(u => u.CreatedAt, f => f.Date.Past(2).ToUniversalTime())
        .RuleFor(u => u.UpdatedAt, (f, u) => u.CreatedAt)
        .RuleFor(u => u.DeletedAt, f => null);

    public static User Create(Action<User>? overrides = null)
    {
        var user = _faker.Generate();
        overrides?.Invoke(user);
        return user;
    }

    public static List<User> CreateMany(int count, Action<User>? overrides = null)
        => Enumerable.Range(0, count).Select(_ => Create(overrides)).ToList();

    public static User CreateAdmin() => Create(u =>
    {
        u.Username = "admin";
        u.Email = "admin@ticketbooking.vn";
        u.Status = UserStatus.Active;
        u.LoginType = LoginType.Email;
    });

    public static User CreateOrganizer() => Create(u =>
    {
        u.Status = UserStatus.Active;
        u.LoginType = LoginType.Email;
    });

    public static User CreateStaff() => Create(u =>
    {
        u.Status = UserStatus.Active;
        u.LoginType = LoginType.Email;
    });

    public static User CreateCustomer() => Create(u =>
    {
        u.Status = UserStatus.Active;
    });
}