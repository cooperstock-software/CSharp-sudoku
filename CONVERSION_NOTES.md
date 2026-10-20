# Sudoku: Delphi → C# (Windows Forms) conversion notes

## Opening it

Open **`SudokuProject.sln`** in Visual Studio 2022 (17.8 or later) or Visual Studio 2026, with the
".NET desktop development" workload installed, and press F5. The project targets `net8.0-windows`;
to use a newer runtime change `TargetFramework` in `SudokuProject.csproj`.

`sudoku.pdf` (the Help file) is copied next to the .exe on build, and `sudoku.ini` is created in the
current directory the first time you save Options — same as the Delphi program.

## File mapping

| Delphi | C# |
|---|---|
| `SudokuProject.dpr` | `Program.cs` |
| `SudokuProject.dproj` | `SudokuProject.csproj` + `SudokuProject.sln` |
| `SudokuProject.res` (MAINICON) | `SudokuProject.ico` (extracted from the .res, set as `ApplicationIcon`) |
| `SudokuBase.pas` / `.dfm` | `SudokuBaseForm.cs` / `.Designer.cs` / `.resx` |
| `Sudoku6x6.pas` / `.dfm` | `Sudoku6x6Form.cs` / `.Designer.cs` / `.resx` (inherits `SudokuBaseForm`) |
| `cell.pas` / `.dfm` (TFrame) | `Cell.cs` / `.Designer.cs` / `.resx` (UserControl) |
| `Options.pas` / `.dfm` | `OptionsForm.cs` / `.Designer.cs` / `.resx` |
| `Backtrack.pas` | `Backtrack.cs` |
| `Globals.pas` | `Globals.cs` (+ `IniFile.cs`, replacing `TMemIniFile`) |
| `ShowMessage` / `MessageDlg` | `Dialogs.cs` |
| `sudoku.pdf`, `sudoku.docx` | `sudoku.pdf` (copied to output), `docs/sudoku.docx` |

## How the Delphi constructs were mapped

* **.dfm → Designer.cs.** Every control's position, size, caption, tab order, font and event hookup is
  in the `.Designer.cs` files, so the forms open in the Visual Studio designer. Delphi pixel-height fonts
  (`Font.Height = -17`) became pixel-unit fonts, so text sizes match.
* **`Sudoku6x6.dfm` (`inherited`)** → visual inheritance. The base form's controls are `protected`, and
  `Sudoku6x6Form.Designer.cs` lists only the properties that differ, as the .dfm did.
* **`OnCreate`** → `Load`. The 6x6 form's `FormCreate6x6` replaced the inherited handler in the .dfm; here
  `Sudoku6x6Form.FormCreate` overrides the base form's virtual `FormCreate`.
* **`OnShortCut`** (Tab / arrows / Home / End / PgUp / PgDn) → `ProcessCmdKey`; typed digits stay in
  `KeyPress` with `KeyPreview` on.
* **`TShape`** → a black `Panel`. **`TRadioGroup`** → `GroupBox` with three `RadioButton`s. **`ModalResult`** →
  `DialogResult`. **`Application.ProcessMessages`** → `Application.DoEvents`.
* **Global form variables** (`SudokuBaseForm`, `Sudoku6x6Form`, `OptionsForm`) → `Program.MainForm`,
  `Program.SixBySixForm`, `Program.OptionsDialog`.
* Two controls were renamed because the Delphi names collide with built-in `Form` properties:
  `HelpButton` → `HelpFileButton`, and the Options dialog's `SaveButton`/`CancelButton` →
  `SaveOptionsButton`/`CancelOptionsButton`. The event handler names are unchanged.
* Arrays stay 1-based (index 0 unused) so the row/column/box arithmetic is identical to the Delphi code.

## Deliberate differences

* **Tab/arrow keys before any square is selected:** the Delphi code indexed `cells[0, …]`
  (out of range). Now the first press selects square 1,1.
* **Window creation:** Delphi built both puzzle windows at startup; here the 6x6 grid is built the first time
  that window is shown. `ReadIni` runs once in `Program.Main` rather than in each form's create handler.
* **Double-clicking a small digit:** VCL sends Click, DblClick, Click; Windows Forms sends Click, DoubleClick.
  `Cell.DblClickSmallNumber` replays the trailing click so the digit's possible/impossible state ends up
  unchanged, as before.
* **DPI:** the layout is fixed-pixel, so the app runs DPI-unaware (Windows scales the window). See `Program.cs`.
* Small digit labels are 12 px wide instead of 10, so GDI text padding can't clip the glyph.
* The Maximize button is disabled on the two puzzle windows (fixed layout).
* The `~n~n` in the (currently unreachable) stepping prompt of `SolveRowsColsBoxes` was a leftover from
  the PowerBuilder original; it is now a real blank line.
* A failure to write `sudoku.ini` now shows a message instead of an unhandled exception.
* `ReportMemoryLeaksOnShutdown` has no .NET equivalent and was dropped.

## Quirks of the original that were kept as they are

* The 6x6 window's title is still `Sudoku6x6Form`.
* Save doesn't remember the chosen file name for next time (only Load does).
* Cancelling the Options dialog doesn't revert checkbox edits you made in that session (the dialog is reused).
* `reportDuration` is switched off in memory if you decline the backtracking prompt.
