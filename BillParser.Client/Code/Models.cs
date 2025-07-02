using UglyToad.PdfPig.Content;

namespace BillParser.Client.Code
{
    public record Line
    {
        public required string PhoneNumberPrefix { get; init; }
        public required string PhoneNumberSuffix { get; init; }
        public required decimal PlanAmt { get; init; }
        public decimal? EquipmentAmt { get; init; }
        public decimal? ServicesAmt { get; init; }
        public decimal? OneTimeChargeAmt { get; set; }
        public decimal Total { get; init; }

        private static Line FromWords(List<Word> words)
        {
            return new Line
            {
                PhoneNumberPrefix = words[0].Text,
                PhoneNumberSuffix = words[1].Text,
                PlanAmt = ParseAmount(words[2].Text),
                EquipmentAmt = ParseAmount(words[3].Text),
                ServicesAmt = ParseAmount(words[4].Text),
                OneTimeChargeAmt = ParseAmount(words[5].Text),
                Total = ParseAmount(words[6].Text)
            };
        }
        
        private static decimal ParseAmount(string amt)
        {
            if (amt.ToUpper() is "INCLUDED" or "-")
            {
                amt = "0.00";
            }
            return decimal.Parse(amt.Split("$").Last().ToString());
        }
        
        public static List<Line> From(IEnumerable<Word> words)
        {
            var linesSection = words.SkipWhile(w => !w.Text.ToUpper().Contains('('));
            var numberOfLines = linesSection.Where(w => w.Text.Contains('(')).Count();
            var lineRecordLength = linesSection.Count() / numberOfLines;

            return linesSection
                .Chunk(lineRecordLength)
                .Select(ch => FromWords([.. ch])).ToList();
        }
    }

    public record Totals
    {
        public decimal Plans { get; init; }
        public decimal Equipment { get; init; }
        public decimal Services { get; init; }
        public decimal OneTimeCharges { get; set; }
        public decimal Total { get; init; }

        public static Totals From(IEnumerable<Word> words)
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
                OneTimeCharges = totals[3],
                Total = totals[4]
            };
        }
    }

    public class Bill
    {
        public required Totals Totals { get; init; }
        public required List<Line> LinesList { get; init; }

        public static Bill From(Totals totals, List<Line> lines)
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
                OneTimeChargeAmt = Math.Round((decimal)line.OneTimeChargeAmt!, 2),
                Total = Math.Round((decimal)(planChargeAmt + line!.EquipmentAmt + line!.ServicesAmt + line!.OneTimeChargeAmt)!, 2)
            }));

            return new Bill { Totals = totals, LinesList = newLines };
        }
    }
}
