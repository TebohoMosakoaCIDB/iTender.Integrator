using iTender.Integrator.Domain.Common;
using iTender.Integrator.Domain.Enums;

namespace iTender.Integrator.Domain.Entities
{
    public sealed class ContractMilestone : Entity<Guid>
    {
        public string ExternalId { get; private set; } = default!;

        public string? Title { get; private set; }

        public string? Type { get; private set; }

        public MilestoneStatus Status { get; private set; }

        public DateTime? DueDate { get; private set; }

        public DateTime? DateMet { get; private set; }

        private ContractMilestone()
        {
        }

        public static ContractMilestone Create(string externalId, string? title, string? type, MilestoneStatus status, DateTime? dueDate, DateTime? dateMet)
        {
            if (string.IsNullOrWhiteSpace(externalId))
                throw new ArgumentException("Milestone id is required.", nameof(externalId));

            return new ContractMilestone
            {
                Id = Guid.NewGuid(),
                ExternalId = externalId,
                Title = title,
                Type = type,
                Status = status,
                DueDate = dueDate,
                DateMet = dateMet
            };
        }
    }
}
