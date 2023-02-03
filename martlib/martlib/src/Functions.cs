using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace martlib.src
{
    public static class Functions
    {
        /// <summary>
        /// Performs an operation on all files in a directory. 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="action"></param>
        public static void Seek(string directory, Func<string, int> action)
        {
            string[] files = Directory.GetFiles(directory);
            for (int i = 0; i < files.Length; i++)
            {
                action(files[i]);
            }

            string[] folders = Directory.GetDirectories(directory);
            for (int i = 0; i < folders.Length; i++)
            {
                Seek(folders[i], action);
            }
        }
    }
}
