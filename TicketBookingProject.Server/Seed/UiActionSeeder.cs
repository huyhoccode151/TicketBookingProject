using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class UiActionSeeder(TicketBookingProjectContext db, ILogger<UiActionSeeder> logger)
{
    public async Task<List<UiAction>> SeedAsync()
    {
        var existing = await db.UiActions.ToListAsync();
        if (existing.Count > 0)
        {
            logger.LogInformation("[UIActionSeeder] Already seeded ({Count} actions), skipping.", existing.Count);
            return existing;
        }

        await db.UiActions.AddRangeAsync(UiActionFactory.SystemUiActions);
        await db.SaveChangesAsync();

        logger.LogInformation("[UIActionSeeder] Seeded {Count} UI actions.", UiActionFactory.SystemUiActions.Count);
        return await db.UiActions.ToListAsync();
    }
}
