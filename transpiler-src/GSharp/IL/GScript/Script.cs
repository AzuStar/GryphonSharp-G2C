using System;
using System.Collections.Generic;

namespace GSharp.IL.GScript
{
    public sealed class Script
    {
        public Schema schema;
        public Dictionary<int, GData.Node> dataNodes;
        public Dictionary<int, GCode.Node> codeNodes;

        public void PostDeserialize()
        {
            foreach (int key in dataNodes.Keys)
            {
                dataNodes[key].id = key;
            }
            foreach (int key in codeNodes.Keys)
            {
                codeNodes[key].id = key;
            }
        }
    }

}