using System.Collections.Concurrent;

namespace BillParser.Client.Code
{
    public interface IBillParserService
    {
        Bill GenerateBill();
    }

    public class BillParserService(IConfiguration config) : IBillParserService
    {
        private readonly IConfiguration config = config;

        public Bill GenerateBill()
        {
            var billLocation = config.GetValue<string>("Settings:BillLocation");
            if (billLocation is null) throw new ArgumentException($"{nameof(billLocation)} is null");

            //Step 0: Get bill name
            var billName = DirectoryReader.GetBillName(billLocation);

            //Step 1: Get bill data
            var billData = PdfPig.GetSummarySection(billLocation, billName);

            //Step 2: Get Totals and Get Lines
            var totals = billData.GetTotals();
            var lines = billData.GetLines();

            //Step 3: Get bill
            return PdfPig.GetBill(totals, lines);
        }
    }
}
