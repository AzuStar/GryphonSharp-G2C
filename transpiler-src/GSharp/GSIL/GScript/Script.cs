using System;
using System.Collections.Generic;

namespace GSharp.GSIL.GScript
{
    public sealed class Script
    {   
        public Schema schema;
        public Dictionary<int, GData.Node> dataNodes;
        public Dictionary<int, GCode.Node> codeNodes;
        
    }

}