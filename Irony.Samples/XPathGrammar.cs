using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irony.Samples
{
    [Language("XPath", "1.0", "XPath 1.0")]
    public class XPathGrammar : Grammar
    {
        public XPathGrammar()
            : base(false)
        {

            //var xpath = new CustomTerminal("xpath", ParseXPathExpressionHandler);
            var number = TerminalFactory.CreateCSharpNumber("number");
            var identifier = TerminalFactory.CreateCSharpIdentifier("identifier");
            var expression = new NonTerminal("expression");
            var binexpr = new NonTerminal("binexpr");
            var parexpr = new NonTerminal("parexpr");
            var fncall = new NonTerminal("fncall");
            var binop = new NonTerminal("binop");
            //var xpathexpr = new NonTerminal("xpathexpr");

            ///xpathexpr.Rule = "xpath(" + xpath + ")";
            expression.Rule = parexpr | binexpr | number | fncall;// | xpathexpr;
            parexpr.Rule = "(" + expression + ")";
            binexpr.Rule = expression + binop + expression;
            binop.Rule = ToTerm("+") | "-" | "/" | "*" | "%";
            fncall.Rule = identifier + "(" + expression + ")";
            this.Root = expression;

            RegisterOperators(1, "+", "-");
            RegisterOperators(2, "*", "/", "%");

        }

    }

    [Language("DataMapping", "1.0", "handles parsing a data map chain")]
    public class DataMappingChain : Grammar
    {
        public DataMappingChain() :
            base(false)
        {
            var number = TerminalFactory.CreateCSharpNumber("number");
            var identifier = TerminalFactory.CreateCSharpIdentifier("identifier");
            var expression = new NonTerminal("expression");
            
            var stmt = new NonTerminal("statement");
            var paraExp = new NonTerminal("parexpr");
            var sep = ToTerm("/");


            var paraFieldExp = new NonTerminal("paraFieldExp");
            var paraEmptyExp = new NonTerminal("paraEmptyEx");
            var paraNumberExp = new NonTerminal("paraNumberExp");

            paraNumberExp.Rule = "[" + number + "]";
            paraEmptyExp.Rule = "[]" + sep + identifier;
            paraFieldExp.Rule = "[{" + expression + "}]";
            paraExp.Rule = paraNumberExp | paraFieldExp | paraEmptyExp;


            expression.Rule = identifier + sep + identifier |
                              identifier + sep + identifier + paraExp |
                              identifier + sep + identifier + paraExp + sep + identifier |
                              identifier + sep + identifier + sep + identifier;

            stmt.Rule = expression;
                              

            this.Root = stmt;

        }

       
    }

    public class StatementNode : AstNode
    {
        public override void Init(Ast.AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
        }
    }

    public class DataMappingRuntime : LanguageRuntime
    {
        public DataMappingRuntime(LanguageData language)
            : base(language)
        {
        }
        public override void Init()
        {
            base.Init();
           
        }

    }
}
