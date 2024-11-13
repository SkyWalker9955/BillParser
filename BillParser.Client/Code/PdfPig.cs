using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Collections.Generic;
using System.Numerics;

namespace BillParser.Client.Code
{
    public static class PdfPig
    {
        public static Task<IEnumerable<Word>> GetSummarySectionAsync(byte[] bytes)
        {
            if (bytes is null) throw new ArgumentNullException(nameof(bytes));

            using var document = PdfDocument.Open(bytes);
            const int pageToReturn = 2;

            var (lowerBoudary, upperBoundary) = ("SUMMARY", "DETAILED");
            string[] wordsToRemove = [
                "SUMMARY",
                "LINE",
                "TYPE",
                "EQUIPMENT",
                "SERVICES",
                "TOTAL",
                "VOICE",
                "DETAILED",
                "CHARGES"
            ];

            return Task.FromResult(document
                .GetPage(pageToReturn)
                .GetWords()
                .GetSummary(lowerBoudary, upperBoundary)
                .CleanData(wordsToRemove));
        }

        private static IEnumerable<Word> GetSummary(this IEnumerable<Word> words, string lowerBoudary, string upperBoundary)
        {
            return words
                .SkipWhile(w => w.Text.ToUpper() != lowerBoudary)
                .TakeWhile(w => w.Text.ToUpper() != upperBoundary);
        }
        private static IEnumerable<Word> CleanData(this IEnumerable<Word> summary, string[] wordsToRemove)
        {
            return summary.Where(w => !wordsToRemove.Contains(w.Text.ToUpper()));
        }
    }
}
