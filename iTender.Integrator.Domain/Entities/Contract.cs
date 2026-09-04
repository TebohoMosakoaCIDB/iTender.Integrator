using iTender.Integrator.Domain.Common;
using iTender.Integrator.Domain.Enums;
using iTender.Integrator.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace iTender.Integrator.Domain.Entities
{
    public sealed class Contract : Entity<Guid>
    {
        public string ExternalId { get; private set; } = default!;

        public string? AwardExternalId { get; private set; }

        public string? Title { get; private set; }

        public string? Description { get; private set; }

        public ContractStatus Status { get; private set; }

        public DateRange? Period { get; private set; }

        public Money? Value { get; private set; }

        public DateTime? DateSigned { get; private set; }

        private readonly List<ContractMilestone> _milestones = new();
        public IReadOnlyCollection<ContractMilestone> Milestones => _milestones.AsReadOnly();

        private readonly List<ContractTransaction> _transactions = new();
        public IReadOnlyCollection<ContractTransaction> Transactions => _transactions.AsReadOnly();

        private readonly List<TenderDocument> _documents = new();
        public IReadOnlyCollection<TenderDocument> Documents => _documents.AsReadOnly();

        private Contract()
        {
        }

        public static Contract Create(
            string externalId,
            string? awardExternalId,
            string? title,
            string? description,
            ContractStatus status,
            DateRange? period,
            Money? value,
            DateTime? dateSigned)
        {
            if (string.IsNullOrWhiteSpace(externalId))
                throw new ArgumentException("Contract id is required.", nameof(externalId));

            return new Contract
            {
                Id = Guid.NewGuid(),
                ExternalId = externalId,
                AwardExternalId = awardExternalId,
                Title = title,
                Description = description,
                Status = status,
                Period = period,
                Value = value,
                DateSigned = dateSigned
            };
        }

        public void AddMilestone(ContractMilestone milestone) => _milestones.Add(milestone ?? throw new ArgumentNullException(nameof(milestone)));

        public void AddTransaction(ContractTransaction transaction) => _transactions.Add(transaction ?? throw new ArgumentNullException(nameof(transaction)));

        public void AddDocument(TenderDocument document) => _documents.Add(document ?? throw new ArgumentNullException(nameof(document)));

        // Point #5 - audit/monitoring: projects slipping behind schedule.
        public bool HasOverdueMilestones(DateTime asOfUtc)
            => _milestones.Any(m => m.Status != MilestoneStatus.Met && m.DueDate.HasValue && m.DueDate.Value < asOfUtc);

        // Point #5 - a contract with no linked award is a "ghost project" signal.
        public bool HasNoLinkedAward() => string.IsNullOrWhiteSpace(AwardExternalId);
    } 
}
