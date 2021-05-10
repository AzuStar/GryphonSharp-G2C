using System.Collections.Generic;

namespace GSharp.System.Data
{
    public class DataModel
    {   
        public Type type;
        
    }

    public enum Type : int{
        Value, localValue, 
    }

}