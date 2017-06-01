using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NETExtLib
{
    public class FileUtil
    {
        public static void RemoveReadOnlyOfFile(string path)
        {
            if (!File.Exists(path)) return;
            FileAttributes attributes = File.GetAttributes(path);

            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                // Make the file RW
                attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                File.SetAttributes(path, attributes);
                // Console.WriteLine("The {0} file is no longer RO.", path);
            }
        }

        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }
    }
}

