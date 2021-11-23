using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace GryphonSharpTranspiler
{
    internal class GSProject : IGSProject
    {
        public int version;
        public string src;
        public string bin;
        public string rootNamespace = "Root";

        [JsonIgnore]
        public string root;
        [JsonIgnore]
        public List<GSFile> files = new List<GSFile>();

        public void TranspileScripts()
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            foreach (GSFile fil in files)
            {
                string sourceFile = fil.GenerateSource(provider);
                string writepath = Path.Join(bin, fil.NamespaceDirectory);
                Directory.CreateDirectory(writepath);
                File.WriteAllText(Path.Join(writepath, fil.FileName+".cs"), sourceFile);
            }
        }
    }
}