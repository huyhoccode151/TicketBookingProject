namespace TicketBookingProject.Server.Helpers
{
    public static class DateTimeResolve
    {
        public static (DateTime? from, DateTime? to) Resolve(DatePreset? preset)
        {
            var now = DateTime.Now;
            var today = now.Date;

            return preset switch
            {
                DatePreset.Today => (today, EndOfDay(today)),

                DatePreset.Tomorrow => (
                    today.AddDays(1),
                    EndOfDay(today.AddDays(1))
                ),

                DatePreset.ThisWeek => (
                    StartOfWeek(today),
                    EndOfWeek(today)
                ),

                DatePreset.ThisWeekend => GetWeekend(today),

                DatePreset.NextWeekend => GetWeekend(today.AddDays(7)),

                DatePreset.ThisMonth => (
                    StartOfMonth(today),
                    EndOfMonth(today)
                ),

                DatePreset.NextMonth => (
                    StartOfMonth(today.AddMonths(1)),
                    EndOfMonth(today.AddMonths(1))
                ),

                DatePreset.ThisYear => (
                    StartOfYear(today),
                    EndOfYear(today)
                ),

                DatePreset.NextYear => (
                    StartOfYear(today.AddYears(1)),
                    EndOfYear(today.AddYears(1))
                ),

                _ => (null, null)
            };
        }

        private static DateTime EndOfDay(DateTime date) => date.AddDays(1).AddTicks(-1);

        private static DateTime StartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }

        private static DateTime EndOfWeek(DateTime date)
        => StartOfWeek(date).AddDays(7).AddTicks(-1);

        // ===== WEEKEND =====
        private static (DateTime, DateTime) GetWeekend(DateTime date)
        {
            int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)date.DayOfWeek + 7) % 7;
            var saturday = date.AddDays(daysUntilSaturday).Date;
            var sunday = saturday.AddDays(1);

            return (saturday, EndOfDay(sunday));
        }

        // ===== MONTH =====
        private static DateTime StartOfMonth(DateTime date)
            => new DateTime(date.Year, date.Month, 1);

        private static DateTime EndOfMonth(DateTime date)
            => StartOfMonth(date).AddMonths(1).AddTicks(-1);

        // ===== YEAR =====
        private static DateTime StartOfYear(DateTime date)
            => new DateTime(date.Year, 1, 1);

        private static DateTime EndOfYear(DateTime date)
            => StartOfYear(date).AddYears(1).AddTicks(-1);
    }
}
