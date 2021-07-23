using System;
using System.Collections.Generic;

namespace GSharp.System.GNode
{
    public sealed class Node
    {   
        public Type type;
        public String reference;
        public String target;
        public int execution = -1;
        public List<int> inputs;
        public List<object> outputs;
        
    }

}