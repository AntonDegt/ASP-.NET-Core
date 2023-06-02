namespace AspNetCore.Services
{
    public class DateService : IDateService
    {
        public DateTime GetDate() => DateTime.Now;
    }
}
