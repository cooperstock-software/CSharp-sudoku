using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SudokuProject
{
    /// <summary>
    /// A small INI file reader/writer standing in for Delphi's TMemIniFile.
    /// Like TMemIniFile it loads the whole file, edits it in memory, and writes it back on Save,
    /// keeping any sections, keys and comments it does not know about (e.g. a hand-edited Debug=1).
    /// Booleans are stored as 1/0, exactly as the Delphi version did, so existing sudoku.ini
    /// files keep working.
    /// </summary>
    internal sealed class IniFile
    {
        private readonly string _path;
        private readonly List<string> _lines = new List<string>();

        public IniFile(string path)
        {
            _path = path;
            if (File.Exists(path))
                _lines.AddRange(File.ReadAllLines(path));
        }

        public string ReadString(string section, string key, string defaultValue)
        {
            int index = FindKey(section, key, out _);
            if (index < 0) return defaultValue;

            string line = _lines[index];
            return line.Substring(line.IndexOf('=') + 1).Trim();
        }

        // TIniFile.ReadInteger uses StrToIntDef: anything that isn't an integer gives the default.
        public int ReadInteger(string section, string key, int defaultValue)
        {
            string value = ReadString(section, key, null);
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
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
            string newLine = key + "=" + value;
            int index = FindKey(section, key, out int lastLineOfSection);

            if (index >= 0)
            {
                _lines[index] = newLine;
            }
            else if (lastLineOfSection >= 0)
            {
                _lines.Insert(lastLineOfSection + 1, newLine);
            }
            else
            {
                if (_lines.Count > 0 && _lines[_lines.Count - 1].Trim().Length > 0)
                    _lines.Add("");
                _lines.Add("[" + section + "]");
                _lines.Add(newLine);
            }
        }

        public void WriteInteger(string section, string key, int value)
        {
            WriteString(section, key, value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteBool(string section, string key, bool value)
        {
            WriteString(section, key, value ? "1" : "0");
        }

        /// <summary>Write the in-memory contents back to the file (TMemIniFile.UpdateFile).</summary>
        public void Save()
        {
            File.WriteAllLines(_path, _lines);
        }

        /// <summary>
        /// Find the line holding section/key. Returns its index, or -1 if there isn't one.
        /// lastLineOfSection is set to the index of the last non-blank line of the section
        /// (or -1 if the section doesn't exist), so new keys can be appended to it.
        /// </summary>
        private int FindKey(string section, string key, out int lastLineOfSection)
        {
            lastLineOfSection = -1;
            bool inSection = false;
            int found = -1;

            for (int i = 0; i < _lines.Count; i++)
            {
                string line = _lines[i].Trim();

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    string name = line.Substring(1, line.Length - 2).Trim();
                    inSection = string.Equals(name, section, StringComparison.OrdinalIgnoreCase);
                    if (inSection) lastLineOfSection = i;
                    continue;
                }

                if (!inSection || line.Length == 0)
                    continue;

                lastLineOfSection = i;

                if (found < 0 && !line.StartsWith(";"))
                {
                    int eq = line.IndexOf('=');
                    if (eq > 0 && string.Equals(line.Substring(0, eq).Trim(), key, StringComparison.OrdinalIgnoreCase))
                        found = i;
                }
            }

            return found;
        }
    }
}
