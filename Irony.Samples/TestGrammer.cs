using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irony.Samples
{
    [Language("Test Expression", "1.0", "Dynamic geometry expression evaluator")]
    public class TestGrammer : Grammar
    {
        public TestGrammer()
            : base(false)
        {

            var number = new NumberLiteral("number", NumberOptions.AllowSign);
            var string_literal = new StringLiteral("string", "\"", StringOptions.AllowsDoubledQuote);
            var lPar = ToTerm("(");
            var rPar = ToTerm(")");


            

            var lParSmt = new NonTerminal("lParStmt");
            var rParSmt = new NonTerminal("rParStmt");

            


            //lParStmt = "("
            lParSmt.Rule = lPar;
            //rParStmt = ")"
            rParSmt.Rule = rPar;

            conStmt.Rule = condIdent + lParSmt + conExpr + rParSmt;


            this.Root = conStmt;


        }


    }

}
