using System.Collections.Generic;

namespace GSharp.System.Data
{
    public sealed class DataModel
    {   
        public Type type;
        
    }

    public enum Type : int{
        /// <summary>
        /// 
        /// </summary>
        Value,
        /// <summary>
        /// 
        /// </summary> 
        localValue, 
    }

}