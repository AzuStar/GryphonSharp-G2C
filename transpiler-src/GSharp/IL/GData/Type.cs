namespace GSharp.IL.GData
{
    public enum Type : int
    {
        /// <summary>
        /// Actual value held at this data node.
        /// </summary>
        primitiveValue = 0,
        /// <summary>
        /// Scoped variable existing for some scope, usually artifact of: <br/> 
        /// current function argument; in example below <i>args</i><code>
        /// public static void main(String[] args){
        /// 
        /// }  
        /// </code><br/>
        /// return of external call; in example below <i>return</i><code>
        /// public static void main(String[] args){
        /// var return = Math.Sqrt(9);
        /// }  
        /// </code><br/>
        /// </summary> 
        localValue = 1,
    }
}