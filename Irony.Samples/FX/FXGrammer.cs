using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irony.Samples.FX
{
    /// <summary>
    /// The grammar dictionary for the function parger
    /// </summary>
    [Language("FX", "4.5.1", "Function Parser Grammer")]
    public sealed class FXGrammer : Grammar
    {
        /// <summary>
        /// FX Grammar constructor
        /// </summary>
        public FXGrammer()
            : base(false)
        {

            #region Basic Grammar
            var number = new NumberLiteral("number", NumberOptions.AllowSign);
            var string_literal = new StringLiteral("string", "\"", StringOptions.AllowsDoubledQuote);

            var lPar = ToTerm("(");
            var rPar = ToTerm(")");
            var equals = ToTerm("=");
            //var empty = ToTerm(" ");
            var dot = ToTerm(".");
            var comma = ToTerm(",");
            var dateAdd = ToTerm("DateAdd");
            var dateDif = ToTerm("DateDiff");
            var now = ToTerm("Now");
            var today = ToTerm("Today");
            var datePart = ToTerm("DatePart");
            var dateFormat = ToTerm("DateFormat");
            var startOfMonth = ToTerm("StartOfMonth");
            var endOfMonth = ToTerm("EndOfMonth");
            var startOfDay = ToTerm("StartOfDay");
            var endOfDay = ToTerm("EndOfDay");
            var isDate = ToTerm("IsDate");
            var dateParse = ToTerm("DateParse");

            var concat = ToTerm("Concat");
            var upper = ToTerm("Upper");
            var lower = ToTerm("Lower");
            var substr = ToTerm("Substring");
            var left = ToTerm("Left");
            var right = ToTerm("Right");
            var split = ToTerm("Split");
            var arrayLen = ToTerm("ArrayLength");
            var indexOf = ToTerm("IndexOf");
            var trim = ToTerm("Trim");
            var condition = ToTerm("Condition");
            var replace = ToTerm("Replace");
            var contains = ToTerm("Contains");
            var join = ToTerm("Join");
            #endregion

            #region Math Terms

            var mathIdent = ToTerm("Math");
            var sum = ToTerm("Sum");
            var avg = ToTerm("Avg");
            var max = ToTerm("Max");
            var min = ToTerm("Min");
            #endregion

            #region helper non terms

            var unExpr = new NonTerminal("unExpr");
            var binExpr = new NonTerminal("binExpr");
            var binOp = new NonTerminal("binOp");
            var lParSmt = new NonTerminal("LeftParen");
            var rParSmt = new NonTerminal("RightParen");
            var bMethod = new NonTerminal("function");
            var method = new NonTerminal("method");
            var fxList = new NonTerminal("fxLst");
            #endregion

            #region Date non terms
            var dateStmts = new NonTerminal("dateStmt");
            var dateAddStmt = new NonTerminal("DateAdd");
            var dateDifStmt = new NonTerminal("DateDiff");
            var nowStmt = new NonTerminal("Now");
            var todayStmt = new NonTerminal("Today");
            var datePartStmt = new NonTerminal("DatePart");
            var dateFormatStmt = new NonTerminal("DateFormat");
            var startOfMonthStmt = new NonTerminal("StartOfMonth");
            var endOfMonthStmt = new NonTerminal("EndOfMonth");
            var startOfDayStmt = new NonTerminal("StartOfDay");
            var endOfDayStmt = new NonTerminal("EndOfDay");
            var isDateStmt = new NonTerminal("IsDate");
            var dateParseStmt = new NonTerminal("DateParse");
            #endregion

            #region string non terms

            var stringStmts = new NonTerminal("stringStmt");
            var concatStmt = new NonTerminal("Concat");
            var upperStmt = new NonTerminal("Upper");
            var lowerStmt = new NonTerminal("Lower");
            var substrStmt = new NonTerminal("Substring");
            var leftStmt = new NonTerminal("Left");
            var rightStmt = new NonTerminal("Right");
            var splitStmt = new NonTerminal("Split");
            var arrayLengthStmt = new NonTerminal("ArrayLength");
            var indexOfStmt = new NonTerminal("IndexOf");
            var replaceStmt = new NonTerminal("Replace");
            var containsStmt = new NonTerminal("Contains");
            var joinStmt = new NonTerminal("Join");

            #endregion

            #region Math Non Terms
            var mathStmts = new NonTerminal("mathStmts");

            var mathStmt = new NonTerminal("Math");
            var avgStmt = new NonTerminal("Avg");
            var sumStmt = new NonTerminal("Sum");
            var minStmt = new NonTerminal("Min");
            var maxStmt = new NonTerminal("Max");

            var mathExpr = new NonTerminal("mathExpr");
            var mathTerm = new NonTerminal("mathTerm");
            var mathBinExpr = new NonTerminal("mathBinExpr");
            var mathParExpr = new NonTerminal("mathParExpr");
            var mathUnExpr = new NonTerminal("mathUnExpr");
            var mathUnOp = new NonTerminal("mathUnOp");
            var mathBinOp = new NonTerminal("mathBinOp", "operator");
            var mathPostFixExpr = new NonTerminal("mathPostFixExpr");
            var mathPostFixOp = new NonTerminal("mathPostFixOp");

            #endregion

            #region Helper Non Terms
            var expression = new NonTerminal("expression");
            var expTree = new NonTerminal("expTree");
            var term = new NonTerminal("term");
            var fx = new NonTerminal("fx");
            var stmtList = new NonTerminal("stmtList");
            var stmtLine = new NonTerminal("stmt");
            var Id = new NonTerminal("Id");
            #endregion

            #region Field

            var fieldSlashTerm = ToTerm("/");
            var fieldNum = new NumberLiteral("number");
            var fieldTerm = TerminalFactory.CreateCSharpIdentifier("field");
            var field = new NonTerminal("field");
            var fieldLstTerm = new NonTerminal("fieldLstTerm");

            var fieldSlash = new NonTerminal("fieldSlash");
            var fieldSlashRule = new NonTerminal("fieldSlashRule");
            var fieldDotRule = new NonTerminal("fieldDotRule");

            var fieldIndex = new NonTerminal("fieldIndex");
            var fieldIndexRule = new NonTerminal("fieldIndexRule");
            var fieldIndexTerm = new NonTerminal("fieldIndexTerm");
            var fieldBraceIndexTerm = new NonTerminal("fieldBraceIndexTerm");


            fieldSlash.Rule = fieldSlashTerm + fieldTerm;
            fieldBraceIndexTerm.Rule = "[{" + fieldSlashRule + "}]";
            fieldIndexTerm.Rule = "[" + fieldNum + "]";
            fieldIndexRule.Rule = fieldBraceIndexTerm | fieldIndexTerm;

            fieldDotRule.Rule = field + dot + field;

            fieldSlashRule.Rule = fieldTerm + fieldSlash | fieldTerm + fieldSlash + fieldSlash;

            fieldIndex.Rule = fieldTerm + fieldSlash + fieldIndexRule | fieldTerm + fieldSlash + fieldIndexRule + fieldSlash;

            field.Rule = fieldTerm | fieldTerm + fieldSlash | fieldTerm + fieldSlash + fieldSlash | fieldIndex | fieldDotRule;

            #endregion

            #region Date Rules
            dateAddStmt.Rule = dateAdd + lParSmt + expression + comma + expression + comma + expression + rParSmt;

            dateDifStmt.Rule = dateDif + lParSmt + expression + comma + expression + comma + expression + rParSmt;

            nowStmt.Rule = now + lParSmt + rParSmt;

            todayStmt.Rule = today + lParSmt + rParSmt;

            datePartStmt.Rule = datePart + lParSmt + expression + comma + expression + rParSmt;

            dateFormatStmt.Rule = dateFormat + lParSmt + expression + comma + expression + rParSmt;

            startOfMonthStmt.Rule = startOfMonth + lParSmt + rParSmt |
                                    startOfMonth + lParSmt + expression + rParSmt;

            endOfMonthStmt.Rule = endOfMonth + lParSmt + rParSmt |
                                  endOfMonth + lParSmt + expression + rParSmt;

            startOfDayStmt.Rule = startOfDay + lParSmt + rParSmt |
                                  startOfDay + lParSmt + expression + rParSmt;

            endOfDayStmt.Rule = endOfDay + lParSmt + rParSmt |
                                endOfDay + lParSmt + expression + rParSmt;

            isDateStmt.Rule = isDate + lParSmt + expression + rParSmt |
                              isDate + lParSmt + expression + comma + expression + rParSmt;// isDate + lParSmt + expression + comma + expression + comma + expression + rParSmt;

            dateParseStmt.Rule = dateParse + lParSmt + expression + rParSmt |
                                 dateParse + lParSmt + expression + comma + expression + rParSmt |
                                 dateParse + lParSmt + expression + comma + expression + comma + expression + rParSmt;


            dateStmts.Rule = dateAddStmt | dateDifStmt | nowStmt |
                             todayStmt | datePartStmt | dateFormatStmt | startOfMonthStmt |
                             endOfMonthStmt | startOfDayStmt | endOfDayStmt | isDateStmt | dateParseStmt;
            #endregion

            #region String Rules

            // concatStmtLst.Rule = MakeListRule(concatStmtLst, comma, expression);
            concatStmt.Rule = concat + lParSmt + expTree + rParSmt;
            lowerStmt.Rule = lower + lParSmt + expression + rParSmt;
            upperStmt.Rule = upper + lParSmt + expression + rParSmt;
            substrStmt.Rule = substr + lParSmt + expression + comma + expression + rParSmt |
                              substr + lParSmt + expression + comma + expression + comma + expression + rParSmt;
            leftStmt.Rule = left + lParSmt + expression + comma + expression + rParSmt;
            rightStmt.Rule = right + lParSmt + expression + comma + expression + rParSmt;
            splitStmt.Rule = split + lParSmt + expression + comma + expression + rParSmt |
                             split + lParSmt + expression + comma + expression + comma + expression + rParSmt;

            arrayLengthStmt.Rule = arrayLen + lParSmt + expression + comma + expression + rParSmt;
            indexOfStmt.Rule = indexOf + lParSmt + expression + comma + expression + rParSmt;


            replaceStmt.Rule = replace + lParSmt + expression + comma + expression + comma + expression + rParSmt |
                               replace + lParSmt + expression + comma + expression + comma + expression + comma + expression + rParSmt;
            containsStmt.Rule = contains + lParSmt + expression + comma + expression + rParSmt;
            joinStmt.Rule = join + lParSmt + expression + comma + expTree + rParSmt;



            stringStmts.Rule = concatStmt | lowerStmt | upperStmt | substrStmt | leftStmt | rightStmt | splitStmt | arrayLengthStmt | indexOfStmt |
                               replaceStmt | containsStmt | joinStmt;

            #endregion

            #region Condition Rules

            var condIdent = ToTerm("Condition");
            var conExpr = new NonTerminal("condExpr");
            var condStmt = new NonTerminal("Condition");
            var condParExp = new NonTerminal("condParExp");
            var condBinOp = new NonTerminal("condBinOp");
            var condBinExp = new NonTerminal("condBinExpr");
            var condNotStmt = new NonTerminal("condNotStmt");
            var condSepOp = new NonTerminal("condSepOp");
            var condSepExpr = new NonTerminal("condSepExpr");

            var condResult = new NonTerminal("condResult");
            var condFalseResult = new NonTerminal("condFalseResult");

            conExpr.Rule = condSepExpr | condBinExp | condParExp | condNotStmt | expression;
            condParExp.Rule = lParSmt + conExpr + rParSmt;
            condBinOp.Rule = ToTerm("=") | "!=" | ">" | "<" | ">=" | "<=" | "Contains" | ToTerm("Start") + ToTerm("With") | ToTerm("Ends") + ToTerm("With");
            condSepOp.Rule = ToTerm("&") | "|";
            condBinExp.Rule = conExpr + condBinOp + conExpr;
            condSepExpr.Rule = conExpr + condSepOp + conExpr;
            condNotStmt.Rule = ToTerm("!") + condParExp;
            condStmt.Rule = condIdent + lParSmt + conExpr + condResult + rParSmt |
                            condIdent + lParSmt + conExpr + condResult + condResult + rParSmt;

            condResult.Rule = comma + expression;

            #endregion

            #region Math Rules
            // 1. Terminals



            mathExpr.Rule = mathTerm | mathUnExpr | expression | mathBinExpr;
            mathTerm.Rule = mathParExpr | mathIdent;
            mathParExpr.Rule = lParSmt + mathExpr + rParSmt;
            mathUnExpr.Rule = mathUnOp + mathTerm;
            mathUnOp.Rule = ToTerm("+") | "-" | "++" | "--";
            mathBinExpr.Rule = mathExpr + mathBinOp + mathExpr;
            mathBinOp.Rule = ToTerm("+") | "-" | "*" | "/";

            mathStmt.Rule = mathIdent + lParSmt + mathExpr + rParSmt;

            avgStmt.Rule = avg + lParSmt + expTree + rParSmt;
            minStmt.Rule = min + lParSmt + expTree + rParSmt;


            sumStmt.Rule = sum + lParSmt + expTree + rParSmt;
            maxStmt.Rule = max + lParSmt + expTree + rParSmt;



            mathStmts.Rule = mathStmt | avgStmt | minStmt | sumStmt | maxStmt;

            #endregion

            #region Helper Rules
            //an expression tree follows the logic expression + comma + expression .... for infinite reoccurangeces
            expTree.Rule = MakeListRule(expTree, comma, expression);

            //a method rule is a rule that starts with a field follwed by left paren with optional expression tree and ends with a right paren
            method.Rule = field + lParSmt + expTree + rParSmt |
                          field + lParSmt + rParSmt;

            //lParStmt = "("
            lParSmt.Rule = lPar;
            //rParStmt = ")"
            rParSmt.Rule = rPar;

            /*epxression rules can be any of the following
             * 
             * Built In Method.
             * Method
             * Field
             * String Literal
             * Number
             * Comma
             * 
             */
            expression.Rule = binExpr | bMethod | string_literal | number | method | field | comma;


            binExpr.Rule = expression + ToTerm("+") + expression;
            binOp.Rule = mathBinOp | "%" //arithmetic
                           | "&" | "|" | "^"                     //bit
                           | "=" | ">" | "<" | ">=" | "<=" | "<>" | "!=" | "!<" | "!>" | "+";

            bMethod.Rule = dateStmts | stringStmts | mathStmts | condStmt;// | method | field;

            fx.Rule = bMethod | string_literal | number | field;// | ToTerm("+");

            stmtLine.Rule = fx;
            stmtList.Rule = MakeListRule(stmtList, ToTerm("+"), stmtLine);
            this.Root = stmtList;

            MarkTransient(expression, mathExpr, fx, mathTerm, mathStmts, mathBinOp, conExpr, condSepOp, stringStmts, dateStmts, stmtLine, lParSmt, rParSmt);

            RegisterOperators(1, "+", "-");
            RegisterOperators(2, "*", "/");
            RegisterOperators(0, "|", "&");
            #endregion
        }
    }

}


