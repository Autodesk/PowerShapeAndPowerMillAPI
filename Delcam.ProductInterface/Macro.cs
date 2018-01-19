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

namespace Autodesk.ProductInterface
{
    /// <summary>
    /// Abstract base class for encapsulating a Macro and all operations thereon.
    /// </summary>
    public abstract class Macro
    {
        #region " Fields "

        /// <summary>
        /// Macro command lines.
        /// </summary>
        protected List<string> _lines;

        /// <summary>
        /// Current index within this Macro.
        /// </summary>
        protected int _localIndex;

        /// <summary>
        /// Current index within this file - including all sub macros.
        /// </summary>
        protected int _totalIndex;

        /// <summary>
        /// If True, indicates that a Macro cancellation is pending.
        /// </summary>
        protected bool _cancel;

        /// <summary>
        /// Currently execturing child Macro.
        /// </summary>
        private Macro withEventsField__childMacro;

        protected Macro _childMacro
        {
            get { return withEventsField__childMacro; }
            set
            {
                if (withEventsField__childMacro != null)
                {
                    withEventsField__childMacro.StepProcessed -= objChildMacro_StepProcessed;
                }
                withEventsField__childMacro = value;
                if (withEventsField__childMacro != null)
                {
                    withEventsField__childMacro.StepProcessed += objChildMacro_StepProcessed;
                }
            }
        }

        /// <summary>
        /// List of commands that have been executed.
        /// </summary>
        protected List<string> _executedCommands;

        #endregion

        #region "Constructor"

        #endregion

        #region " Events "

        /// <summary>
        /// Raised when a Macro line is processed.
        /// </summary>
        public event StepProcessedEventHandler StepProcessed;

        public delegate void StepProcessedEventHandler(object sender, MacroStepEventArgs e);

        #endregion

        #region " Properties "

        /// <summary>
        /// Number of lines in this macro, excluding children.
        /// </summary>
        public int LocalCount
        {
            get { return _lines.Count; }
        }

        /// <summary>
        /// Number of lines in this macro including children.
        /// </summary>
        public virtual int TotalCount
        {
// Just return the number of lines in this file.  Specific version will need to override
// this operation as calling one macro will differ between programs.

            get { return _lines.Count; }
        }

        /// <summary>
        /// Index of the next line to be executed in this Macro.
        /// </summary>
        public int Index
        {
            get { return _localIndex; }

            set { _localIndex = value; }
        }

        /// <summary>
        /// If True, Macro will be cancelled; False otherwise.
        /// </summary>
        public bool Cancel
        {
            get { return _cancel; }

            set
            {
                _cancel = value;

                if (_childMacro != null)
                {
                    _childMacro.Cancel = value;
                }
            }
        }

        /// <summary>
        /// All lines of the Macro in a string.
        /// </summary>
        public string Text
        {
            get
            {
                string macroText = "";

                foreach (string line in _lines)
                {
                    macroText += line + Environment.NewLine;
                }

                return macroText;
            }
        }

        /// <summary>
        /// Commands that have been executed by this Macro.
        /// </summary>
        /// <returns>The commands that have been executed by this macro</returns>
        public List<string> ExecutedCommands
        {
            get { return _executedCommands; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Will run the macro.
        /// </summary>
        /// <param name="giveProgress">If True, events will be raised reporting the Macro's progress; False otherwise.</param>
        /// <param name="hideDialogs">If True, dialogs will be hidden; False otherwise.</param>
        public abstract void Run(bool giveProgress = true, bool hideDialogs = true);

        /// <summary>
        /// Will Run the Macro from start to finish rather then line by line. Substitution strings will be replaced before the macro is run.
        /// </summary>
        public abstract void RunComplete();

        /// <summary>
        /// Will execute the next step in the Macro.
        /// </summary>
        /// <param name="hideDialogs">If True, dialogs will be hidden; False otherwise.</param>
        public abstract void RunStep(bool hideDialogs = true);

        /// <summary>
        /// Raises the step processed event.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Macro step event arguments.</param>
        protected void RaiseStepProcessedEvent(object sender, MacroStepEventArgs e)
        {
            // Raise the event
            if (StepProcessed != null)
            {
                StepProcessed(sender, e);
            }
        }

        #endregion

        #region " Event Handling "

        /// <summary>
        /// This operation handles the child macro processing a step.  It raises the event up to the owner
        /// of this macro and ensures that the index and count values are correct
        /// </summary>
        private void objChildMacro_StepProcessed(object sender, MacroStepEventArgs e)
        {
            //Pass the event up the chain of macros
            RaiseStepProcessedEvent(sender, new MacroStepEventArgs(_totalIndex + e.Index, TotalCount));
        }

        #endregion
    }
}