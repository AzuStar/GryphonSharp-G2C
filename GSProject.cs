using System.IO;
using System.Runtime.Serialization;

namespace GryphonSharpTranspiler
{
    public class GSProject
    {
        public int version;
        public string src;
        public string bin;

        [OnDeserialized]
        private void PostDesirialize()
        {
            src = Path.GetFullPath(src);
        }
    }
}