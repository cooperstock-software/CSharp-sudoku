using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SudokuProject
{
    /// <summary>
    /// A small INI file reader/writer.
    /// Booleans are stored as 1/0, exactly as the Delphi version did, so existing sudoku.ini
    /// files keep working.
    /// </summary>
    internal sealed class IniFile
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern uint GetPrivateProfileString(
                    string section, string key, string defaultValue,
                    StringBuilder result, uint size, string fileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileString(
            string section, string key, string value, string fileName);

        private readonly string _path;
        private static string CurrentDirectory => Environment.CurrentDirectory;

        public IniFile(string path)
        {
            // Must be a full path: with a bare file name these APIs look in the Windows
            // directory, not the current directory.
            _path = CurrentDirectory + "\\" + path;
        }

        public string ReadString(string section, string key, string defaultValue)
        {
            StringBuilder result = new(512);
            GetPrivateProfileString(section, key, defaultValue, result, (uint)result.Capacity, _path);
            return result.ToString();
        }

        // TIniFile.ReadInteger uses StrToIntDef: anything that isn't an integer gives the default.
        public int ReadInteger(string section, string key, int defaultValue)
        {
            return int.TryParse(ReadString(section, key, null), NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
                ? result
                : defaultValue;
        }

        // TIniFile.ReadBool is ReadInteger(...) <> 0.
        public bool ReadBool(string section, string key, bool defaultValue)
        {
            return ReadInteger(section, key, defaultValue ? 1 : 0) != 0;
        }

        public void WriteString(string section, string key, string value)
        {
            if (!WritePrivateProfileString(section, key, value, _path))
                throw new IOException(new Win32Exception(Marshal.GetLastWin32Error()).Message);
        }

        public void WriteInteger(string section, string key, int value)
        {
            WriteString(section, key, value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteBool(string section, string key, bool value)
        {
            WriteString(section, key, value ? "1" : "0");
        }
    }
}
