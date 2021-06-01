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
            IGSProject proj = TranspilerAPI.TranspilerProject(Path.GetFullPath("./HelloWorldSample/HWS.gsproj"));
            
            proj.TranspileScripts();
            // TranspilerAPI.TranspileScripts("./Samples/GSProject.gsproj");
            // GSharp.System.GScript sc1 = JsonConvert.DeserializeObject<GSharp.System.GScript>("");
        }
    }
}
