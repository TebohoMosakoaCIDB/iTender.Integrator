using iTender.Integrator.Domain.Common;
using iTender.Integrator.Domain.Enums;
using iTender.Integrator.Domain.Events;
using iTender.Integrator.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace iTender.Integrator.Domain.Entities
{
    public sealed class Contractor : AggregateRoot<Guid>
    {
        public string CidbRegistrationNumber { get; private set; } = default!;

        public string Name { get; private set; } = default!;

        public Guid? DynamicsAccountId { get; private set; }

        public string? LastMatchedOcdsPartyId { get; private set; }

        private readonly List<CidbGrading> _gradings = new();
        public IReadOnlyCollection<CidbGrading> Gradings => _gradings.AsReadOnly();

        public CidbComplianceStatus ComplianceStatus { get; private set; } = CidbComplianceStatus.NotChecked;

        public DateTime? LastComplianceCheckUtc { get; private set; }

        public DateTime? LastSyncedWithCrmUtc { get; private set; }

        private Contractor()
        {
        }

        public static Contractor Register(string cidbRegistrationNumber, string name, Guid? dynamicsAccountId = null)
        {
            if (string.IsNullOrWhiteSpace(cidbRegistrationNumber))
                throw new ArgumentException("CIDB registration number is required.", nameof(cidbRegistrationNumber));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Contractor name is required.", nameof(name));

            return new Contractor
            {
                Id = Guid.NewGuid(),
                CidbRegistrationNumber = cidbRegistrationNumber.Trim().ToUpperInvariant(),
                Name = name,
                DynamicsAccountId = dynamicsAccountId
            };
        }

        // Reconstructs a Contractor from persisted state (e.g. a Dataverse record), preserving
        // its existing Id. Only repositories should call this - application/domain code that
        // wants a brand new contractor must use Register() so a fresh Id is minted.
        public static Contractor Rehydrate(
            Guid id,
            string cidbRegistrationNumber,
            string name,
            Guid? dynamicsAccountId,
            string? lastMatchedOcdsPartyId,
            CidbComplianceStatus complianceStatus,
            DateTime? lastComplianceCheckUtc,
            DateTime? lastSyncedWithCrmUtc,
            IEnumerable<CidbGrading> gradings)
        {
            var contractor = new Contractor
            {
                Id = id,
                CidbRegistrationNumber = cidbRegistrationNumber,
                Name = name,
                DynamicsAccountId = dynamicsAccountId,
                LastMatchedOcdsPartyId = lastMatchedOcdsPartyId,
                ComplianceStatus = complianceStatus,
                LastComplianceCheckUtc = lastComplianceCheckUtc,
                LastSyncedWithCrmUtc = lastSyncedWithCrmUtc
            };

            foreach (var grading in gradings)
                contractor.AddGrading(grading);

            return contractor;
        }

        public void LinkToDynamicsAccount(Guid dynamicsAccountId)
        {
            DynamicsAccountId = dynamicsAccountId;
            LastSyncedWithCrmUtc = DateTime.UtcNow;
        }

        public void LinkToOcdsParty(string ocdsPartyId) => LastMatchedOcdsPartyId = ocdsPartyId;

        public void AddGrading(CidbGrading grading)
        {
            if (grading is null) throw new ArgumentNullException(nameof(grading));
            _gradings.Add(grading);
        }

        public CidbGrading? CurrentGrading(string classOfWork)
            => _gradings
                .Where(g => g.ClassOfWork.Equals(classOfWork, StringComparison.OrdinalIgnoreCase) && !g.IsExpired(DateTime.UtcNow))
                .OrderByDescending(g => g.PotentialGrade)
                .FirstOrDefault();

        public bool IsEligibleFor(string classOfWork, int requiredGrade, DateTime asOfUtc)
        {
            var grading = _gradings.FirstOrDefault(g => g.ClassOfWork.Equals(classOfWork, StringComparison.OrdinalIgnoreCase));
            return grading is not null && !grading.IsExpired(asOfUtc) && grading.PotentialGrade >= requiredGrade;
        }

        public void SetComplianceStatus(CidbComplianceStatus status)
        {
            ComplianceStatus = status;
            LastComplianceCheckUtc = DateTime.UtcNow;

            if (status == CidbComplianceStatus.NonCompliant
                || status == CidbComplianceStatus.GradingExpired
                || status == CidbComplianceStatus.RegistrationSuspended)
            {
                Raise(new ComplianceViolationDetectedDomainEvent(Id, CidbRegistrationNumber, status, DateTime.UtcNow));
            }
        }

        public void MarkSyncedWithCrm() => LastSyncedWithCrmUtc = DateTime.UtcNow;
    }
}