using System;
using System.IO;

namespace SudokuProject
{
    /// <summary>The three solving strategies selectable on the Options form (Delphi: Methods).</summary>
    internal enum SolveMethod
    {
        SimplePromptBacktrack = 1,
        SimpleAutoBacktrack = 2,
        OnlyBacktrack = 3
    }

    /// <summary>
    /// Program-wide settings and constants. Replaces Globals.pas.
    /// The settings persist in sudoku.ini in the current directory, as before.
    /// </summary>
    internal static class Globals
    {
        // Settings (Delphi: unit-level variables)
        public static SolveMethod Method;          // selection from the methods above
        public static bool ShowSmallDigits;        // do we show the small digits, or not?
        public static bool DoSleeps;               // do we sleep after each solved (or proposed) digit?
        public static bool ReportDuration;         // do we report the # of seconds taken for a solve?
        public static bool DebugMode;              // are we in debug mode (INI key "Debug")
        public static bool UseC;                   // use our C DLL for the backtracking (never implemented)
        public static bool ShowAttempts;           // in backtracking code, show digits being considered

        // Values for the "verbose" argument of CheckAllowedResult
        public const bool Verbose = true;
        public const bool Silent = false;

        // Values for the "isOriginal" argument to a couple of cell functions
        public const bool Original = true;
        public const bool Solved = false;

        // Values for the "possible" argument to a cell function
        public const bool Possible = true;
        public const bool Impossible = false;

        public const string FileName = "sudoku.ini";

        /// <summary>Read the values in from the ini file.</summary>
        public static void ReadIni()
        {
            IniFile iniFile = new IniFile(FileName);

            Method = (SolveMethod)iniFile.ReadInteger("All", "Method", (int)SolveMethod.SimplePromptBacktrack);
            ShowSmallDigits = iniFile.ReadBool("All", "ShowSmallDigits", true);
            DoSleeps = iniFile.ReadBool("All", "DoSleeps", true);
            ReportDuration = iniFile.ReadBool("All", "ReportDuration", false);
            ShowAttempts = iniFile.ReadBool("All", "ShowAttempts", false);
            DebugMode = iniFile.ReadBool("All", "Debug", false);
        }

        /// <summary>Write the current values back to the INI file.</summary>
        public static void WriteIni()
        {
            IniFile iniFile = new IniFile(FileName);

            iniFile.WriteInteger("All", "Method", (int)Method);
            iniFile.WriteBool("All", "ShowSmallDigits", ShowSmallDigits);
            iniFile.WriteBool("All", "DoSleeps", DoSleeps);
            iniFile.WriteBool("All", "ReportDuration", ReportDuration);
            iniFile.WriteBool("All", "ShowAttempts", ShowAttempts);
            // iniFile.WriteBool("All", "Debug", DebugMode); // only set this manually

            try
            {
                iniFile.Save();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                Dialogs.ShowMessage("Unable to save the settings to " + Path.GetFullPath(FileName) +
                                    "." + Environment.NewLine + "Error: " + ex.Message);
            }
        }
    }
}
