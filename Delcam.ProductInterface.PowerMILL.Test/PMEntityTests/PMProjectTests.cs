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

namespace Autodesk.ProductInterface.PowerMILLTest
{
    [TestFixture]
    public class PMProjectTests
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
        public void ImportPTFTest()
        {
            Assert.AreEqual(0, _powerMILL.ActiveProject.Toolpaths.Count);
            _powerMILL.ActiveProject.ImportTemplateFile(TestFiles.ImportTemplateTestFile);
            Assert.AreEqual(1, _powerMILL.ActiveProject.Toolpaths.Count);
        }

        #region Refresh

        [Test]
        public void RefreshAfterAddingNewEntity()
        {
            var activeProject = _powerMILL.Reset();

            _powerMILL.Execute("CREATE BOUNDARY ;");

            Assert.IsFalse(activeProject.Boundaries.Any());

            activeProject.Refresh();

            Assert.IsTrue(activeProject.Boundaries.Any());
        }

        [Test]
        public void RefreshAfterRemovingEntity()
        {
            var activeProject = _powerMILL.Reset();
            _powerMILL.Execute("CREATE BOUNDARY ;");
            activeProject.Refresh();

            Assert.IsTrue(activeProject.Boundaries.Any());

            _powerMILL.Execute("DELETE Boundary \"1\"");

            activeProject.Refresh();

            Assert.IsFalse(activeProject.Boundaries.Any());
        }

        [Test]
        public void RefreshDoesNotAffectAnythingThatHasNotBeenAddedOrRemoved()
        {
            var activeProject = _powerMILL.Reset();
            _powerMILL.Execute("CREATE BOUNDARY ;");
            _powerMILL.Execute("CREATE BOUNDARY ;");
            activeProject.Refresh();

            Assert.IsTrue(activeProject.Boundaries.Count() == 2);

            _powerMILL.Execute("DELETE Boundary \"1\"");

            activeProject.Refresh();

            Assert.IsFalse(activeProject.Boundaries.Any(x => x.Name == "1"));
            Assert.IsTrue(activeProject.Boundaries.Any(x => x.Name == "2"));
        }

        #endregion

        [Test]
        public void ActiveWorkplaneTest()
        {
            _powerMILL.LoadProject(TestFiles.SimplePmProject1);
            _powerMILL.ActiveProject.ActiveWorkplane = _powerMILL.ActiveProject.Workplanes[0];
            Assert.That(_powerMILL.ActiveProject.Workplanes[0].IsActive, Is.True);
        }

        [Test]
        public void ActiveWorldWorkplaneTest()
        {
            _powerMILL.LoadProject(TestFiles.SimplePmProject1);
            _powerMILL.ActiveProject.ActiveWorkplane = _powerMILL.ActiveProject.Workplanes[0];
            Assert.That(_powerMILL.ActiveProject.Workplanes[0].IsActive, Is.True);
            _powerMILL.ActiveProject.ActiveWorkplane = null;
            Assert.That(_powerMILL.ActiveProject.Workplanes.All(x => x.IsActive == false), Is.True);
        }

        [Test]
        public void CreateBlockFromBoundary()
        {   
            _powerMILL.Execute(string.Format("CREATE BOUNDARY \"{0}\"", "TestBoundary"));            
            _powerMILL.Execute(string.Format("EDIT BOUNDARY \"{0}\" INSERT FILE \"{1}\"", "TestBoundary", TestFiles.CurvesFiles));
            _powerMILL.ActiveProject.Refresh();
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries.ActiveItem;
            _powerMILL.ActiveProject.CreateBlock(boundary, 0, 100);
            Assert.AreEqual("100,0", _powerMILL.ExecuteEx("PRINT $BLOCK.LIMITS.ZMax"));
        }

        [Test]
        public void CreateBlockFromBoundaryZMinMax()
        {
            _powerMILL.LoadProject(TestFiles.SimplePmProject1);
            _powerMILL.Execute(string.Format("CREATE BOUNDARY \"{0}\"", "TestBoundary"));
            _powerMILL.Execute(string.Format("EDIT BOUNDARY \"{0}\" INSERT FILE \"{1}\"", "TestBoundary", TestFiles.CurvesFiles));
            _powerMILL.ActiveProject.Refresh();
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries.ActiveItem;            
            _powerMILL.ActiveProject.CreateBlock(boundary);
            Assert.AreEqual("53,353777", _powerMILL.ExecuteEx("PRINT $BLOCK.LIMITS.ZMax"));
            Assert.AreEqual("-30,004846", _powerMILL.ExecuteEx("PRINT $BLOCK.LIMITS.ZMin"));            
        }

        #endregion
    }
}