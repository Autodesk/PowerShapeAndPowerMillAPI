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
using Autodesk.ProductInterface.PowerMILLTest.Files;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.PMEntityTests
{
    [TestFixture]
    public class PMPatternTests
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

        #region Test operations

        [Test]
        public void WriteToFileTest()
        {
            PMPattern pattern = _powerMILL.ActiveProject.Patterns.CreatePattern(TestFiles.CurvesFiles);
            var testFile = FileSystem.File.CreateTemporaryFile("pic");
            pattern.WriteToFile(testFile);
            bool fileExists = testFile.Exists;
            testFile.Delete();
            Assert.AreEqual(true, fileExists);
        }

        [Test]
        public void ToPolylinesTest()
        {
            PMPattern pattern = _powerMILL.ActiveProject.Patterns.CreatePattern(TestFiles.CurvesFiles);
            var polylines = pattern.ToPolylines();
            Assert.AreEqual(2, polylines.Count);
        }

        [Test]
        public void ToSplinesTest()
        {
            PMPattern pattern = _powerMILL.ActiveProject.Patterns.CreatePattern(TestFiles.CurvesFiles);
            var splines = pattern.ToSplines();
            Assert.AreEqual(2, splines.Count);
        }

        [Test]
        public void InsertToolpathTest()
        {
            _powerMILL.LoadProject(TestFiles.BasicToolpath);

            var pattern = _powerMILL.ActiveProject.Patterns.CreateEmptyPattern();
            var toolpath = _powerMILL.ActiveProject.Toolpaths.First();
            pattern.InsertToolpath(toolpath);
            var result = pattern.ToPolylines();

            Assert.AreEqual(15, result.Count);
            Assert.AreEqual(18, result[0].Count);
        }

        [Test]
        public void CreatePatternFromSpline()
        {
            PMPattern comparisonPattern = _powerMILL.ActiveProject.Patterns.CreatePattern(TestFiles.CurvesFiles);
            var spline = comparisonPattern.ToSplines().First();

            PMPattern outputPattern = _powerMILL.ActiveProject.Patterns.CreatePattern(spline);
            var outSplines = outputPattern.ToSplines();

            Assert.AreEqual(1, outSplines.Count);
            Assert.AreEqual(230, outSplines.First().Count);
        }

        [Test]
        public void DrawAndUndrawAllPatternsTest()
        {
            _powerMILL.ActiveProject.Patterns.CreatePattern(TestFiles.CurvesFiles);
            _powerMILL.ActiveProject.Patterns.CreatePattern(TestFiles.CurvesFiles);
            _powerMILL.ActiveProject.Patterns.DrawAll();
            _powerMILL.ActiveProject.Patterns.UndrawAll();
            Assert.IsTrue(true);
        }

        #endregion
    }
}