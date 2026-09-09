namespace iTender.Integrator.Domain.Entities.Csd
{
    public class Association
    {
        public int AssociationID { get; set; }

        public string? SupplierNumberRequestor { get; set; }

        public string? SupplierNumberRequested { get; set; }

        public string? AssociationTypeCode { get; set; }

        public string? AssociationStatusTypeCode { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? EditDate { get; set; }
    }
}
