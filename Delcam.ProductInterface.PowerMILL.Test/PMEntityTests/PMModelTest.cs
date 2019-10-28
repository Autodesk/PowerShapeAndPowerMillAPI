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
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerMILL;
using Autodesk.ProductInterface.PowerMILLTest.Files;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.PMEntityTests
{
    [TestFixture]
    public class PMModelTest
    {
        private PMModel _defaultModel;
        private PMAutomation _powerMill;
        private readonly List<File> _tempFilesToDelete = new List<File>();
        private readonly List<Directory> _tempFoldersToDelete = new List<Directory>();

        #region Test Setup operations

        [SetUp]
        public void TestFixtureSetup()
        {
            _powerMill = new PMAutomation(InstanceReuse.UseExistingInstance);
            _powerMill.DialogsOff();
            _powerMill.CloseProject();

            var tempFolder = Directory.CreateTemporaryDirectory();
            TestFiles.SimplePmProject1.Copy(tempFolder);
            _powerMill.LoadProject(tempFolder);
            _tempFoldersToDelete.Add(tempFolder);

            _defaultModel = _powerMill.ActiveProject.Models[0];
        }

        [TearDown]
        public void TestFixtureTearDown()
        {
            try
            {
                _powerMill.CloseProject();

                foreach (var file in _tempFilesToDelete)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    {
                    }
                }
                _tempFilesToDelete.Clear();

                foreach (var folder in _tempFoldersToDelete)
                {
                    try
                    {
                        folder.Delete();
                    }
                    catch
                    {
                    }
                }
                _tempFoldersToDelete.Clear();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Tests

        [Test]
        public void WriteToFile()
        {
            // #1
            var dmtFile = File.CreateTemporaryFile("dmt");
            _tempFilesToDelete.Add(dmtFile);
            Assert.False(dmtFile.Exists, "File should not exist");

            // #2
            _defaultModel.ExportModel(dmtFile);
            Assert.True(dmtFile.Exists, "File should exist");

            // #3
            var stlFile = File.CreateTemporaryFile("stl");
            _tempFilesToDelete.Add(stlFile);
            Assert.False(stlFile.Exists, "File should not exist");

            // #4
            _defaultModel.ExportModel(stlFile);
            Assert.True(stlFile.Exists, "File should exist");
        }

        [Test]
        public void ToDMTModelTest()
        {
            // #1
            var dmtFile = File.CreateTemporaryFile("dmt");
            _tempFilesToDelete.Add(dmtFile);
            Assert.False(dmtFile.Exists, "File should not exist");

            // #2
            _defaultModel.ExportModel(dmtFile, false);
            Assert.True(dmtFile.Exists, "File should exist");

            // #3
            var model1 = DMTModelReader.ReadFile(dmtFile);
            var pcModel1 = model1.ToPointCloud();
            var model2 = _defaultModel.ToDMTModel();
            var pcModel2 = model2.ToPointCloud();

            //check all the points are the same
            Assert.AreEqual(pcModel1.Count, pcModel2.Count);

            // #4
            for (var n = 0; n < pcModel1.Count; n++)
            {
                Assert.AreEqual(pcModel1[n], pcModel2[n], "Dmt models not equal");
            }
        }

        [Test]
        public void MirrorInPlaneTest()
        {
            // #1
            // bravo workplane should be active at world 0,0,0
            var bxBeforeYZ = _defaultModel.BoundingBox;
            var beforeXMax = bxBeforeYZ.MaxX;
            var beforeZMax = bxBeforeYZ.MaxZ;
            _defaultModel.MirrorInPlane(Planes.YZ);
            var bxAfterYZ = _defaultModel.BoundingBox;
            Assert.AreEqual(bxAfterYZ.MinX, -beforeXMax);
            Assert.AreEqual(beforeZMax, bxAfterYZ.MaxZ);

            // #2
            // now min Z should now be max Z and vice versa but opposite signs.
            _defaultModel.MirrorInPlane(Planes.XY);
            var bxAfterXY = _defaultModel.BoundingBox;
            Assert.AreEqual(bxAfterXY.MaxZ, -bxAfterYZ.MinZ, "XY Mirror failed");
            Assert.AreEqual(bxAfterXY.MinZ, -bxAfterYZ.MaxZ, "XY Mirror failed MinZ");

            // #3
            var bxModel = _powerMill.ActiveProject.Models.CreateModel(TestFiles.SingleMesh);
            var bxBoxBefore = bxModel.BoundingBox;
            bxModel.MirrorInPlane(Planes.ZX);
            var bxBoxAfter = bxModel.BoundingBox;
            Assert.AreEqual(-bxBoxBefore.MinY, bxBoxAfter.MaxY, " ZX mirror failed");
            Assert.AreEqual(-bxBoxBefore.MaxY, bxBoxAfter.MinY, " ZX mirror failed");
        }

        [Test]
        public void MoveTest()
        {
            // #1
            var bxBefore = _defaultModel.BoundingBox;
            var moveBy = new Vector(100.0, 200.0, 300.0);
            _defaultModel.Move(moveBy);
            var bxAfter = _defaultModel.BoundingBox;
            Assert.AreEqual(bxAfter.MaximumBounds - bxBefore.MaximumBounds, moveBy);
        }

        [Test]
        public void RotateTest()
        {
            _powerMill.Execute("DEACTIVATE WORKPLANE");
            // #1
            // Rotate by 180° in X-Axis
            var bxBefore = _defaultModel.BoundingBox;
            _defaultModel.Rotate(Axes.X, 180);
            var bxAfter = _defaultModel.BoundingBox;
            Assert.AreEqual(bxBefore.MinY * -1, bxAfter.MaxY);
            Assert.AreEqual(bxBefore.MaxY * -1, bxAfter.MinY);
            Assert.AreEqual(bxBefore.MinZ * -1, bxAfter.MaxZ);
            Assert.AreEqual(bxBefore.MaxZ * -1, bxAfter.MinZ);

            // #2
            // Rotate by 180° in Y-Axis
            bxBefore = _defaultModel.BoundingBox;
            _defaultModel.Rotate(Axes.Y, 180);
            bxAfter = _defaultModel.BoundingBox;
            Assert.AreEqual(bxBefore.MinX * -1, bxAfter.MaxX);
            Assert.AreEqual(bxBefore.MaxX * -1, bxAfter.MinX);
            Assert.AreEqual(bxBefore.MinZ * -1, bxAfter.MaxZ);
            Assert.AreEqual(bxBefore.MaxZ * -1, bxAfter.MinZ);

            // #3
            // Rotate by 180° in Z-Axis
            bxBefore = _defaultModel.BoundingBox;
            _defaultModel.Rotate(Axes.Z, 180);
            bxAfter = _defaultModel.BoundingBox;
            Assert.AreEqual(bxBefore.MinX * -1, bxAfter.MaxX);
            Assert.AreEqual(bxBefore.MaxX * -1, bxAfter.MinX);
            Assert.AreEqual(bxBefore.MinY * -1, bxAfter.MaxY);
            Assert.AreEqual(bxBefore.MaxY * -1, bxAfter.MinY);
        }

        [Test]
        public void PathTest()
        {
            // #1
            var tmpDmt = File.CreateTemporaryFile("dmt");
            TestFiles.SingleMesh.Copy(tmpDmt);
            _tempFilesToDelete.Add(tmpDmt);
            var model = _powerMill.ActiveProject.Models.CreateModel(tmpDmt);
            Assert.AreEqual(tmpDmt.Path, model.Path);
        }

        [Test]
        public void Delete()
        {
            // #1
            var tmpDmt = File.CreateTemporaryFile("dmt");
            TestFiles.SingleMesh.Copy(tmpDmt);
            _tempFilesToDelete.Add(tmpDmt);
            var model = _powerMill.ActiveProject.Models.CreateModel(tmpDmt);
            var modelName = model.Name;
            var nModelsBeforeDelete = _powerMill.ActiveProject.Models.Count;
            model.Delete();
            var nModelsAfterDelete = _powerMill.ActiveProject.Models.Count;
            Assert.AreEqual(nModelsBeforeDelete - 1, nModelsAfterDelete);
            Assert.AreEqual(0, _powerMill.ActiveProject.Models.Where(x => x.Name == modelName).Count());
        }

        [Test]
        public void Reimporting_ShouldReimportModel()
        {
            _defaultModel.Reimport(TestFiles.TestModelSurfaceFile1);
            Assert.AreEqual(_defaultModel.Path,
                            TestFiles.TestModelSurfaceFile1.Path,
                            "Model should have been reimported.");
        }

        [Test]
        public void AddToSelectionTest()
        {
            _defaultModel.AddToSelection(true);
            _powerMill.ActiveProject.Models.DeleteSelectedSurfaces();
            _powerMill.ActiveProject.Models.DeleteEmptyModels();
            Assert.IsFalse(_defaultModel.Exists);
        }

        [Test]
        public void RemoveFromSelectionTest()
        {
            _defaultModel.AddToSelection(true);
            _defaultModel.RemoveFromSelection();
            _powerMill.ActiveProject.Models.DeleteSelectedSurfaces();
            _powerMill.ActiveProject.Models.DeleteEmptyModels();
            Assert.IsTrue(_defaultModel.Exists);
        }

        [Test]
        public void InvertSelectionTest()
        {
            _defaultModel.AddToSelection(true);
            _defaultModel.InvertSelection();
            _powerMill.ActiveProject.Models.DeleteSelectedSurfaces();
            _powerMill.ActiveProject.Models.DeleteEmptyModels();
            Assert.IsTrue(_defaultModel.Exists);
        }

        [Test]
        public void DrawAndUndrawAllModels()
        {
            _powerMill.ActiveProject.Models.DrawAll();            
            _powerMill.ActiveProject.Models.UndrawAll();
            Assert.IsTrue(true);
        }
        #endregion
    }
}