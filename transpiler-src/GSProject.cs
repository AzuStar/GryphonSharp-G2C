using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace GryphonSharpTranspiler
{
    public class GSProject
    {
        public int version;
        public string src;
        public string bin;

        [JsonIgnore]
        public string root;
        [JsonIgnore]
        public List<GSFile> files = new List<GSFile>();
    }
}