using System.Collections.Generic;
#nullable enable

namespace GSharp.GSIL.GData
{
    public sealed class Node
    {   
        public int id;
        public Type type;
        /// <summary>
        /// Primitive value stored at this node
        /// </summary>
        public object? value;

    }



}