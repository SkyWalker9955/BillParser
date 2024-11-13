using System.Collections.Concurrent;

namespace BillParser.Client.Code
{
    public interface IBillParserService
    {
        Task<Bill> GenerateBillAsync(byte[] bytes);
    }

    public class BillParserService(IConfiguration config) : IBillParserService
    {
        private readonly IConfiguration _config = config;

        public async Task<Bill> GenerateBillAsync(byte[] bytes)
        {
            //Step 1: Get bill data
            var billData = await PdfPig.GetSummarySectionAsync(bytes);

            //Step 2: Get Totals and Get Lines
            var totals = Totals.From(billData);
            var lines = Line.From(billData);

            //Step 3: Get bill
            return Bill.From(totals, lines);
        }
    }
}
