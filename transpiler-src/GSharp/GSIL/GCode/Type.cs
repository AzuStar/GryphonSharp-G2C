namespace GSharp.GSIL.GCode
{
    /// <summary>
    /// Dont change numbers after 1.0 release as those will play role of backwards campatibility
    /// </summary>
    public enum Type : int
    {
        /// <summary>
        /// The entry node into a function <see cref="Node.target"/>.<br/>
        /// Parsed arguments (a, b...) accessible through <see cref="Node.outputs"/>.<br/>
        /// To execute next block set value of <see cref="Node.execution"/>.<br/>
        /// </summary> 
        executionEnter = 0,
        /// <summary>
        /// Performs invocation of <see cref="Node.target"/> in  <see cref="Node.reference"/> domain.<br/>
        /// To execute next block set value of <see cref="Node.execution"/>.<br/>
        /// Takes arguments into function through <see cref="Node.inputs"/>.<br/>
        /// Returns values through <see cref="Node.outputs"/>.<br/>
        /// </summary>
        invokeInstanceCall = 1,
        /// <summary>
        /// Primitive value in the function. This is a local variable of type int, gloat, string etc<br/>
        /// Fields:
        /// outputs
        /// </summary>
        primitiveValue = 2,
        /// <summary>
        /// Performs invocation on Static Reference.
        /// <br/>
        /// Fields:
        /// ref
        /// target
        /// execOut
        /// inputs
        /// outputs
        /// </summary>
        invokeStaticCall = 3,
        /// <summary>
        /// This will take infinite array of types and apply <see cref="Node.target"/> operator on them.
        /// <br/>
        /// Operator in <see cref="Node.target"/> can be one of: <b>*</b>, <b>/</b>, <b>+</b>, <b>-</b>
        /// </summary>
        invokeOperatorCall = 4,
        /// <summary>
        /// The exit node of the script.<br/>
        /// Returns from the function with values of <see cref="Node.inputs"/>.<br/>
        /// </summary> 
        executionExit = 100
    }
}