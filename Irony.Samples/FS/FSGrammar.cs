using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Interpreter.Evaluator;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irony.Samples.FS
{
    // <summary>
    /// The grammar dictionary for the function parger
    /// </summary>

    // A ready-to-use evaluator implementation.

    // This grammar describes programs that consist of simple expressions and assignments
    // for ex:
    // x = 3
    // y = -x + 5
    //  the result of calculation is the result of last expression or assignment.
    //  Irony's default  runtime provides expression evaluation. 
    //  supports inc/dec operators (++,--), both prefix and postfix, and combined assignment operators like +=, -=, etc.
    //  supports bool operators &, |, and short-circuit versions &&, ||
    //  supports ternary ?: operator

    [Language("FS", "4.5.1", "Field Service Grammer")]
    public class FSEvaluatorGrammar : InterpretedLanguageGrammar
    {
        public FSEvaluatorGrammar() : base(caseSensitive: false)
        {
            this.GrammarComments =
      @"Irony expression evaluator. Case-insensitive. Supports big integers, float data types, variables, assignments,
arithmetic operations, augmented assignments (+=, -=), inc/dec (++,--), strings with embedded expressions; 
bool operations &,&&, |, ||; ternary '?:' operator.";
            // 1. Terminals
            var number = new NumberLiteral("NUMBER");
            //Let's allow big integers (with unlimited number of digits):
            number.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.Int64, NumberLiteral.TypeCodeBigInt };
            var identifier = new IdentifierTerminal("IDENTIFIER");
            //var comment = new CommentTerminal("comment", "#", "\n", "\r");
            //comment must be added to NonGrammarTerminals list; it is not used directly in grammar rules,
            // so we add it to this list to let Scanner know that it is also a valid terminal. 

            var comma = ToTerm(",", "COMMA");

            //String literal with embedded expressions  ------------------------------------------------------------------
            //var stringLit = new StringLiteral("STRING", "\"", StringOptions.AllowsAllEscapes | StringOptions.IsTemplate);
            //stringLit.AddStartEnd("'", StringOptions.AllowsAllEscapes | StringOptions.IsTemplate);
            //stringLit.AstConfig.NodeType = typeof(StringTemplateNode);
            var Expr = new NonTerminal("Expr"); //declare it here to use in template definition 
            var templateSettings = new StringTemplateSettings(); //by default set to Ruby-style settings 
            templateSettings.ExpressionRoot = Expr; //this defines how to evaluate expressions inside template
            this.SnippetRoots.Add(Expr);
            //stringLit.AstConfig.Data = templateSettings;
            //--------------------------------------------------------------------------------------------------------

            // 2. Non-terminals
            var Term = new NonTerminal("Term");
            var BinExpr = new NonTerminal("BinExpr", typeof(BinaryOperationNode));
            var ParExpr = new NonTerminal("ParExpr");
            var UnExpr = new NonTerminal("UnExpr", typeof(UnaryOperationNode));
            //var TernaryIfExpr = new NonTerminal("TernaryIf", typeof(IfNode));
            var ArgList = new NonTerminal("ArgList", typeof(ExpressionListNode));
            //var FunctionCall = new NonTerminal("FunctionCall", typeof(FunctionCallNode));
            //var MemberAccess = new NonTerminal("MemberAccess", typeof(MemberAccessNode));
            //var IndexedAccess = new NonTerminal("IndexedAccess", typeof(IndexedAccessNode));
            //var ObjectRef = new NonTerminal("ObjectRef"); // foo, foo.bar or f['bar']
            var UnOp = new NonTerminal("UnOp");
            var BinOp = new NonTerminal("BinOp", "operator");
            //var PrefixIncDec = new NonTerminal("PrefixIncDec", typeof(IncDecNode));
            //var PostfixIncDec = new NonTerminal("PostfixIncDec", typeof(IncDecNode));
            //var IncDecOp = new NonTerminal("IncDecOp");
            //var AssignmentStmt = new NonTerminal("AssignmentStmt", typeof(AssignmentNode));
            //var AssignmentOp = new NonTerminal("AssignmentOp", "assignment operator");
            var Statement = new NonTerminal("Statement");

            var AVGFunction = new NonTerminal("Avg", typeof(FunctionCallNode));
            var MinFunction = new NonTerminal("Min", typeof(FunctionCallNode));
            var MaxFunction = new NonTerminal("Max", typeof(FunctionCallNode));
            var SumFunction = new NonTerminal("Sum", typeof(FunctionCallNode));
            var PowFunction = new NonTerminal("Pow", typeof(FunctionCallNode));
            var SqrtFunction = new NonTerminal("SQRT", typeof(FunctionCallNode));

            var Program = new NonTerminal("Program", typeof(StatementListNode));

            // 3. BNF rules
            Expr.Rule = Term | UnExpr | BinExpr;// |;
            Term.Rule = number | ParExpr |  AVGFunction | MaxFunction | MinFunction | SqrtFunction | PowFunction | SumFunction | identifier; // | MemberAccess;// | IndexedAccess;
            ParExpr.Rule = "(" + Expr + ")";
            UnExpr.Rule = UnOp + Term + ReduceHere();
            UnOp.Rule = ToTerm("+") | "-";// | 
            BinExpr.Rule = Expr + BinOp + Expr + ReduceHere();
            BinOp.Rule = ToTerm("+") | "-" | "*" | "/" | "^";// "**" | "==" | "<" | "<=" | ">" | ">=" | "!=" | "&&" | "||" | "&" | "|";
           
            Statement.Rule =  Expr | Empty;
            ArgList.Rule = MakeStarRule(ArgList, comma, Expr);
            
            AVGFunction.Rule = ToTerm("AVG") + PreferShiftHere() + "(" + ArgList + ")";
            AVGFunction.NodeCaptionTemplate = "avg(...)";
            MinFunction.Rule = ToTerm("MIN") + PreferShiftHere() + "(" + ArgList + ")";
            MinFunction.NodeCaptionTemplate = "min(...)";
            MaxFunction.Rule = ToTerm("MAX") + PreferShiftHere() + "(" + ArgList + ")";
            MaxFunction.NodeCaptionTemplate = "max(...)";
            SumFunction.Rule = ToTerm("SUM") + PreferShiftHere() + "(" + ArgList + ")";
            SumFunction.NodeCaptionTemplate = "sum(...)";
            PowFunction.Rule = ToTerm("POW") + PreferShiftHere() + "(" + Expr + comma + Expr + ")";
            PowFunction.NodeCaptionTemplate = "pow(...)";
            SqrtFunction.Rule = ToTerm("SQRT") + PreferShiftHere() + "(" + Expr + ")";
            SqrtFunction.NodeCaptionTemplate = "sqrt(...)";

            //ObjectRef.Rule = identifier;// | MemberAccess;// | IndexedAccess;
            //IndexedAccess.Rule = Expr + PreferShiftHere() + "[" + Expr + "]";

            Program.Rule = MakePlusRule(Program, NewLine, Statement);

            this.Root = Program;       // Set grammar root
            //base.NonGrammarTerminals.Add(comment);

            // 4. Operators precedence
            RegisterOperators(10, "?");
            RegisterOperators(15, "&", "&&", "|", "||");
            RegisterOperators(20, "==", "<", "<=", ">", ">=", "!=");
            RegisterOperators(30, "+", "-");
            RegisterOperators(40, "*", "/", "^");
            RegisterOperators(50, Associativity.Right, "**");
            RegisterOperators(60, "!");
            // For precedence to work, we need to take care of one more thing: BinOp. 
            //For BinOp which is or-combination of binary operators, we need to either 
            // 1) mark it transient or 2) set flag TermFlags.InheritPrecedence
            // We use first option, making it Transient.  

            // 5. Punctuation and transient terms
            MarkPunctuation("(", ")", ",");
            RegisterBracePair("(", ")");
            MarkTransient(Term, Expr, Statement, BinOp, UnOp, ParExpr);

            // 7. Syntax error reporting
            MarkNotReported("++", "--");
            AddToNoReportGroup("(", "++", "--");
            AddToNoReportGroup(NewLine);
            AddOperatorReportGroup("operator");
            AddTermsReportGroup("assignment operator", "=", "+=", "-=", "*=", "/=");

            //9. Language flags. 
            // Automatically add NewLine before EOF so that our BNF rules work correctly when there's no final line break in source
            this.LanguageFlags = LanguageFlags.NewLineBeforeEOF | LanguageFlags.CreateAst | LanguageFlags.SupportsBigInt;
        }

        public override LanguageRuntime CreateRuntime(LanguageData language)
        {
            return new ExpressionEvaluatorRuntime(language);
        }

        #region Running in Grammar Explorer
        private static ExpressionEvaluator _evaluator;
        public override string RunSample(RunSampleArgs args)
        {
            //if (_evaluator == null)
            //{
            //    _evaluator = new ExpressionEvaluator(this);
            //    _evaluator.Globals.Add("null", _evaluator.Runtime.NoneValue);
            //    _evaluator.Globals.Add("true", true);
            //    _evaluator.Globals.Add("false", false);

            //}
            //_evaluator.ClearOutput();
            ////for (int i = 0; i < 1000; i++)  //for perf measurements, to execute 1000 times
            //_evaluator.Evaluate(args.ParsedSample);
            //return _evaluator.GetOutput();
            return "";
        }
        #endregion




    }
}
