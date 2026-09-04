using iTender.Integrator.Domain.Common;

namespace iTender.Integrator.Domain.ValueObjects
{
    public sealed class DateRange : ValueObject
    {
        public DateTime? StartDate { get; }

        public DateTime? EndDate { get; }

        public DateTime? MaxExtentDate { get; }

        public int? DurationInDays { get; }

        private DateRange(DateTime? startDate, DateTime? endDate, DateTime? maxExtentDate, int? durationInDays)
        {
            if (startDate.HasValue && endDate.HasValue && endDate < startDate)
                throw new ArgumentException("End date cannot be before start date.");

            StartDate = startDate;
            EndDate = endDate;
            MaxExtentDate = maxExtentDate;
            DurationInDays = durationInDays;
        }

        public static DateRange Create(DateTime? startDate, DateTime? endDate, DateTime? maxExtentDate = null, int? durationInDays = null)
            => new(startDate, endDate, maxExtentDate, durationInDays);

        public bool IsOpenNow(DateTime asOfUtc)
            => StartDate.HasValue && EndDate.HasValue && asOfUtc >= StartDate.Value && asOfUtc <= EndDate.Value;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return StartDate;
            yield return EndDate;
            yield return MaxExtentDate;
            yield return DurationInDays;
        }
    }
}
