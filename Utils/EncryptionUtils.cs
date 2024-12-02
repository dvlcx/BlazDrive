using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazDrive.Utils
{
    public static class EncryptionUtils
    {
        public static string FormatAesKey(string origin)
        {
            var key = new string(origin.Take(43).ToArray()) + "=";
            var iv = new string(origin.Skip(30).Take(22).ToArray()) + "==";
            var result = $"<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<AesKeyValue xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\n  <key>{key}</key>\n  <iv>{iv}</iv>\n</AesKeyValue>";
            return result;
        }
    }
}