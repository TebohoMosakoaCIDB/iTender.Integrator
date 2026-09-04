using iTender.Integrator.Domain.Common;

namespace iTender.Integrator.Domain.ValueObjects
{
    public sealed class BriefingSession : ValueObject
    {
        public bool IsSession { get; }

        public bool Compulsory { get; }

        public DateTime? Date { get; }

        public string? Venue { get; }

        private BriefingSession(bool isSession, bool compulsory, DateTime? date, string? venue)
        {
            IsSession = isSession;
            Compulsory = compulsory;
            Date = date;
            Venue = venue;
        }

        public static BriefingSession Create(bool isSession, bool compulsory, DateTime? date, string? venue)
            => new(isSession, compulsory, date, venue);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return IsSession;
            yield return Compulsory;
            yield return Date;
            yield return Venue;
        }
    }
}
