using System;
using System.Collections.Generic;
#nullable enable

namespace GSharp.GSIL.GCode
{
    public sealed class Node
    {
        public int id;
        public Type type;
        public String? reference;
        public String? target;
        public int execution = -1;
        public List<int>? inputs;
        public List<int>? outputs;

        // data-only references
        public int dataReference;

    }

}