
namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dateTime)
        {
            var age = DateTime.Today.Year - dateTime.Year;
            if (DateTime.Today < dateTime.AddYears(age))
            {
                age--;
            }
            return age;
        }
    }
}