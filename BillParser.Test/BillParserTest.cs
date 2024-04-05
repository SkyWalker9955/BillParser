using BillParser.Client.Code;

namespace BillParser.Test
{
    public class BillParserTest
    {
        [Fact]
        public void DirectoryReaderGetBillNameTest()
        {
            var result = DirectoryReader.GetBillName("X:\\ProgramFiles\\EclipseIDE\\BillLocation");

            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetSummarySectionTest()
        {
            var path = "X:\\ProgramFiles\\EclipseIDE\\BillLocation";
            var billName = "SummaryBillFeb2024.pdf";
            var result = PdfPig.GetSummarySection(path, billName);

            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetToatalsTest()
        {
            var path = "X:\\ProgramFiles\\EclipseIDE\\BillLocation";
            var billName = "SummaryBillFeb2024.pdf";
            var summarySection = PdfPig.GetSummarySection(path, billName);

            var result = summarySection.GetTotals();

            Assert.True(result.GetType() == typeof(Totals));
        }

        [Fact]
        public void GetLinesTest()
        {
            var path = "X:\\ProgramFiles\\EclipseIDE\\BillLocation";
            var billName = "SummaryBillFeb2024.pdf";
            var summarySection = PdfPig.GetSummarySection(path, billName);

            var result = summarySection.GetLines();
            Assert.True(result.GetType() == typeof(List<Line>));
        }

        [Fact]
        public void GetBillTest()
        {
            var path = "X:\\ProgramFiles\\EclipseIDE\\BillLocation";
            var billName = "SummaryBillFeb2024.pdf";
            var summarySection = PdfPig.GetSummarySection(path, billName);
            var totals = summarySection.GetTotals();
            var lines = summarySection.GetLines();

            var result = PdfPig.GetBill(totals, lines);

            Assert.NotNull(result);
            Assert.Equal(30, result.LinesList.First().PlanAmt);
            Assert.Equal(46.39M, result.LinesList.First().Total);
        }
    }


}