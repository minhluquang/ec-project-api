public static class DateOnlyExtensions
{
    private static readonly TimeZoneInfo VietNamTz =
        TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");     
        public static DateTime ToUtcStartOfDay(this DateOnly date)
    {
        var local = date.ToDateTime(TimeOnly.MinValue); 
        return TimeZoneInfo.ConvertTimeToUtc(local, VietNamTz);
    }
    public static DateTime ToUtcEndOfDay(this DateOnly date)
    {
        var local = date.ToDateTime(new TimeOnly(23, 59, 59, 999));
        return TimeZoneInfo.ConvertTimeToUtc(local, VietNamTz);
    }
}