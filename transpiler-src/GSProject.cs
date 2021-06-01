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

        [JsonIgnore]
        public string root;
        [JsonIgnore]
        public List<GSFile> files = new List<GSFile>();

        public void TranspileScripts()
        {
            foreach (GSFile fil in files)
            {
                
            }
        }
    }
}