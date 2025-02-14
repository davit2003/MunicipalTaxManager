namespace MunicipalTaxManager.Models
{
    public class TaxRecord
    {
        public int Id { get; set; }
        public string Municipality { get; set; } = null!;
        public decimal Rate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
