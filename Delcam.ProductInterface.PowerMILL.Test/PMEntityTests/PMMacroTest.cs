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
using Autodesk.FileSystem;
using Autodesk.ProductInterface.PowerMILL;
using Autodesk.ProductInterface.PowerMILLTest.Files;
using Autodesk.ProductInterface.PowerMILLTest.HelperClasses;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.PMEntityTests
{
    [TestFixture]
    public class PMMacroTest : PMEntityTestBase
    {
        #region Tests

        /// <summary>
        /// Tests that whilst a macro is running the step event is fired.
        /// </summary>
        [Test]
        public void MacroStepEventFired()
        {
            _powerMILL.IssueEchoOffCommands = true;

            Directory existingPM = LoadCopyOfPowerMillProject(TestFiles.PMwithNcProgram1, true);
            _TmpFoldersToDelete.Add(existingPM);
            File macFile = TestFiles.TestMacroStepEventFires;
            _powerMILL.LoadProject(existingPM);

            PMMacro myMac = _powerMILL.LoadMacro(macFile);
            MacroEventCatcher catcher = new MacroEventCatcher(myMac);
            catcher.Run();

            Assert.Greater(catcher.DataList.Count, 0, "Expecting more than 0 events");
            Assert.AreEqual(catcher.DataList.Count, 24, "Expected 24 events!");
        }

        [Test]
        public void RecordMacroTest()
        {
            File tmpMacFile = File.CreateTemporaryFile(".mac");
            tmpMacFile.Delete();
            _powerMILL.RecordMacro(tmpMacFile.Path);
            _powerMILL.Execute("CREATE PATTERN 'fred'");

            _powerMILL.StopMacroRecording();

            Assert.True(tmpMacFile.Exists, "macro file should exist");

            string[] lines = System.IO.File.ReadAllLines(tmpMacFile.Path);
            if (lines == null || lines.Length < 1)
            {
                Assert.Fail("Macro file is empty");
            }

            tmpMacFile.Delete();
        }

        [Test]
        public void RunMacroAsSingleCommandTest()
        {
            File macFile = File.CreateTemporaryFile("mac");
            TestFiles.TestMacro2File.Copy(macFile);
            TestFiles.TestMacro3File.Copy(new File(macFile.ParentDirectory, TestFiles.TestMacro3File.Name));
            _powerMILL.SubstitutionTokens.Add(new PMSubstitutionToken("{MACROPATH}", macFile.ParentDirectory.Path));

            PMMacro mainMac = _powerMILL.LoadMacro(macFile);

            int nLines = mainMac.TotalCount; //This should include sub macro lines
            MacroEventCatcher catcher = new MacroEventCatcher(mainMac);
            catcher.Run();
            int nCatcherEventCaptured = catcher.DataList.Count;
            Assert.AreEqual(nLines, nCatcherEventCaptured, "Not run as embedded macro");

            PMMacro macro2 = _powerMILL.LoadMacro(macFile);

            macro2.RunSubMacrosOption = PMMacro.SubMacroRunOptions.RunAsSingleLine;
            int nLines2 = macro2.TotalCount; //This should include sub macro as 1 line
            catcher = new MacroEventCatcher(macro2);
            catcher.Run();

            Assert.AreEqual(nLines2, catcher.DataList.Count, "Not run as single line macro");
            Assert.AreNotEqual(nLines, nLines2, "Should not have the same number of commands ran");
        }

        [Test]
        public void ShouldRunMacrosFromAListOfStrings_UsingRunMacro()
        {
            var macroText = TestFiles.CreateThreeWorkplaneMacro.ReadTextLines();
            var macro = _powerMILL.LoadMacro(macroText.ToArray());
            var actualWorkplaneNames = new[] {"WP1", "WP2", "WP3"};

            _powerMILL.RunMacro(macro);
            _powerMILL.ActiveProject.Refresh();

            AssertWorkplanes(actualWorkplaneNames, _powerMILL.ActiveProject.Workplanes);
        }

        [Test]
        public void ShouldRunMacrosFromAString_UsingLoadMacro()
        {
            var macroText = TestFiles.CreateThreeWorkplaneMacro.ReadTextLines();
            var actualWorkplaneNames = new[] {"WP1", "WP2", "WP3"};
            var macro = _powerMILL.LoadMacro(string.Join(Environment.NewLine, macroText));

            macro.Run();
            _powerMILL.ActiveProject.Refresh();

            AssertWorkplanes(actualWorkplaneNames, _powerMILL.ActiveProject.Workplanes);
        }

        private void AssertWorkplanes(ICollection<string> actualWorkplaneNames, ICollection<PMWorkplane> workplanesToTest)
        {
            Assert.That(workplanesToTest.Count, Is.EqualTo(actualWorkplaneNames.Count));
            var results = workplanesToTest.Select(w => w.Name).ToList();
            CollectionAssert.IsSubsetOf(actualWorkplaneNames, results);
        }

        #endregion
    }
}