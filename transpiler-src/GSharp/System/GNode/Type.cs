namespace GSharp.System.GNode
{
    public enum Type : int
    {
        /// <summary>
        /// The entry node into a function <see cref="Node.target"/>.<br />
        /// Parsed arguments (a, b...) accessible through <see cref="Node.outputs"/>.<br />
        /// To execute next block set value of <see cref="Node.execution"/>.<br />
        /// </summary> 
        executionEnter = 0,
        /// <summary>
        /// Performs invocation of <see cref="Node.reference"/>.<see cref="Node."/>
        /// <br />
        /// Fields:
        /// ref
        /// target
        /// execOut
        /// inputs
        /// outputs
        /// </summary>
        invokeFunctionCall,
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