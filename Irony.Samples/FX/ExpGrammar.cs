using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irony.Samples.FX
{
    [Language("Expression", "1.0", "Dynamic geometry expression evaluator")]
    public class ExpGrammar : Grammar
    {
        public ExpGrammar()
        {

            var fieldSlashTerm = ToTerm("/");
            var fieldNum = new NumberLiteral("number");
            var fieldTerm = TerminalFactory.CreateCSharpIdentifier("field");
            var field = new NonTerminal("field");
            var fieldLstTerm = new NonTerminal("fieldLstTerm");
            
            var fieldSlash = new NonTerminal("fieldSlash");
            var fieldSlashRule = new NonTerminal("fieldSlashRule");

            var fieldIndex = new NonTerminal("fieldIndex");
            var fieldIndexRule = new NonTerminal("fieldIndexRule");
            var fieldIndexTerm = new NonTerminal("fieldIndexTerm");
            var fieldBraceIndexTerm = new NonTerminal("fieldBraceIndexTerm");


            fieldSlash.Rule = fieldSlashTerm + fieldTerm;
            fieldBraceIndexTerm.Rule = "[{" + fieldSlashRule + "}]";
            fieldIndexTerm.Rule = "[" + fieldNum + "]";
            fieldIndexRule.Rule = fieldBraceIndexTerm | fieldIndexTerm;

            fieldSlashRule.Rule = fieldTerm + fieldSlash | fieldTerm + fieldSlash + fieldSlash;

            fieldIndex.Rule = fieldTerm + fieldSlash + fieldIndexRule | fieldTerm + fieldSlash + fieldIndexRule + fieldSlash;

            field.Rule = fieldTerm | fieldTerm + fieldSlash | fieldTerm + fieldSlash + fieldSlash | fieldIndex;

            
            

            this.MarkTransient(fieldSlash);
            this.MarkPunctuation(fieldSlashTerm);
            var stmt = new NonTerminal("stmt");
            stmt.Rule = field;
            this.Root = stmt;
        }

       

    }
}
