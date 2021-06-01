using System.Collections.Generic;
using System.IO;

namespace GryphonSharpTranspiler
{
    public static class Utils
    {
        public static List<string> IterateFolder(string folder)
        {
            List<string> list = new List<string>();
            RecursiveIteration(folder, list);
            return list;
        }

        public static void RecursiveIteration(string folder, List<string> listToPopulate)
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