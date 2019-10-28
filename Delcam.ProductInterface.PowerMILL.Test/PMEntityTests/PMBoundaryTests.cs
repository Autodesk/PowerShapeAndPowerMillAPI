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
    public class PMBoundaryTests
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
        public void BlockTest()
        {
            _powerMILL.LoadProject(TestFiles.BoundaryTypes);
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries[0];
            Assert.IsNotNull(boundary as PMBoundaryBlock);
        }

        [Test]
        public void RestTest()
        {
            _powerMILL.LoadProject(TestFiles.BoundaryTypes);
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries[1];
            Assert.IsNotNull(boundary as PMBoundaryRest);
        }

        [Test]
        public void SelectedSurfaceTest()
        {
            _powerMILL.LoadProject(TestFiles.BoundaryTypes);
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries[2];
            Assert.IsNotNull(boundary as PMBoundarySelectedSurface);
        }

        [Test]
        public void ShallowTest()
        {
            _powerMILL.LoadProject(TestFiles.BoundaryTypes);
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries[3];
            Assert.IsNotNull(boundary as PMBoundaryShallow);
        }

        [Test]
        public void SilhouetteTest()
        {
            _powerMILL.LoadProject(TestFiles.BoundaryTypes);
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries[4];
            Assert.IsNotNull(boundary as PMBoundarySilhouette);
        }

        [Test]
        public void CollisionTest()
        {
            _powerMILL.LoadProject(TestFiles.BoundaryTypes);
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries[5];
            Assert.IsNotNull(boundary as PMBoundaryCollisionSafe);
        }

        [Test]
        public void StockModelRestTest()
        {
            _powerMILL.LoadProject(TestFiles.BoundaryTypes);
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries[6];
            Assert.IsNotNull(boundary as PMBoundaryStockModelRest);
        }

        [Test]
        public void ContactPointTest()
        {
            _powerMILL.LoadProject(TestFiles.BoundaryTypes);
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries[7];
            Assert.IsNotNull(boundary as PMBoundaryContactPoint);
        }

        [Test]
        public void ContactConversionTest()
        {
            _powerMILL.LoadProject(TestFiles.BoundaryTypes);
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries[8];
            Assert.IsNotNull(boundary as PMBoundaryContactConversion);
        }

        [Test]
        public void BooleanOperationTest()
        {
            _powerMILL.LoadProject(TestFiles.BoundaryTypes);
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries[9];
            Assert.IsNotNull(boundary as PMBoundaryBooleanOperation);
        }

        [Test]
        public void UserDefinedTest()
        {
            _powerMILL.LoadProject(TestFiles.BoundaryTypes);
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries[10];
            Assert.IsNotNull(boundary as PMBoundaryUserDefined);
        }

        [Test]
        public void WriteToFileTest()
        {
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries.CreateBoundary(TestFiles.CurvesFiles);
            var testFile = FileSystem.File.CreateTemporaryFile("pic");
            boundary.WriteToFile(testFile);
            bool fileExists = testFile.Exists;
            testFile.Delete();
            Assert.AreEqual(true, fileExists);
        }

        [Test]
        public void ToPolylinesTest()
        {
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries.CreateBoundary(TestFiles.CurvesFiles);
            var polylines = boundary.ToPolylines();
            Assert.AreEqual(2, polylines.Count);
        }

        [Test]
        public void ToSplinesTest()
        {
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries.CreateBoundary(TestFiles.CurvesFiles);
            var splines = boundary.ToSplines();
            Assert.AreEqual(2, splines.Count);
        }

        [Test]
        public void CreateFromSpline()
        {
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries.CreateBoundary(TestFiles.CurvesFiles);
            var spline = boundary.ToSplines().First();

            PMBoundary newBoundary = _powerMILL.ActiveProject.Boundaries.CreateBoundary(spline);
            Assert.NotNull(newBoundary);
        }

        [Test]
        public void CreateFromPolyLine()
        {
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries.CreateBoundary(TestFiles.CurvesFiles);
            var polyline = boundary.ToPolylines().First();

            PMBoundary newBoundary = _powerMILL.ActiveProject.Boundaries.CreateBoundary(polyline);
            Assert.NotNull(newBoundary);
        }

        [Test]
        public void FlatTest()
        {
            PMBoundary boundary = _powerMILL.ActiveProject.Boundaries.CreateBoundary(TestFiles.SihouetteFile);

            Assert.IsTrue(boundary.BoundingBox.ZSize != 0, "Border cannot be flat.");

            boundary.Flat();

            Assert.IsTrue(boundary.BoundingBox.ZSize == 0, "Border must be flat.");
        }

        [Test]
        public void InsertToolpathTest()
        {
            _powerMILL.LoadProject(TestFiles.BasicToolpath);

            var boundary = _powerMILL.ActiveProject.Boundaries.CreateEmptyBoundary();
            var toolpath = _powerMILL.ActiveProject.Toolpaths.First();
            boundary.InsertToolpath(toolpath);
            var result = boundary.ToPolylines();

            Assert.AreEqual(15, result.Count);
            Assert.AreEqual(18, result[0].Count);
        }

        [Test]
        public void DrawAndUndrawAllBoundariesTest()
        {
            _powerMILL.LoadProject(TestFiles.BoundaryTypes);
            _powerMILL.ActiveProject.Boundaries.DrawAll();
            _powerMILL.ActiveProject.Boundaries.UndrawAll();
            Assert.IsTrue(true);
        }

        [Test]
        public void InsertBoundaryTest()
        {
            PMBoundary bToInsert = _powerMILL.ActiveProject.Boundaries.CreateBoundary(TestFiles.CurvesFiles);
            PMBoundary bEmpty = _powerMILL.ActiveProject.Boundaries.CreateEmptyBoundary();
            bEmpty.InsertBoundary(bToInsert);
            Assert.IsTrue(bEmpty.BoundingBox != null);
        }

        [Test]
        public void InsertBoundaryByNameTest()
        {
            PMBoundary bToInsert = _powerMILL.ActiveProject.Boundaries.CreateBoundary(TestFiles.CurvesFiles);
            PMBoundary bEmpty = _powerMILL.ActiveProject.Boundaries.CreateEmptyBoundary();
            bEmpty.InsertBoundaryByName(bToInsert.Name);
            Assert.IsTrue(bEmpty.BoundingBox != null);
        }

        #endregion
    }
}