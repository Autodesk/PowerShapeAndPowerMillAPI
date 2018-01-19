// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;
using System.Linq;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Handles Macros in PowerSHAPE.
    /// </summary>
    public class PSMacro : Macro
    {
        #region " Fields "

        /// <summary>
        /// The instance of PowerSHAPE will run the macro on.
        /// </summary>
        protected PSAutomation _powerSHAPE;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Constructor reads the Macro File in.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="macroFile">The file path to the macro.</param>
        /// <remarks></remarks>
        protected internal PSMacro(PSAutomation powerSHAPE, FileSystem.File macroFile)
        {
            if (macroFile != null)
            {
                // Break the file down into lines
                _lines = macroFile.ReadTextLines();
            }
            else
            {
                _lines = new List<string>();
            }
            ConfigureMacro(powerSHAPE);
        }

        protected void ConfigureMacro(PSAutomation powerShape)
        {
            _localIndex = 0;
            _powerSHAPE = powerShape;
            _cancel = false;
            _executedCommands = new List<string>();
        }

        /// <summary>
        /// Creates a new macro from a list of strings.
        /// </summary>
        /// <param name="psAutomation">The base instance to interact with PowerShape.</param>
        /// <param name="macros">a list of macros to be loaded.</param>
        protected internal PSMacro(PSAutomation psAutomation, params string[] macros)
        {
            _lines = new List<string>();
            if (macros.Any())
            {
                _lines = macros.ToList();
            }

            ConfigureMacro(psAutomation);
        }

        #endregion

        #region " Properties "

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
                    if (line.ToUpper().StartsWith("MACRO RUN"))
                    {
                        int index = line.IndexOf("\"") + 1;
                        PSMacro childMacro = null;
                        childMacro = new PSMacro(_powerSHAPE,
                                                 new FileSystem.File(line.Substring(index, line.LastIndexOf("\"") - index)));
                        count += childMacro.TotalCount;
                    }
                }

                return count;
            }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Executes the next line in the macro and increments the index marker.
        /// </summary>
        /// <param name="hideDialogs">If true, hides any dialogs while running commands.</param>
        public override void RunStep(bool hideDialogs = true)
        {
            string command = _lines[_localIndex];
            string trimLine = "";

            // Run the command
            _localIndex += 1;
            _totalIndex += 1;

            _executedCommands.Add(command);
            _powerSHAPE.DoCommand(command);
        }

        /// <summary>
        /// Executes the macro until the end, optionally feeding back progress as it goes.
        /// </summary>
        /// <param name="giveProgress">If true, it will give progress status.</param>
        /// <param name="hideDialogs">If true, hides any dialogs while running commands.</param>
        /// <remarks></remarks>
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
            FileSystem.File tempFile = FileSystem.File.CreateTemporaryFile("mac");
            tempFile.WriteTextLines(_lines);

            _powerSHAPE.DoCommand("MACRO RUN \"" + tempFile.Path + "\"");

            // Wait for the macro to finish
            System.Threading.Thread.Sleep(1000);
            while (_powerSHAPE.IsBusy()) System.Threading.Thread.Sleep(1000);

            tempFile.Delete();
        }

        #endregion
    }
}