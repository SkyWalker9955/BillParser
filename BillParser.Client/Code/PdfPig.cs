using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Collections.Generic;
using System.Numerics;

namespace BillParser.Client.Code
{
    public static class PdfPig
    {
        #region Get Sections
        public static Task<IEnumerable<Word>> GetSummarySectionAsync(byte[] bytes)
        {
            if (bytes is null) throw new ArgumentNullException(nameof(bytes));

            using PdfDocument document = PdfDocument.Open(bytes);
            var pageToReturn = 2;

            (string lowerBoudary, string upperBoundary) = ("SUMMARY", "CHARGES");
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
        #endregion

        #region Get Types

        
        public static Totals GetTotals(this IEnumerable<Word> words)
        {
            var totalsSection = words
                .SkipWhile(w => w.Text.ToUpper() != "TOTALS")
                .TakeWhile(w => w.Text.ToUpper() != "ACCOUNT");

            List<decimal> totals = [];

            foreach(var item in totalsSection)
            {
                if (!item.Text.Contains('$')) continue;
                var numberToken = item.Text.Split("$").Last();
                var num = decimal.Parse(numberToken);
                totals.Add(num);
            }
            return new Totals
            {
                Plans = totals[0],
                Equipment = totals[1],
                Services = totals[2],
                Total = totals[3]
            };

        }

        public static List<Line> GetLines(this IEnumerable<Word> words)
        {
            var linesSection = words.SkipWhile(w => !w.Text.ToUpper().Contains('('));
            var numberOfLines = linesSection.Where(w => w.Text.Contains('(')).Count();
            var lineRecordLength = linesSection.Count() / numberOfLines;

            return linesSection
                .Chunk(lineRecordLength)
                .Select(ch => Map([.. ch])).ToList();
        }

        private static Line Map(List<Word> data)
        {
            return new Line
            {
                PhoneNumberPrefix = data[0].Text,
                PhoneNumberSuffix = data[1].Text,
                PlanAmt = data[2].Text.ParseAmount(),
                EquipmentAmt = data[3].Text.ParseAmount(),
                ServicesAmt = data[4].Text.ParseAmount(),
                Total = data[5].Text.ParseAmount()
            };
        }

        private static decimal ParseAmount(this string amt)
        {
            if (amt.ToUpper() is "INCLUDED" or "-")
            {
                amt = new string("0.00");
            }
            return decimal.Parse(amt.Split("$").Last().ToString());
        }

        public static Bill GetBill(Totals totals, List<Line> lines)
        {
            //Get correct charge amt for service
            var planChargeAmt = totals.Plans / lines.Count;
            List<Line> newLines = [];
            newLines.AddRange(lines.Select(line => new Line
            {
                PhoneNumberPrefix = line.PhoneNumberPrefix,
                PhoneNumberSuffix = line.PhoneNumberSuffix,
                PlanAmt = Math.Round(planChargeAmt, 2),
                EquipmentAmt = Math.Round((decimal)line.EquipmentAmt!, 2),
                ServicesAmt = Math.Round((decimal)line.ServicesAmt!, 2),
                Total = Math.Round((decimal)(planChargeAmt + line!.EquipmentAmt + line!.ServicesAmt)!, 2)
            }));

            return new Bill { Totals = totals, LinesList = newLines };
        }
        #endregion
    }
}
