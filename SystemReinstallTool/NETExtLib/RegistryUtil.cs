using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETExtLib
{
    public class RegistryUtil
    {
        public static void Export(string exportPath, string registryPath)
        { 
            string path = "\""+ exportPath + "\"";
            string key = "\""+ registryPath + "\"";
            Util.StartProcessToEnd("regedit.exe", "/e " + path + " " + key + " ");
        }
    }
}
