namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsTransactionDto
    {
        public string? Id { get; set; }

        public string? Source { get; set; }

        public string? Date { get; set; }

        public OcdsValueDto? Value { get; set; }

        public string? Uri { get; set; }

        public OcdsPartyReferenceDto? Payer { get; set; }

        public OcdsPartyReferenceDto? Payee { get; set; }
    }
}
