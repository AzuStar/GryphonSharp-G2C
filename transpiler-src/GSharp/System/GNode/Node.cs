using System;
using System.Collections.Generic;

namespace GSharp.System.GNode
{
    public sealed class Node
    {   
        public Type type;
        public string reference;
        public String target;
        public List<int> execOut;
        public List<int> inputs;
        public List<object> outputs;
        
    }

}