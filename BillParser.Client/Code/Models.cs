namespace BillParser.Client.Code
{
    public record Line
    {
        public required string PhoneNumberPrefix { get; set; }
        public required string PhoneNumberSuffix { get; set; }
        public required decimal PlanAmt { get; set; }
        public decimal? EquipmentAmt { get; set; }
        public decimal? ServicesAmt { get; set; }
        public decimal Total { get; set; }
    }

    public record Totals
    {
        public decimal Plans { get; set; }
        public decimal Equipment { get; set; }
        public decimal Services { get; set; }
        public decimal Total { get; set; }
    }

    public class Bill
    {
        public required Totals Totals { get; set; }
        public required List<Line> LinesList { get; set; }
    }
}
