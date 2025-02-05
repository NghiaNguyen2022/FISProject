namespace PN.SmartLib.Helper
{
    public class NumberToWordVN
    {
        static string[] ones = { "", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        static string[] teens = { "mười", "mười một", "mười hai", "mười ba", "mười bốn", "mười lăm", "mười sáu", "mười bảy", "mười tám", "mười chín" };
        static string[] tens = { "", "mười", "hai mươi", "ba mươi", "bốn mươi", "năm mươi", "sáu mươi", "bảy mươi", "tám mươi", "chín mươi" };
        static string[] thousands = { "", "nghìn", "triệu", "tỷ" };
        public static string NumberToWordsInVietnamese(long number)
        {
            if (number == 0)
                return "Không";

            int index = 0;
            string words = "";

            do
            {
                long chunk = number % 1000;
                if (chunk != 0)
                {
                    words = $"{ConvertChunkToWords(chunk)} {thousands[index]} {words}";
                }
                index++;
                number /= 1000;
            } while (number > 0);

            var charArray = words.Trim().ToCharArray();
            var resultString = words;
            charArray[0] = char.ToUpper(charArray[0]);
            resultString = new string(charArray);
            return resultString.Trim();
        }

        public static string ConvertChunkToWords(long number)
        {
            string chunkWords = "";

            // Extract hundreds, tens, and ones
            long hundreds = number / 100;
            long remainder = number % 100;

            if (hundreds > 0)
            {
                chunkWords += $"{ones[hundreds]} trăm ";
            }

            if (remainder > 0)
            {
                if (remainder < 10)
                {
                    if (hundreds > 0)
                        chunkWords += $"lẻ {ones[remainder]}";
                    else chunkWords += ones[remainder];
                }
                else if (remainder < 20)
                {
                    chunkWords += teens[remainder - 10];
                }
                else
                {
                    long tensDigit = remainder / 10;
                    long onesDigit = remainder % 10;
                    chunkWords += $"{tens[tensDigit]} {ones[onesDigit]}";
                }
            }

            return chunkWords.Trim();
        }

        //private static readonly string[] ZeroLeftPadding = {"", "00", "0"};

        //private static readonly string[] Digits =
        //{
        //    "không",
        //    "một",
        //    "hai",
        //    "ba",
        //    "bốn",
        //    "năm",
        //    "sáu",
        //    "bảy",
        //    "tám",
        //    "chín"
        //};

        //private static readonly string[] MultipleThousand =
        //{
        //    "",
        //    "nghìn",
        //    "triệu",
        //    "tỷ",
        //    "nghìn tỷ",
        //    "triệu tỷ",
        //    "tỷ tỷ"
        //};

        //private static IEnumerable<string> Chunked(this string str, int chunkSize) => Enumerable
        //    .Range(0, str.Length / chunkSize)
        //    .Select(i => str.Substring(i * chunkSize, chunkSize));

        //private static bool ShouldShowZeroHundred(this string[] groups) =>
        //    groups.Reverse().TakeWhile(it => it == "000").Count() < groups.Count() - 1;

        //private static void Deconstruct<T>(this IReadOnlyList<T> items, out T t0, out T t1, out T t2)
        //{
        //    t0 = items.Count > 0 ? items[0] : default;
        //    t1 = items.Count > 1 ? items[1] : default;
        //    t2 = items.Count > 2 ? items[2] : default;
        //}

        //private static string ReadTriple(string triple, bool showZeroHundred)
        //{
        //    var (a, b, c) = triple.Select(ch => int.Parse(ch.ToString())).ToArray();

        //    return a switch
        //    {
        //        0 when b == 0 && c == 0 => "",
        //        0 when showZeroHundred => "không trăm " + ReadPair(b, c),
        //        0 when b == 0 => Digits[c],
        //        0 => ReadPair(b, c),
        //        _ => Digits[a] + " trăm " + ReadPair(b, c)
        //    };
        //}

        //private static string ReadPair(int b, int c)
        //{
        //    return b switch
        //    {
        //        0 => c == 0 ? "" : " lẻ " + Digits[c],
        //        1 => "mười " + c switch
        //        {
        //            0 => "",
        //            5 => "lăm",
        //            _ => Digits[c]
        //        },
        //        _ => Digits[b] + " mươi " + c switch
        //        {
        //            0 => "",
        //            1 => "mốt",
        //            4 => "tư",
        //            5 => "lăm",
        //            _ => Digits[c]
        //        }
        //    };
        //}

        //private static string Capitalize(this string input)
        //{
        //    return input switch
        //    {
        //        null => throw new ArgumentNullException(nameof(input)),
        //        "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
        //        _ => input.First().ToString().ToUpper() + input.Substring(1).ToLower()
        //    };
        //}

        //public static string ToVietnameseWords(this long n)
        //{
        //    if (n == 0L) return "Không";
        //    if (n < 0L) return "Âm " + (-n).ToVietnameseWords().ToLower();

        //    var s = n.ToString();
        //    var groups = (ZeroLeftPadding[s.Length % 3] + s).Chunked(3).ToArray();
        //    var showZeroHundred = groups.ShouldShowZeroHundred();

        //    var index = -1;
        //    var rawResult = groups.Aggregate("", (acc, e) =>
        //    {
        //        checked
        //        {
        //            index++;
        //        }

        //        var readTriple = ReadTriple(e, showZeroHundred && index > 0);
        //        var multipleThousand = (string.IsNullOrWhiteSpace(readTriple)
        //            ? ""
        //            : (MultipleThousand.ElementAtOrDefault(groups.Length - 1 - index) ?? ""));
        //        return $"{acc} {readTriple} {multipleThousand} ";
        //    });

        //    return Regex
        //        .Replace(rawResult, "\\s+", " ")
        //        .Trim()
        //        .Capitalize();
        //}
    }
}
