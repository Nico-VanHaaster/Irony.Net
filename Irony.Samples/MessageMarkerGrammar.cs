using Irony.Interpreter.Ast;
using Irony.Parsing;
using Irony.Samples.InboundSMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irony.Samples
{
    [Language("Message Markdown", "1.00", "Sample GW Basic grammar")]
    public class MessageMarkerGrammar : Grammar
    {
        public MessageMarkerGrammar()
        {

            var root = new NonTerminal("root");

            var stringLit = new TextTerminal("string");
            stringLit.Priority = -1000;

            var number = new NumberLiteral("number", NumberOptions.AllowSign);
            var string_literal = new StringLiteral("string_l", "\"", StringOptions.AllowsDoubledQuote);
            var lit = TerminalFactory.CreateCSharpIdentifier("literal");
            var field_lit = new NonTerminal("member_access_literal");
            var statement = new NonTerminal("statement");
            var program = new NonTerminal("program", typeof(StatementListNode));

            var dot = ToTerm(".", "dot");


            var value_start = ToTerm("#=", "marker");
            var hash = ToTerm("#", "hash");

            var s_lpr = ToTerm("(");
            var s_rpr = ToTerm(")");
            var s_lbr = ToTerm("{");
            var s_rbr = ToTerm("}");

            var expression = new NonTerminal("expression");

            field_lit.Rule = MakeStarRule(field_lit, dot, lit);

            var field = new NonTerminal("member");
            field.Rule = field_lit | string_literal | number;

            var field_exp = new NonTerminal("field_exp");
            field_exp.Rule = value_start + field + hash;



            #region If Statemts
            var statement_list = new NonTerminal("statement_list");

            var block = new NonTerminal("block");

            //block.Rule = s_lbr + 


            var if_statement = new NonTerminal("if_statement");
            if_statement.Rule = ToTerm("if") + s_lpr + field + s_rpr + s_lbr;

            #endregion

            expression.Rule = if_statement;
            statement.Rule = stringLit | expression | field_exp;


            program.Rule = MakePlusRule(program, null, statement);

            root.Rule = program;

            this.MarkPunctuation("{", "}");
            this.Root = root;


        }
    }
}
