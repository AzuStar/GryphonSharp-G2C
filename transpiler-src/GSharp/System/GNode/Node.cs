using System.Collections.Generic;

namespace GSharp.System.GNode
{
    public class Node
    {   
        public Type type;
        public List<int> execEnterNodes;
        public List<int> execOutNodes;
        public List<object> args;
        public List<object> returns;
        
    }

}