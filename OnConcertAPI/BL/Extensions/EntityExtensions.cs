using OnConcert.DAL.Entities;

namespace OnConcert.BL.Extensions
{
    public static class EntityExtensions
    {
        public static bool HasStarted(this Event @event) => DateTime.Now >= @event.DateFrom;
        public static bool HasFinished(this Event @event) => DateTime.Now > @event.DateTo;
    }
}