namespace iTender.Integrator.Domain.Entities.Csd
{
    public class CommodityGroup
    {
        public int CommodityGroupID { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool NationWide { get; set; }

        public bool ProvinceWide { get; set; }

        public bool IsActive { get; set; }

        public List<CommodityItem> CommodityItems { get; set; } = [];

        public string? CommodityLocations { get; set; }

        public string? CommodityProvinces { get; set; }

        public string? Field1 { get; set; }

        public string? Field2 { get; set; }

        public string? Field3 { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? EditDate { get; set; }
    }
}
