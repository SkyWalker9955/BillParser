using BillParser.Client.Code;
using Xunit.Abstractions;

namespace BillParser.Test
{
    public class BillParserTest(ITestOutputHelper testOutputHelper)
    {
        [Fact]
        public void DirectoryReaderGetBillNameTest()
        {
            var result = DirectoryReader.GetBillName(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\SummaryBillOct2024.pdf");
            testOutputHelper.WriteLine(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetSummarySectionTest()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var billName = "SummaryBillOct2024.pdf";
            var str = new MemoryStream(await File.ReadAllBytesAsync(Path.Combine(path, billName)));
            var result = await PdfPig.GetSummarySectionAsync(await File.ReadAllBytesAsync(Path.Combine(path, billName)));

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetTotalsTest()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var billName = "SummaryBillOct2024.pdf";
            var summarySection = await PdfPig.GetSummarySectionAsync(await File.ReadAllBytesAsync(Path.Combine(path, billName)));

            var result = Totals.From(summarySection);

            Assert.True(result.GetType() == typeof(Totals));
        }

        [Fact]
        public async Task GetLinesTest()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var billName = "SummaryBillOct2024.pdf";
            var summarySection = await PdfPig.GetSummarySectionAsync(await File.ReadAllBytesAsync(Path.Combine(path, billName)));

            var result = Line.From(summarySection);
            Assert.True(result.GetType() == typeof(List<Line>));
        }

        [Fact]
        public async Task GetBillTest()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SummaryBillOct2024.pdf");
            var summarySection = await PdfPig.GetSummarySectionAsync(await File.ReadAllBytesAsync(path));
            var totals = Totals.From(summarySection);
            var lines = Line.From(summarySection);

            var result = Bill.From(totals, lines);

            Assert.NotNull(result);
            Assert.Equal(30, result.LinesList.First().PlanAmt);
            Assert.Equal(46.34M, result.LinesList.First().Total);
        }
    }
}