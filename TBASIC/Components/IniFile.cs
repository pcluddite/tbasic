using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Tbasic.Components {
    /// <summary>
    /// Create a New INI file to store or load data
    /// </summary>
    internal class IniFile {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileSection(string section,
         byte[] retVal, int size, string filePath);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer,
           uint nSize, string lpFileName);

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <PARAM name="INIPath"></PARAM>
        public IniFile(string INIPath) {
            path = INIPath;
        }
        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// Section name
        /// <PARAM name="Key"></PARAM>
        /// Key Name
        /// <PARAM name="Value"></PARAM>
        /// Value Name
        public void IniWriteValue(string Section, string Key, string Value) {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key) {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp,
                                            255, this.path);
            return temp.ToString();

        }
        /// <summary>
        /// Read Section From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, string>> ReadSection(string Section) {
            Regex keyValPair = new Regex(@"\s*([^=]*)=(.*)");
            Regex sectionRegEx = new Regex(@"\s*\[([^]]*)\]\s*");
            using (System.IO.StreamReader reader = new System.IO.StreamReader(path)) {
                Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
                string line, currentSection = null;
                while ((line = reader.ReadLine().Trim()) != null) {
                    if (line.Equals("")) {
                        continue;
                    }
                    Match m = sectionRegEx.Match(line);
                    if (m.Success) {
                        currentSection = m.Value;
                        result.Add(currentSection, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                    }
                    else if (currentSection != null) {
                        m = keyValPair.Match(line);
                        if (m.Success) {
                            result[currentSection].Add(m.Groups[1].Value, m.Groups[2].Value);
                        }
                    }

                }
                return result;
            }
        }

        public string SectionNames() {
            uint MAX_BUFFER = 32767;
            IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER);
            uint bytesReturned = GetPrivateProfileSectionNames(pReturnedString, MAX_BUFFER, this.path);
            if (bytesReturned == 0) {
                Marshal.FreeCoTaskMem(pReturnedString);
                return null;
            }
            string local = Marshal.PtrToStringAnsi(pReturnedString, (int)bytesReturned).ToString();
            Marshal.FreeCoTaskMem(pReturnedString);
            //use of Substring below removes terminating null for split
            return local.Substring(0, local.Length - 1);//.Split('\0');
            //return bytesReturned.ToString();
        }
    }
}