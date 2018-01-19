// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Handles Macros in PowerMILL.
    /// </summary>
    public class PMMacro : Macro
    {
        #region Fields

        /// <summary>
        /// The instance of PowerMILL used to run the macro.
        /// </summary>
        protected PMAutomation _powerMILL;

        /// <summary>
        /// The options to run a macro. By default is set to send all macro lines as individual commands.
        /// </summary>

        #endregion
        protected SubMacroRunOptions _RunSubMacrosOption = SubMacroRunOptions.RunAllLines;

        #region Public Enums

        /// <summary>
        /// Determines whether a macro is run sub macro directly with no substitution (RunAsSingleLine) or send all macro lines as individual command (RunAllLines).
        /// </summary>
        public enum SubMacroRunOptions
        {
            /// <summary>
            /// Macro is run sub macro directly with no substitution
            /// </summary>
            RunAsSingleLine,

            /// <summary>
            /// Send all macro lines as individual command
            /// </summary>
            RunAllLines
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new macro and reads the Macro File in.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="macroFile">The file with the macro to be loaded.</param>
        protected internal PMMacro(PMAutomation powerMILL, FileSystem.File macroFile)
        {
            _lines = new List<string>();
            if (macroFile != null)
            {
                // Break the file down into lines
                _lines = macroFile.ReadTextLines();
            }
            ConfigureMacro(powerMILL);
        }

        private void ConfigureMacro(PMAutomation powerMILL)
        {
            _localIndex = 0;
            _powerMILL = powerMILL;
            _cancel = false;
            _executedCommands = new List<string>();
            DoNotExecuteCommentsOrBlankLines = false;
            UseExecuteEx = false;
            _RunSubMacrosOption = SubMacroRunOptions.RunAllLines;
        }

        /// <summary>
        /// Creates a new macro from a list of strings.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="macros">a list of macros to be loaded.</param>
        protected internal PMMacro(PMAutomation powerMILL, params string[] macros)
        {
            _lines = new List<string>();
            if (macros.Any())
            {
                _lines = macros.ToList();
            }

            ConfigureMacro(powerMILL);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets, sets whether an embedded macro is run as a single command or treated as a sub macro and stepped through.
        /// </summary>
        public SubMacroRunOptions RunSubMacrosOption
        {
            get { return _RunSubMacrosOption; }
            set { _RunSubMacrosOption = value; }
        }

        /// <summary>
        /// Gets the number of lines in the macro (includes lines in called macros).
        /// </summary>
        public override int TotalCount
        {
            get
            {
                int count = 0;
                foreach (string line in _lines)
                {
                    count += 1;
                    if (RunSubMacrosOption == SubMacroRunOptions.RunAllLines && line.ToUpper().StartsWith("MACRO"))
                    {
                        int index = line.IndexOf("\"") + 1;
                        PMMacro childMacro = null;
                        string macroFilepath = line.Substring(index, line.LastIndexOf("\"") - index);
                        if (macroFilepath.IndexOf("{") > -1)
                        {
                            macroFilepath = ReplaceTokens(macroFilepath)[0];
                        }
                        childMacro = new PMMacro(_powerMILL, new FileSystem.File(macroFilepath));
                        count += childMacro.TotalCount;
                    }
                }
                return count;
            }
        }

        /// <summary>
        /// If true then if the macro step to be run is a comment or a blank line then it does not send the command to PowerMill.
        /// </summary>
        public bool DoNotExecuteCommentsOrBlankLines { get; set; }

        /// <summary>
        /// If true then macro commands in the macro is sent to PowerMill using the ExecuteEX() command.
        /// This can be used to capture the result of the command being sent for debugging purposes.
        /// </summary>
        public bool UseExecuteEx { get; set; }

        #endregion

        #region Operations

        /// <summary>
        /// Executes the next line in the macro and increments the index marker.
        /// </summary>
        /// <param name="hideDialogs">If true, hides all dialogs.</param>
        public override void RunStep(bool hideDialogs = true)
        {
            List<string> commands = ReplaceTokens(_lines[_localIndex]);
            string trimLine = "";
            var results = new List<string>();
            foreach (string item in commands)
            {
                var command = item;

                // Handle the case where the line is trying to open a file
                if (hideDialogs && command.ToUpper().Contains("FILEOPEN"))
                {
                    //Macro could have filename on same line or next line.
                    trimLine = command.ToUpper().Trim();

                    // remove spaces
                    string[] works = trimLine.Split(' ');

                    //check last word
                    if (works[works.Length - 1] == "FILEOPEN")
                    {
                        // Instead of launching a dialog box, concatonate the two lines removing the FILEOPEN command
                        command = command.Substring(0, command.ToUpper().IndexOf("FILEOPEN")) +
                                  ReplaceTokens(_lines[_localIndex + 1])[0];
                        _localIndex += 2;
                        _totalIndex += 2;

                        //single line
                    }
                    else
                    {
                        //do nothing as already on line
                        _localIndex += 1;
                        _totalIndex += 1;
                    }

                    //_powerMILL.DoCommand(command)
                }
                else if (hideDialogs && command.ToUpper().Contains("FILESAVE"))
                {
                    trimLine = command.ToUpper().Trim();

                    // remove spaces
                    string[] works = trimLine.Split(' ');

                    //check last word
                    if (works[works.Length - 1] == "FILESAVE")
                    {
                        // Instead of launching a dialog box, concatonate the two lines removing the FILESAVE command
                        command = command.Substring(0, command.ToUpper().IndexOf("FILESAVE")) +
                                  ReplaceTokens(_lines[_localIndex + 1])[0];
                        _localIndex += 2;
                        _totalIndex += 2;

                        //single line
                    }
                    else
                    {
                        //do nothing as already on line
                        _localIndex += 1;
                        _totalIndex += 1;
                    }
                }
                else if (hideDialogs && command.ToUpper().Contains("TMPLTSELECTORGUI"))
                {
                    trimLine = command.ToUpper().Trim();

                    // remove spaces
                    string[] works = trimLine.Split(' ');

                    //check last word
                    if (works[works.Length - 1] == "TMPLTSELECTORGUI")
                    {
                        // Instead of launching a dialog box, concatonate the two lines removing the TMPLTSELECTORGUI command
                        command = command.Substring(0, command.ToUpper().IndexOf("TMPLTSELECTORGUI")) +
                                  ReplaceTokens(_lines[_localIndex + 1])[0];
                        _localIndex += 2;
                        _totalIndex += 2;

                        //single line
                    }
                    else
                    {
                        //do nothing as already on line
                        _localIndex += 1;
                        _totalIndex += 1;
                    }
                }
                else if (hideDialogs && command.ToUpper().StartsWith("FORM"))
                {
                    // Ignore first two words
                    if (command.IndexOf(" ", 5) != -1)
                    {
                        command = command.Substring(command.IndexOf(" ", 5) + 1);

                        //_powerMILL.DoCommand(command)
                    }
                    else
                    {
                        command = "$$$NothingToDo$$$";
                    }
                    _localIndex += 1;
                    _totalIndex += 1;
                }
                else if (hideDialogs && command.ToUpper().Contains(" FORM "))
                {
                    // Ignore the bit from the form command onward
                    command = command.Substring(0, command.ToUpper().IndexOf(" FORM "));

                    //_powerMILL.DoCommand(command)
                    _localIndex += 1;
                    _totalIndex += 1;
                }
                else if (command.ToUpper().StartsWith("MACRO"))
                {
                    //In some cases sub macros should be run as a single line. E.g they contain loops
                    if (RunSubMacrosOption == SubMacroRunOptions.RunAllLines)
                    {
                        _childMacro = new PMMacro(_powerMILL,
                                                  new FileSystem.File(command.Substring(command.IndexOf("\"") + 1,
                                                                                        command.LastIndexOf("\"") -
                                                                                        command.IndexOf("\"") - 1)));
                        _childMacro.Run();
                        command = "$$ " + command;
                        _totalIndex += 1 + _childMacro.TotalCount;
                    }
                    else
                    {
                        _totalIndex += 1;
                    }
                    _localIndex += 1;
                }
                else if (command.ToUpper().StartsWith("RUNMACRO"))
                {
                    // Remove the "RUN" from the start and just run as a macro
                    command = command.Substring(3);
                    _localIndex += 1;
                    _totalIndex += 1;
                }
                else
                {
                    // Otherwise just run the command
                    _localIndex += 1;
                    _totalIndex += 1;
                }
                bool bIgnoreCmdLine = false;
                if (DoNotExecuteCommentsOrBlankLines)
                {
                    string strCmd = command.Trim();

                    // remove whitespace
                    if (string.IsNullOrEmpty(strCmd) || strCmd.StartsWith("$$") || strCmd.StartsWith("//"))
                    {
                        //must use start with because have to account for case where comment is on right of a command.i.e
                        // PRINT ENTITY 'fred' $$ a comment
                        bIgnoreCmdLine = true;
                    }
                }
                if (command != "$$$NothingToDo$$$" && bIgnoreCmdLine == false)
                {
                    _executedCommands.Add(command);
                    if (UseExecuteEx == false)
                    {
                        _powerMILL.DoCommand(command);
                    }
                    else
                    {
                        // capture the result of the command for potential debugging purposes
                        string result = _powerMILL.DoCommandEx(command).ToString();
                        if (!string.IsNullOrEmpty(result.Trim()))
                        {
                            _executedCommands.Add("$$ MESSAGE INVOKED:");

                            //EchoCommand("$$ MESSAGE INVOKED:")
                            foreach (
                                string ritem in
                                result.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries))
                            {
                                _executedCommands.Add("$$ " + ritem);

                                //EchoCommand("$$ " + ritem)
                            }
                        }
                    }
                }
            }

            // Rejig the counters for where extra lines were inserted because of the ReplaceTokens command
            if (commands.Count > 1)
            {
                _localIndex -= commands.Count - 1;
                _totalIndex -= commands.Count - 1;
            }
        }

        /// <summary>
        /// Executes the macro until the end, optionally feeding back progress as it goes.
        /// </summary>
        /// <param name="giveProgress">If true it gives a progress status.</param>
        /// <param name="hideDialogs">If true it hides any dialogs.</param>
        public override void Run(bool giveProgress = true, bool hideDialogs = true)
        {
            while ((_localIndex < LocalCount) & (_cancel == false))
            {
                RunStep(hideDialogs);
                if (giveProgress)
                {
                    RaiseStepProcessedEvent(this, new MacroStepEventArgs(_totalIndex, TotalCount));
                }
            }
        }

        /// <summary>
        /// Executes the macro in one go rather than line by line.
        /// </summary>
        public override void RunComplete()
        {
            // Replace substitution tokens
            List<string> linesToExecute = new List<string>();
            foreach (string lineToParse in _lines)
            {
                linesToExecute.AddRange(ReplaceTokens(lineToParse));
            }
            FileSystem.File tempFile = FileSystem.File.CreateTemporaryFile("mac");
            tempFile.WriteTextLines(linesToExecute);
            _powerMILL.DoCommand("MACRO \"" + tempFile.Path + "\"");
            tempFile.Delete();
        }

        #endregion

        #region Private Operations

        /// <summary>
        /// Replaces the tokens in the passed in command and returns the result.
        /// </summary>
        /// <param name="command">The command to be modified.</param>
        /// <returns>The modified command.</returns>
        protected virtual List<string> ReplaceTokens(string command)
        {
            string modifiedCommand = command;
            foreach (PMSubstitutionToken token in _powerMILL.SubstitutionTokens)
            {
                modifiedCommand = modifiedCommand.Replace(token.Token, token.Value);
            }
            return new List<string>(new[] {modifiedCommand});
        }

        #endregion
    }
}