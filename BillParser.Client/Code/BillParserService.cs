using System.Collections.Concurrent;

namespace BillParser.Client.Code
{
    public interface IBillParserService
    {
        Task<Bill> GenerateBillAsync(byte[] bytes);
    }

    public class BillParserService(IConfiguration config) : IBillParserService
    {
        private readonly IConfiguration config = config;

        public async Task<Bill> GenerateBillAsync(byte[] bytes)
        {
            //Step 1: Get bill data
            var billData = await PdfPig.GetSummarySectionAsync(bytes);

            //Step 2: Get Totals and Get Lines
            var totals = billData.GetTotals();
            var lines = billData.GetLines();

            //Step 3: Get bill
            return PdfPig.GetBill(totals, lines);
        }
    }
}
