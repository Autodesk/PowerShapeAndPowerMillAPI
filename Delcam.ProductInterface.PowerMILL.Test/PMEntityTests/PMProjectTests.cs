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
        public void CreateBlockFromBoundaryWithLimitsTest()
        {               
            _powerMILL.ActiveProject.Boundaries.CreateBoundary(TestFiles.CurvesFiles);
            _powerMILL.ActiveProject.Refresh();
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries.ActiveItem;
            var boundingBox = _powerMILL.ActiveProject.CreateBlockFromBoundaryWithLimits(boundary, 0, 100);            
            Assert.That(boundingBox.MaxZ.Value, Is.EqualTo(100));
        }

        [Test]
        public void CreateBlockFromBoundaryTest()
        {            
            _powerMILL.LoadProject(TestFiles.SimplePmProject1);            
            _powerMILL.ActiveProject.Boundaries.CreateBoundary(TestFiles.CurvesFiles);
            _powerMILL.ActiveProject.Refresh();
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries.ActiveItem;
            var boundingBox = _powerMILL.ActiveProject.CreateBlockFromBoundary(boundary);            
            Assert.That(boundingBox.MaxZ, Is.EqualTo((Autodesk.Geometry.MM)53.353777));
            Assert.That(boundingBox.MinZ, Is.EqualTo((Autodesk.Geometry.MM)(-30.004846)));
        }

        [Test]
        public void ExportBlockTest()
        {            
            _powerMILL.ActiveProject.Boundaries.CreateBoundary(TestFiles.CurvesFiles);
            _powerMILL.ActiveProject.Refresh();
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries.ActiveItem;
            var boundingBox = _powerMILL.ActiveProject.CreateBlockFromBoundaryWithLimits(boundary, 0, 100);            
            FileSystem.File file = FileSystem.File.CreateTemporaryFile("dmt", false);
            _powerMILL.ActiveProject.ExportBlock(file);
            Assert.That(file.Exists);
            file.Delete();            
            file = FileSystem.File.CreateTemporaryFile("stl", false);
            _powerMILL.ActiveProject.ExportBlock(file);
            Assert.That(file.Exists);
            file.Delete();
        }


        #endregion
    }
}