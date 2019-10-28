// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Linq;
using Autodesk.ProductInterface.PowerMILL;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.PMEntityTests
{
    [TestFixture]
    public class PMNCProgramTest
    {
        private PMAutomation _powerMILL;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _powerMILL = new PMAutomation(InstanceReuse.UseExistingInstance);
            _powerMILL.DialogsOff();
            _powerMILL.CloseProject();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _powerMILL.CloseProject();
            }
            catch (Exception)
            {
            }
        }

        #region Test properties

        [Test]
        public void Delete()
        {
            PMNCProgram nc = _powerMILL.ActiveProject.NCPrograms.CreateNCProgram("1");
            string ncProgramName = nc.Name;
            int nNCProgramsBeforeDelete = _powerMILL.ActiveProject.NCPrograms.Count;
            nc.Delete();
            Assert.AreEqual(nNCProgramsBeforeDelete - 1,
                            _powerMILL.ActiveProject.NCPrograms.Count,
                            "Expected 1 less NC program than prior to delete.");
            Assert.AreEqual(0,
                            _powerMILL.ActiveProject.NCPrograms.Where(x => x.Name == ncProgramName).Count(),
                            "Expected no NC program with name '1'");
        }

        [Test]
        public void Property_OutputFileName()
        {
            PMNCProgram nc = _powerMILL.ActiveProject.NCPrograms.CreateNCProgram("1");
            nc.OutputFileName = @"c:\OutputFileName2";
            var expected = "c:/OutputFileName2";

            // PowerMill 2017 gave a different style of output format
            if (_powerMILL.Version.Major == 21)
            {
                expected = "c:\\OutputFileName2";
            }
            Assert.AreEqual(expected, nc.OutputFileName, $"Expected property 'OutputFileName' to change to '{expected}'");
        }

        [Test]
        public void Property_OutputWorkplane()
        {
            PMWorkplane wp = _powerMILL.ActiveProject.Workplanes.CreateWorkplane(new Geometry.Workplane());
            PMNCProgram nc = _powerMILL.ActiveProject.NCPrograms.CreateNCProgram("1");
            nc.OutputWorkplaneName = wp.Name;
            Assert.AreEqual(wp.Name,
                            nc.OutputWorkplaneName,
                            String.Format("Expected property 'OutputWorkplaneName' to change to '{0}'", wp.Name));
        }

        [Test]
        public void Property_ProgramNumber()
        {
            PMNCProgram nc = _powerMILL.ActiveProject.NCPrograms.CreateNCProgram("1");
            nc.ProgramNumber = 5;
            Assert.AreEqual(5, nc.ProgramNumber, "Expected property 'ProgramNumber' to change to 5");
        }

        [Test]
        public void Property_PartName()
        {
            PMNCProgram nc = _powerMILL.ActiveProject.NCPrograms.CreateNCProgram("1");
            nc.PartName = "321";
            Assert.AreEqual("321", nc.PartName, "Expected property 'PartName' to change to '321'");
        }

        [Test]
        public void Property_ToolValue()
        {
            PMNCProgram nc = _powerMILL.ActiveProject.NCPrograms.CreateNCProgram("1");
            nc.ToolValue = ToolValues.Tip;
            Assert.AreEqual(ToolValues.Tip, nc.ToolValue, "Expected property 'ToolValue' to change to be 'Tip'");
            nc.ToolValue = ToolValues.Centre;
            Assert.AreEqual(ToolValues.Centre, nc.ToolValue, "Expected property 'ToolValue' to change to be 'Centre'");
        }

        [Test]
        public void Property_Toolpaths()
        {
            _powerMILL.LoadProject(Files.TestFiles.PMwithNcProgram);

            // #1
            PMNCProgram nc = _powerMILL.ActiveProject.NCPrograms["cutting_1"];
            var toolpaths = nc.Toolpaths;
            Assert.AreEqual(1, toolpaths.Count, "Expected NC program 'cutting_1' to have 1 toolpath.");

            // #2
            nc = _powerMILL.ActiveProject.NCPrograms["Milling_2"];
            toolpaths = nc.Toolpaths;
            Assert.AreEqual(7, toolpaths.Count, "Expected NC program 'Milling_2' to have 7 toolpaths.");
        }

        [Test]
        public void Property_ToolChange()
        {
            PMNCProgram nc = _powerMILL.ActiveProject.NCPrograms.CreateNCProgram("1");
            nc.ToolChange = ToolChanges.New;
            Assert.AreEqual(ToolChanges.New, nc.ToolChange, "Expected property 'ToolChange' to change to 'New'");
            nc.ToolChange = ToolChanges.Change;
            Assert.AreEqual(ToolChanges.Change, nc.ToolChange, "Expected property 'ToolChange' to change to 'Change'");
            nc.ToolChange = ToolChanges.Always;
            Assert.AreEqual(ToolChanges.Always, nc.ToolChange, "Expected property 'ToolChange' to change to 'Always'");
        }

        [Test]
        public void Property_ToolNumbering()
        {
            PMNCProgram nc = _powerMILL.ActiveProject.NCPrograms.CreateNCProgram("1");
            nc.ToolNumbering = ToolNumberings.Sequential;
            Assert.AreEqual(ToolNumberings.Sequential,
                            nc.ToolNumbering,
                            "Expected property 'ToolNumbering' to change to 'Sequential'");
            nc.ToolNumbering = ToolNumberings.On;
            Assert.AreEqual(ToolNumberings.On, nc.ToolNumbering, "Expected property 'ToolNumbering' to change to 'On'");
            nc.ToolNumbering = ToolNumberings.Automatic;
            Assert.AreEqual(ToolNumberings.Automatic,
                            nc.ToolNumbering,
                            "Expected property 'ToolNumbering' to change to 'Automatic'");
        }

        [Test]
        public void Property_ToolChangePosition()
        {
            PMNCProgram nc = _powerMILL.ActiveProject.NCPrograms.CreateNCProgram("1");
            nc.ToolChangePosition = ToolChangePositions.After;
            Assert.AreEqual(ToolChangePositions.After,
                            nc.ToolChangePosition,
                            "Expected property 'ToolChangePosition' to change to 'After'");
            nc.ToolChangePosition = ToolChangePositions.Before;
            Assert.AreEqual(ToolChangePositions.Before,
                            nc.ToolChangePosition,
                            "Expected property 'ToolChangePosition' to change to 'Before'");
        }

        #endregion

        #region Test operations

        [Test]
        public void AddToolpath()
        {
            _powerMILL.LoadProject(Files.TestFiles.PMwithNcProgram);
            PMNCProgram nc = _powerMILL.ActiveProject.NCPrograms.Where(x => x.Name == "cutting_1").First();
            nc.AddToolpath("Rough-U_1", true);
            nc.AddToolpath("Rest-Rough-U_1", false);
            var toolpaths = nc.Toolpaths;
            Assert.AreEqual(3, toolpaths.Count, "Expected NC program 'cutting_1' to have 3 toolpaths.");
            Assert.AreEqual("Rough-U_1",
                            toolpaths[0].Name,
                            "Expected NC program 'cutting_1' to have as its first toolpath 'Rough-U_1'.");
            Assert.AreEqual("Rest-Rough-U_1",
                            toolpaths[2].Name,
                            "Expected NC program 'cutting_1' to have as its third toolpath 'Rest-Rough-U_1'.");
        }

        [Test]
        public void RemoveToolpath()
        {
            _powerMILL.LoadProject(Files.TestFiles.PMwithNcProgram);
            PMNCProgram nc = _powerMILL.ActiveProject.NCPrograms.Where(x => x.Name == "cutting_1").First();
            nc.RemoveToolpath("PIN_MACHINE_1");
            Assert.AreEqual(0, nc.Toolpaths.Count, "Expected NC program 'cutting_1' to have its only toolpath removed.");
        }

        //[Test]
        //public void Write()
        //{
        //    _powerMILL.LoadProject(Files.TestFiles.PMwithNcProgram);
        //    PMNCProgram nc = _powerMILL.ActiveProject.NCPrograms.Where(x => x.Name == "cutting_1").First();
        //    Autodesk.FileSystem.File tempFile = Autodesk.FileSystem.File.CreateTemporaryFile("drp");
        //    nc.MachineOptionFileName = Files.TestFiles.OPTION_FILE_Roeders_RXD5_RCS_DPP48.FilePath;
        //    nc.OutputFileName = tempFile.FilePath;
        //    nc.Write();
        //    Assert.IsTrue(tempFile.Exists, String.Format("Expected the NC program 'cutting_1' to be written to {0}.", tempFile.FilePath));
        //}

        [Test]
        public void DrawAndUndrawAllNCPrograms()
        {
            _powerMILL.ActiveProject.NCPrograms.CreateNCProgram("Test NCProgram 1");
            _powerMILL.ActiveProject.NCPrograms.CreateNCProgram("Test NCProgram 2");
            _powerMILL.ActiveProject.NCPrograms.DrawAll();
            _powerMILL.ActiveProject.NCPrograms.UndrawAll();
            Assert.IsTrue(true);
        }

        #endregion
    }
}