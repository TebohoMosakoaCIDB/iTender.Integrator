namespace iTender.Integrator.Application.DTOs.eTenders
{
    public sealed record CreateTenderResult(
        bool EtendersSuccess,
        string? EtendersMessage,
        int? EtendersTenderId,
        string? EtendersTenderNumber,
        bool PublishedToCrm,
        Guid? CrmTenderId,
        string? CrmPublishReason);
}
