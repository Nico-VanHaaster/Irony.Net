using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irony.Samples.InboundSMS
{

    public class TimeTerminal : CompoundTerminalBase
    {
        string formatString = "HH:mm";
        string seperators = "";
        public TimeTerminal(string name)
            : this(name, ":,.")
        {

        }

        public TimeTerminal(string name, string seperators)
            : base(name)
        {
            this.seperators = seperators;
        }

        /// <summary>
        /// the start of a time can only be 0, 1, 2 (24hour clock goes to 23)
        /// </summary>
        /// <returns></returns>
        public override IList<string> GetFirsts()
        {
            StringList result = new StringList();
            result.AddRange(new string[] { "0", "1", "2" });
            return result;
        }

        public override void Init(GrammarData grammarData)
        {
            base.Init(grammarData);
            this.EditorInfo = new TokenEditorInfo(TokenType.String, TokenColor.String, TokenTriggers.None);
        }


        protected override bool ConvertValue(CompoundTerminalBase.CompoundTokenDetails details)
        {
            if (String.IsNullOrEmpty(details.Body) || string.IsNullOrWhiteSpace(this.formatString))
            {
                details.Error = "Invalid time";  // "Invalid number.";
                return false;
            }

            string body = details.Body;

            DateTime parseDate = DateTime.Now;
            if (!DateTime.TryParseExact(body, this.formatString, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out parseDate))
            {
                details.Error = "Invalid time";
                return false;
            }
            details.Value = parseDate.ToString("HH:mm:ss");
            return true;
        }

        protected override bool ReadBody(ISourceStream source, CompoundTokenDetails details)
        {
            if (string.IsNullOrWhiteSpace(this.seperators))
                return false;
            int start = source.PreviewPosition;
            bool foundDigits = false;
            string seperator = "";
            bool spaceFound = false;
            string digits = Strings.DecimalDigits;
            while (!source.EOF())
            {
                char current = source.PreviewChar;
                //1. It is a digit... (good)
                if (digits.IndexOf(current) >= 0)
                {
                    source.PreviewPosition++;
                    foundDigits = true;
                    continue;
                }
                else if (' ' == current && !spaceFound)
                {
                    source.PreviewPosition++;
                    spaceFound = true;
                    continue;
                }
                //could be am or pm
                else if ("APap".IndexOf(current) >= 0)
                {
                    //advance forward
                    source.PreviewPosition++;
                    //at eof... we are done
                    if (source.EOF())
                        return false;
                    //reset current
                    current = source.PreviewChar;

                    //is it an m? Yeah?
                    if ("Mm".IndexOf(current) >= 0)
                    {
                        source.PreviewPosition++;
                        continue;
                    }
                    return false;
                }
                //2. Its a seperator.. continue but dont set digits
                else if (seperators.IndexOf(current) >= 0 && (seperator == "" || seperator[0] == current))
                {
                    seperator = "" + current;
                    source.PreviewPosition++;
                    continue;
                }

                break;
            }

            int end = source.PreviewPosition;
            if (!foundDigits || seperator == "")
                return false;
            string parseString = source.Text.Substring(start, end - start);
            bool hour, min, sec, tt, twentyFour;
            hour = min = sec = tt = twentyFour = false;

            StringList formats = new StringList();



            string[] parts = parseString.Split(new string[] { seperator }, StringSplitOptions.RemoveEmptyEntries);
            //parts must be between 2 and 3 pieces. 2 parts matches HH:mm (or varients) 3 matches HH:mm:ss (or varients)
            if (parts.Length < 2 || parts.Length > 3)
                return false;
            for (int i = 0; i < parts.Length; i++)
            {
                var datePart = parts[i];
                //the only one that can contain a space is 
                if (datePart.Contains(" "))
                {
                    var pmParts = datePart.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    //this length should always be 2
                    if (pmParts.Length != 2)
                        return false;

                    if (pmParts[1].ToLower() != "pm" && pmParts[1].ToLower() != "am")
                        return false;

                    if (tt)
                        return false;

                    if (!min)
                    {
                        min = true;
                        tt = true;
                        if (!twentyFour)
                            formats.Add("mm tt");
                        else
                            formats.Add("mm");
                        continue;
                    }
                    else if (!sec)
                    {
                        sec = true;
                        tt = true;
                        if (!twentyFour)
                            formats.Add("ss tt");
                        else
                            formats.Add("ss");
                        continue;
                    }
                    //invalid time
                    else
                        return false;

                }

                else if (datePart.Length == 1)
                {
                    if (!hour)
                    {
                        hour = true;
                        formats.Add("H");
                        if (datePart == "0")
                            twentyFour = true;
                        else
                            twentyFour = false;

                        continue;
                    }
                    else if (!min)
                    {
                        min = true;
                        formats.Add("m");
                        continue;
                    }
                    else if (!sec)
                    {
                        sec = true;
                        formats.Add("s");
                        continue;
                    }
                }
                else if (datePart.Length == 2)
                {
                    if (!hour)
                    {
                        hour = true;
                        int hourNum = int.Parse(datePart);
                        if (hourNum > 12)
                            twentyFour = true;
                        else
                            twentyFour = false;
                        formats.Add("HH");
                        continue;
                    }
                    else if (!min)
                    {
                        min = true;
                        formats.Add("mm");
                        continue;
                    }
                    else if (!sec)
                    {
                        sec = true;
                        formats.Add("ss");
                        continue;
                    }
                    else
                        return false;
                }


            }
            if (twentyFour && tt)
                parseString = parseString.ToLower().Replace(" am", "").Replace(" pm", "");

            this.formatString = formats.ToString(seperator);
            DateTime parseDate = DateTime.Now;
            if (!DateTime.TryParseExact(parseString, this.formatString, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out parseDate))
                return false;

            details.Body = parseString;


            return true;
        }
    }

    public class DateTerminal : CompoundTerminalBase
    {
        string seperators = "";
        string formatString = "";

        public DateTerminal(string name)
            : this(name, "/,.-")
        {

        }

        public DateTerminal(string name, string seperators)
            : base(name)
        {
            this.seperators = seperators;
            this.ValidateToken += DateTerminal_ValidateToken;
        }

        void DateTerminal_ValidateToken(object sender, ValidateTokenEventArgs e)
        {

        }

        public override IList<string> GetFirsts()
        {
            StringList result = new StringList();
            result.AddRange(new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            return result;
        }

        public override void Init(GrammarData grammarData)
        {
            base.Init(grammarData);
            this.EditorInfo = new TokenEditorInfo(TokenType.String, TokenColor.String, TokenTriggers.None);
        }


        protected override bool ConvertValue(CompoundTerminalBase.CompoundTokenDetails details)
        {
            if (String.IsNullOrEmpty(details.Body) || string.IsNullOrWhiteSpace(this.formatString))
            {
                details.Error = "Invalid date";  // "Invalid number.";
                return false;
            }

            string body = details.Body;

            DateTime parseDate = DateTime.Now;
            if (!DateTime.TryParseExact(body, this.formatString, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out parseDate))
            {
                details.Error = "Invalid date";
                return false;
            }
            details.Value = parseDate.ToString("yyyy-MM-dd");
            return true;
        }


        protected override bool ReadBody(ISourceStream source, CompoundTokenDetails details)
        {
            if (string.IsNullOrWhiteSpace(this.seperators))
                return false;
            int start = source.PreviewPosition;
            bool foundDigits = false;
            string seperator = "";
            string digits = Strings.DecimalDigits;
            while (!source.EOF())
            {
                char current = source.PreviewChar;
                //1. It is a digit... (good)
                if (digits.IndexOf(current) >= 0)
                {
                    source.PreviewPosition++;
                    foundDigits = true;
                    continue;
                }
                //2. Its a seperator.. continue but dont set digits
                else if (seperators.IndexOf(current) >= 0 && (seperator == "" || seperator[0] == current))
                {
                    seperator = "" + current;
                    source.PreviewPosition++;
                    continue;
                }

                break;
            }

            int end = source.PreviewPosition;
            if (!foundDigits || seperator == "")
                return false;
            string parseString = source.Text.Substring(start, end - start);
            bool day, month, year;
            day = month = year = false;

            StringList formats = new StringList();

            string[] parts = parseString.Split(new string[] { seperator }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
                return false;
            for (int i = 0; i < parts.Length; i++)
            {
                var datePart = parts[i];
                if (datePart.Length == 2)
                {
                    //could be a month or a day.. months can either start with a zero or a one
                    if ((datePart[0] == '0' || datePart[0] == '1') && !month)
                    {
                        //if month hasnt been set then we assume its a month (even if it wasnt intended)
                        month = true;
                        formats.Add("MM");
                        continue;
                    }

                    if (!day)
                    {
                        day = true;
                        formats.Add("dd");
                        continue;
                    }
                    //else try a year
                    else if (!year)
                    {
                        year = true;
                        formats.Add("yy");
                    }
                    //otherwise this is a bad date
                    else
                        return false;

                }
                //trickier.. this could be a day or month.. we are going to assume no single year values
                else if (datePart.Length == 1)
                {
                    if (!month)
                    {
                        month = true;
                        formats.Add("M");
                        continue;
                    }
                    else if (!day)
                    {
                        day = true;
                        formats.Add("d");
                        continue;
                    }
                    else return false;
                }
                else if (datePart.Length == 4 && !year)
                {
                    year = true;
                    formats.Add("yyyy");
                    continue;
                }
                else
                    return false;
            }

            DateTime parseDate = DateTime.Now;
            this.formatString = formats.ToString(seperator);
            if (!DateTime.TryParseExact(parseString, this.formatString, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out parseDate))
                return false;
            details.Body = parseString;
            return true;
            //return base.ReadBody(source, details);
        }
    }



    public class TextTerminal : CompoundTerminalBase
    {
        public const string AllLatinLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        public const string DecimalDigits = "1234567890";
        public const string OctalDigits = "12345670";
        public const string HexDigits = "1234567890aAbBcCdDeEfF";
        public const string BinaryDigits = "01";
        public const string AllSpecialCharacters = @"`~!@#$%^&*()_+=-\|""';:/?.>,<£€";

        public const string AllLatinCharacters = AllLatinLetters + DecimalDigits + AllSpecialCharacters;

        public override IList<string> GetFirsts()
        {
            return AllLatinCharacters.ToArray().Select(x => new string(x, 1)).ToList();
        }

        public TextTerminal(string name)
            : base(name)
        {

        }


        public override void Init(GrammarData grammarData)
        {

            this.EditorInfo = new TokenEditorInfo(TokenType.String, TokenColor.Text, TokenTriggers.None);
            base.Init(grammarData);
        }

        protected override bool ConvertValue(CompoundTerminalBase.CompoundTokenDetails details)
        {

            details.Value = details.Body;
            return true;
        }


        protected override bool ReadBody(ISourceStream source, CompoundTokenDetails details)
        {
            int start = source.PreviewPosition;
            while (!source.EOF())
            {
                char current = source.PreviewChar;
                //kill off at a digit
                if (char.IsDigit(current))
                    break;
                //or kill of at punctiation that could infer a modifed number ie -1 .0 +1 etc....
                else if ("-.+".IndexOf(current) >= 0)
                {
                    var next = source.NextPreviewChar;
                    if (char.IsDigit(next))
                        break;
                }
                //kills off at a boolean
                else if (isBoolean(source))
                    break;
                source.PreviewPosition++;
            }
            int end = source.PreviewPosition;
            if (end == start)
                return false;
            var exposed = source.Text.Substring(start, end - start);
            if (string.IsNullOrWhiteSpace(exposed))
                return false;
            details.Body = exposed;
            return true;
        }

        static string[] bools = new string[] { "Yes", "No", "True", "False" };

        bool isBoolean(ISourceStream source)
        {
            return bools.Any(x => source.MatchSymbol(x));
        }

    }


    [Language("SMS Message", "4.5.6", "Inbound SMS Message Grammar")]
    public class SMSGrammar : Grammar
    {
        public SMSGrammar()
            : base(false)
        {
            //var field = TerminalFactory.CreateCSharpIdentifier("string");
            //var term = CreateTerm("term");
            var field = new TextTerminal("field");


            var dateField = new DateTerminal("date");
            var timeField = new TimeTerminal("time", ":-");
            var numberField = new NumberLiteral("number", NumberOptions.AllowSign | NumberOptions.AllowStartEndDot);
            //var special = new Spe
            var message = new NonTerminal("message");
            var statement = new NonTerminal("statement");
            var specials = new NonTerminal("puncuation");
            var date = new NonTerminal("date");
            var dateTime = new NonTerminal("datetime");
            var boolTerm = new NonTerminal("boolean");

            boolTerm.Rule = ToTerm("True") | "False" | "Yes" | "No";

            date.Rule = dateField | timeField;
            dateTime.Rule = dateField + timeField;

            MarkTransient(date);
            statement.Rule = numberField | date | dateTime | boolTerm | field;

            message.Rule = MakeListRule(message, Empty, statement);

            MarkTransient(statement);
            this.Root = message;

        }
    }

    [Language("SMS Syntax", "4.5.6", "Inbound SMS Syntax Grammer")]
    public class SMSSyntaxGrammar : Grammar
    {
        public SMSSyntaxGrammar()
            : base(false)
        {

            var field = TerminalFactory.CreateCSharpIdentifier("field");
            var identifer = new StringLiteral("identifer", "\"");

            var stringField = new NonTerminal("stringField");
            var groupExp = new NonTerminal("group");
            var expression = new NonTerminal("expression");
            var expInner = new NonTerminal("expInner");
            var expType = new NonTerminal("expType");
            var stmt = new NonTerminal("statement");
            var groupStmt = new NonTerminal("groupStmt");
            var expStmt = new NonTerminal("expStmt");

            var startGroup = ToTerm("[");
            var endGroup = ToTerm("]");
            var startExp = ToTerm("{");
            var endExp = ToTerm("}");

            var fieldSep = ToTerm(":");


            var stringPlusRule = new NonTerminal("expStringP");
            stringPlusRule.Rule = ToTerm("?+") | "STRING+";

            expType.Rule = ToTerm("STRING") | "INT" | "DECIMAL" | "DATE" | "DATETIME" | "TIME" | "BOOL" | "?+" | "STRING+";// | "?+";

            expInner.Rule = expType + fieldSep + field |
                            expType + fieldSep + field + fieldSep + identifer;



            expression.Rule = startExp + expInner + endExp;


            expStmt.Rule = MakeListRule(expStmt, Empty, expression);


            groupExp.Rule = startGroup + expStmt + endGroup;

            MarkTransient(expression);
            MarkPunctuation(startGroup, endGroup, startExp, endExp, fieldSep);

            //groupStmt.Rule = MakeListRule(groupStmt, Empty, groupExp) |
            //                 startGroup + stmt + endGroup;
                    

            //stmt.Rule = MakeStarRule(stmt, groupStmt);
            stmt.Rule = MakeListRule(stmt, Empty, groupExp) |
                        startGroup + stmt + endGroup;

            this.Root = stmt;
        }
    }
}
