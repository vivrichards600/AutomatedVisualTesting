using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedVisualTesting
{
    public class Common
    {
        /// <summary>
        /// Check file path exists
        /// </summary>
        /// <param name="filePath">File path to check</param>
        /// <returns></returns>
        public static bool CheckFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File '" + filePath + "' not found!");
            }
            return true;
        }

    }
}
