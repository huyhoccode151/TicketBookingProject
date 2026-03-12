using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public static class CategoryFactory
{
    public static readonly List<Category> SystemCategories =
    [
        new() { Name = "Âm nhạc",   Description = "Concert, live show, nhạc hội"              },
        new() { Name = "Thể thao",  Description = "Giải đấu thể thao trong và ngoài nước"     },
        new() { Name = "Sân khấu",  Description = "Kịch, cải lương, múa, ballet"              },
        new() { Name = "Hội thảo",  Description = "Conference, seminar, workshop chuyên ngành" },
        new() { Name = "Triển lãm", Description = "Triển lãm nghệ thuật, công nghệ, thương mại"},
        new() { Name = "Lễ hội",    Description = "Lễ hội truyền thống và hiện đại"            },
        new() { Name = "Điện ảnh",  Description = "Chiếu phim đặc biệt, liên hoan phim"       },
        new() { Name = "Giải trí",  Description = "Game show, talkshow, event giải trí"        },
        new() { Name = "Giáo dục",  Description = "Khóa học, bootcamp, training"               },
        new() { Name = "Ẩm thực",   Description = "Food festival, wine tasting, culinary event"},
    ];
}