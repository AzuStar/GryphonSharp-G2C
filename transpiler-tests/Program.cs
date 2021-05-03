using System;
using System.IO;
using GryphonSharpTranspiler;

namespace transpiler_tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Path.GetFullPath("./"));
            // TranspilerAPI.TranspileScripts("./Samples/GSProject.gsproj");
            // GSharp.System.GScript sc1 = JsonConvert.DeserializeObject<GSharp.System.GScript>("");
        }
    }
}
