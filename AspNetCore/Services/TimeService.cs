namespace AspNetCore.Services
{
    public class TimeService
    {
        public TimeOnly GetTime() => TimeOnly.FromDateTime(DateTime.Now);
    }
}
