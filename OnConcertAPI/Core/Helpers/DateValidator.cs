namespace OnConcert.Core.Helpers
{
    public static class DateValidator
    {
        public static bool IsValidDateRange(DateTime dateFrom, DateTime dateTo) =>
            dateFrom >= DateTime.Now && dateFrom < dateTo;

        public static bool DateRangesOverlap(DateTime startFirstDate, DateTime endFirstDate, DateTime startSecondDate, DateTime endSecondDate) =>
            startFirstDate <= endSecondDate && startSecondDate <= endFirstDate;
    }
}