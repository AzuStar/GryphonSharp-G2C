using System;
using System.Collections.Generic;
using System.IO;
using GSharp.IL.GScript;
using Newtonsoft.Json;

namespace GryphonSharpTranspiler
{
    public static class TranspilerAPI
    {
        public static IGSProject LoadProject(string GSProjFile)
        {
            if (!File.Exists(GSProjFile)) throw new Exception("File doesn't exist:" + GSProjFile);
            String projectLocation = Directory.GetParent(GSProjFile).FullName;
            GSProject project = JsonConvert.DeserializeObject<GSProject>(File.ReadAllText(GSProjFile));
            project.root = Directory.GetParent(GSProjFile).FullName;
            project.src = Path.GetFullPath(project.root + "/" + project.src);
            project.bin = Path.GetFullPath(project.root + "/" + project.bin);
            List<string> srcFiles = Utils.IterateFolder(project.src);
            foreach (string s in srcFiles)
            {
                GSFile fil = new GSFile(project, s);
                if (fil.ScriptBody != null)
                    project.files.Add(fil);
            }

            return project;
        }

    }
}
