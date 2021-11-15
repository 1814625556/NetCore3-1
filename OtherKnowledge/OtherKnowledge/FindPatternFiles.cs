using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherKnowledge
{
    public class FindPatternFiles
    {
        public static List<string> GetFileListWithExtend(DirectoryInfo directory, string pattern)
        {
            List<string> pathList = new List<string>();
            string result = String.Empty;
            if (directory.Exists || pattern.Trim() != string.Empty)
            {

                foreach (FileInfo info in directory.GetFiles(pattern))
                {
                    result = info.FullName.ToString();
                    pathList.Add(result);
                }
            }
            return pathList;

        }
    }
}
