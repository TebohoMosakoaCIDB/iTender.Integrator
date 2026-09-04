namespace iTender.Integrator.Domain.Enums
{
    public enum PartyRole
    {
        None = 0,
        Buyer = 1 << 0,
        ProcuringEntity = 1 << 1,
        Supplier = 1 << 2,
        Tenderer = 1 << 3,
        Payer = 1 << 4,
        Payee = 1 << 5,
        ReviewBody = 1 << 6,
        Enquirer = 1 << 7,
        Funder = 1 << 8
    }
}
