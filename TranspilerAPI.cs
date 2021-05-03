using System;
using System.IO;
using GSharp.System.GScript;
using Newtonsoft.Json;

namespace GryphonSharpTranspiler
{
    public static class TranspilerAPI
    {
        public static void TranspileScripts(string GSProjFile)
        {
            GSProject project = JsonConvert.DeserializeObject<GSProject>(File.ReadAllText(GSProjFile));
            Console.WriteLine(project.src);
            // Script sc1 = JsonConvert.DeserializeObject<Script>("");
        }
    }
}
