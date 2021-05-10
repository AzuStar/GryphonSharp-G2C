using System;
using System.Collections.Generic;
using System.IO;
using GSharp.System.GScript;
using Newtonsoft.Json;

namespace GryphonSharpTranspiler
{
    public static class TranspilerAPI
    {
        public static void TranspilerProject(string GSProjFile)
        {
            if (!File.Exists(GSProjFile)) throw new Exception("File doesn't exist:" + GSProjFile);
            String projectLocation = Directory.GetParent(GSProjFile).FullName;
            Console.WriteLine(projectLocation);
            GSProject project = JsonConvert.DeserializeObject<GSProject>(File.ReadAllText(GSProjFile));
            project.root = Directory.GetParent(GSProjFile).FullName;
            project.src = Path.GetFullPath(project.root + "/" + project.src);
            project.bin = Path.GetFullPath(project.root + "/" + project.bin);
           List<string> srcFiles = IterateFolder(project.src);
           foreach(string s in srcFiles)
             project.files.Add(new GSFile(s));
            // Script sc1 = JsonConvert.DeserializeObject<Script>("");
        }

        private static List<string> IterateFolder(string folder){
            List<string> list = new List<string>();
            RecursiveIteration(folder, list);
            return list;
        }

        private static void RecursiveIteration(string folder, List<string> listToPopulate)
        {
            foreach (string fol in Directory.EnumerateDirectories(folder))
            {
                RecursiveIteration(fol, listToPopulate);
            }
            foreach (string fil in Directory.EnumerateFiles(folder))
            {
                listToPopulate.Add(fil);
            }
        }
    }
}
