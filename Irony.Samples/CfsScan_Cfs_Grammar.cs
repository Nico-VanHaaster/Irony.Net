using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irony.Samples
{
    [Language("CfsScan", "1", "CfsScan 1.0 grammar")]
    public class CfsScan_Cfs_Grammar : Grammar
    {
        public CfsScan_Cfs_Grammar()
        {

            var MFS = ToTerm("MFS");
            var CFSRES = ToTerm("CFSRES");

            var COLON = ToTerm(":");
            var COMMA = ToTerm(",");
            var SPACE = ToTerm(" ");
            var ASTERISK = ToTerm("*");


            var MFS_CFSRES_RESPONSE = new NonTerminal("mfs_cfsres");

            MFS_CFSRES_RESPONSE = MFS + COLON + T


        }
    }
}
