using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Channels;
using GSharp.System.GScript;
using Newtonsoft.Json;

namespace GryphonSharpTranspiler
{
    public class GSFile
    {
        public string FileName;
        public string NameSpacePath;
        /// <summary>
        /// Script's body
        /// </summary>
        public Script Body;

        public GSFile(string path){
            Body = JsonConvert.DeserializeObject<Script>(path);
        }
    }
}