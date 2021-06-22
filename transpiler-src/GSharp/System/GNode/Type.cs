namespace GSharp.System.GNode
{
    public enum Type : int
    {
        /// <summary>
        /// The entry node into script.<br />
        /// Fields:
        /// execOut
        /// outputs
        /// </summary> 
        executionEnter = 0,
        /// <summary>
        /// Performs invocation on Instance Reference
        /// <br />
        /// Fields:
        /// ref
        /// target
        /// execOut
        /// inputs
        /// outputs
        /// </summary>
        callInstance,
        /// <summary>
        /// A special hidden node that play role that of function output reference. E.g. Sum(2, 3) -> reference <br />
        /// </summary>
        reference,
        /// <summary>
        /// Primitive value in the function. This is a local variable of type int, gloat, string etc<br />
        /// Fields:
        /// outputs
        /// </summary>
        primitiveValue,
        /// <summary>
        /// Performs invocation on Static Reference.
        /// <br />
        /// Fields:
        /// ref
        /// target
        /// execOut
        /// inputs
        /// outputs
        /// </summary>
        callStatic,
        /// <summary>
        /// The exit node of the script.<br />
        /// Fields:
        /// inputs
        /// </summary> 
        executionExit = 100
    }
}