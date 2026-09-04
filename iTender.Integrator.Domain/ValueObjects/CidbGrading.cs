using iTender.Integrator.Domain.Common;

namespace iTender.Integrator.Domain.ValueObjects
{
    public sealed class CidbGrading : ValueObject
    {
        public string ClassOfWork { get; }

        public int PotentialGrade { get; }

        public DateTime IssueDate { get; }

        public DateTime ExpiryDate { get; }

        private CidbGrading(string classOfWork, int potentialGrade, DateTime issueDate, DateTime expiryDate)
        {
            ClassOfWork = classOfWork;
            PotentialGrade = potentialGrade;
            IssueDate = issueDate;
            ExpiryDate = expiryDate;
        }

        public static CidbGrading Create(string classOfWork, int potentialGrade, DateTime issueDate, DateTime expiryDate)
        {
            if (string.IsNullOrWhiteSpace(classOfWork))
                throw new ArgumentException("Class of work is required.", nameof(classOfWork));

            if (potentialGrade < 1 || potentialGrade > 9)
                throw new ArgumentOutOfRangeException(nameof(potentialGrade), "CIDB potential grade must be between 1 and 9.");

            if (expiryDate <= issueDate)
                throw new ArgumentException("Expiry date must be after issue date.", nameof(expiryDate));

            return new CidbGrading(classOfWork.Trim().ToUpperInvariant(), potentialGrade, issueDate, expiryDate);
        }

        public bool IsExpired(DateTime asOfUtc) => ExpiryDate <= asOfUtc;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return ClassOfWork;
            yield return PotentialGrade;
            yield return IssueDate;
            yield return ExpiryDate;
        }
    }
}
