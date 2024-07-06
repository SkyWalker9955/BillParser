using System.Diagnostics;
using BillParser.Client.Code;
using Xunit.Abstractions;

namespace BillParser.Test
{
    public class BillParserTest(ITestOutputHelper testOutputHelper)
    {
        [Fact]
        public void DirectoryReaderGetBillNameTest()
        {
            var result = DirectoryReader.GetBillName("/home/mike/Documents/PhoneBill");
            testOutputHelper.WriteLine(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetSummarySectionTest()
        {
            var path = "/home/mike/Documents/PhoneBill";
            var billName = "SummaryBillJun2024.pdf";
            var str = new MemoryStream(File.ReadAllBytes(Path.Combine(path, billName)));
            var result = await PdfPig.GetSummarySectionAsync(File.ReadAllBytes(Path.Combine(path, billName)));

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetToatalsTest()
        {
            var path = "/home/mike/Documents/PhoneBill";
            var billName = "SummaryBillJun2024.pdf";
            var summarySection = await PdfPig.GetSummarySectionAsync(File.ReadAllBytes(Path.Combine(path, billName)));

            var result = summarySection.GetTotals();

            Assert.True(result.GetType() == typeof(Totals));
        }

        [Fact]
        public async Task GetLinesTest()
        {
            var path = "/home/mike/Documents/PhoneBill";
            var billName = "SummaryBillJun2024.pdf";
            var summarySection = await PdfPig.GetSummarySectionAsync(File.ReadAllBytes(Path.Combine(path, billName)));

            var result = summarySection.GetLines();
            Assert.True(result.GetType() == typeof(List<Line>));
        }

        [Fact]
        public async Task GetBillTest()
        {
            var path = "/home/mike/Documents/PhoneBill";
            var billName = "SummaryBillJun2024.pdf";
            var summarySection = await PdfPig.GetSummarySectionAsync(File.ReadAllBytes(Path.Combine(path, billName)));
            var totals = summarySection.GetTotals();
            var lines = summarySection.GetLines();

            var result = PdfPig.GetBill(totals, lines);

            Assert.NotNull(result);
            Assert.Equal(30, result.LinesList.First().PlanAmt);
            Assert.Equal(46.34M, result.LinesList.First().Total);
        }
    }
}