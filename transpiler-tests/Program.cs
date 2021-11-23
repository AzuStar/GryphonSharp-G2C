using System;
using System.Diagnostics;
using System.IO;
using GryphonSharpTranspiler;

namespace transpiler_tests
{
    class Program
    {
        static void Main(string[] args)
        {
            IGSProject proj = TranspilerAPI.LoadProject(Path.GetFullPath("../GryphonSharp-Samples/TestExample/Test.gsproj"));
            
            proj.TranspileScripts();
            // TranspilerAPI.TranspileScripts("./Samples/GSProject.gsproj");
            // GSharp.GSIL.GScript sc1 = JsonConvert.DeserializeObject<GSharp.GSIL.GScript>("");
        }
    }
}
