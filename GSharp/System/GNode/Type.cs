namespace GSharp.System.GNode
{
    public enum Type : int
    {
        // The entry node into script
        executionEnter = 0,
        call,
        reference,
        localValue,
        // The exit node of the script
        executionExit
    }
}