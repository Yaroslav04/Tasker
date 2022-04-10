using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.Core
{
    public static class FileManager
    {
        public static string GeneralPath()
        {
            return @"Data\";
        }

        public static string GeneralPath(string _file)
        {
            return Path.Combine(GeneralPath(), _file);
        }

        public static void FileInit()
        {
            if (!Directory.Exists(GeneralPath()))
            {
                Directory.CreateDirectory(GeneralPath());
            }
        }

    }
}
