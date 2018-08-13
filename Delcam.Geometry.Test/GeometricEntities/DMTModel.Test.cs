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
using System.Runtime.CompilerServices;
using Autodesk.FileSystem;
using NUnit.Framework;
using System.Xml;
using System.Xml.Serialization;

namespace Autodesk.Geometry.Test.GeometricEntities
{
    [TestFixture]
    public class DMTModelTest
    {
        [Test]
        public void DMTPointProjectionTest()
        {
            // Ensures result of projecting a known point is as expected.
            DMTModel dmtModel = DMTModelReader.ReadFile(new File(TestFiles.FetchTestFile("Point Projection.dmt")));

            // testPoint correlates with PowerSHAPE model.
            var testPoint = new Point(0, 0, 998.465026855469);

            // NOTE: ProjectPoints(...) is implicitly tested by a call to ProjectPoint(...).
            Point projectedPoint = dmtModel.ProjectPoint(new Point(0, 0, 2000));

            Assert.AreEqual(testPoint, projectedPoint);
            Assert.Pass();
        }

        [Test]
        public void WhenCoincidentPointsInATriangle_ThenStillAppendBinarySTLFile()
        {
            var mesh = new File(TestFiles.FetchTestFile("CoincidentPointsInATriangle.stl"));
            var importedModel = DMTModelReader.ReadFile(mesh);

            Assert.That(importedModel.TotalNoOfTriangles, Is.EqualTo(3), "Failed expected number of triangles.");
            Assert.That(importedModel.TotalNoOfVertices, Is.EqualTo(3), "Failed expected number of vertices.");

            var expectedOutput =
                DMTModelReader.ReadFile(new File(TestFiles.FetchTestFile("CoincidentPointsInATriangle_Expected.stl")));

            Assert.IsTrue(importedModel.BoundingBox.XSize == expectedOutput.BoundingBox.XSize,
                          "It is not giving the expected bounding box xsize.");
            Assert.IsTrue(importedModel.BoundingBox.YSize == expectedOutput.BoundingBox.YSize,
                          "It is not giving the expected bounding box xsize.");
            Assert.IsTrue(importedModel.BoundingBox.ZSize == expectedOutput.BoundingBox.ZSize,
                          "It is not giving the expected bounding box xsize.");
        }

        [Test]
        public void ProjectPointsTest()
        {
            DMTModel modelToZone = DMTModelReader.ReadFile(new File(TestFiles.SmallModel));
            var nearestPoint = modelToZone.ProjectPoint(new Point(-28.186, -4.1135, 1000));
            Assert.AreNotEqual(nearestPoint, null);
        }

        [Test]
        public void WhenProjectPointsAlongProjectionVectors_ThenCheckExpectedProjections()
        {
            DMTModel model = DMTModelReader.ReadFile(new File(@"E:\PowerShapeAndPowerMillAPI\Delcam.Geometry.Test\TestFiles\Plane_1m_50mmX.dmt"));
            var pointsToProject = new List<Point>();
            pointsToProject.Add(new Point(0, -500, -500));
            pointsToProject.Add(new Point(0, -500, 0));
            pointsToProject.Add(new Point(0, -500, 500));
            pointsToProject.Add(new Point(0, 0, -500));
            pointsToProject.Add(new Point(0, 0, 0));
            pointsToProject.Add(new Point(0, 0, 500));
            pointsToProject.Add(new Point(0, 500, -500));
            pointsToProject.Add(new Point(0, 500, 0));
            pointsToProject.Add(new Point(0, 500, 500));
            

            var projectionVectors = new List<Vector>();
            projectionVectors.Add(new Vector(1, 0,0));
            projectionVectors.Add(new Vector(1, 0, 0));
            projectionVectors.Add(new Vector(1, 0, 0));
            projectionVectors.Add(new Vector(1, 0, 0));
            projectionVectors.Add(new Vector(1, 0, 0));
            projectionVectors.Add(new Vector(1, 0, 0));
            projectionVectors.Add(new Vector(1, 0, 0));
            projectionVectors.Add(new Vector(1, 0, 0));
            projectionVectors.Add(new Vector(1, 0, 0));

            var projectedPoints = model.ProjectPoints(pointsToProject, projectionVectors);
            Assert.AreEqual(new Point(50, -500, -500), projectedPoints[0]);
            Assert.AreEqual(new Point(50, -500, 0), projectedPoints[1]);
            Assert.AreEqual(new Point(50, -500, 500), projectedPoints[2]);
            Assert.AreEqual(new Point(50, 0, -500), projectedPoints[3]);
            Assert.AreEqual(new Point(50, 0, 0), projectedPoints[4]);
            Assert.AreEqual(new Point(50, 0, 500), projectedPoints[5]);
            Assert.AreEqual(new Point(50, 500, -500), projectedPoints[6]);
            Assert.AreEqual(new Point(50, 500, 0), projectedPoints[7]);
            Assert.AreEqual(new Point(50, 500, 500), projectedPoints[8]);
        }

        [Test]
        public void WhenProjectPointsAlongProjectionVectors_GivenMeasuredBelowNominalPoints_ThenCheckExpectedProjections()
        {
            DMTModel model = DMTModelReader.ReadFile(new File(@"E:\PowerShapeAndPowerMillAPI\Delcam.Geometry.Test\TestFiles\Plane_1m_0mmZ.dmt"));

            // NOTE: These points and vectors correspond to CurvedSurface when extracted 4 points for each lateral and longoitudinal
            var pointsToProject = new List<Point>();
            pointsToProject.Add(new Point(-456.318173, 415.756336, 10));
            pointsToProject.Add(new Point(-56.69534, 408.421105, 69.512778));
            pointsToProject.Add(new Point(300.14362, 396.455541, 84.970686));
            pointsToProject.Add(new Point(477.832501, 344.086974, 10));
            pointsToProject.Add(new Point(-456.318173, 349.08967, 10));
            pointsToProject.Add(new Point(-56.69534, 341.754438, 69.512778));
            pointsToProject.Add(new Point(300.14362, 329.788874, 84.970686));
            pointsToProject.Add(new Point(477.832501, 277.420307, 10));
            pointsToProject.Add(new Point(-456.318173, 282.423003, 10));
            pointsToProject.Add(new Point(-56.69534, 275.087772, 69.512778));
            pointsToProject.Add(new Point(300.14362, 263.122207, 84.970686));
            pointsToProject.Add(new Point(477.832501, 210.75364, 10));
            pointsToProject.Add(new Point(-456.318173, 215.756336, 10));
            pointsToProject.Add(new Point(-56.69534, 208.421105, 69.512778));
            pointsToProject.Add(new Point(300.14362, 196.455541, 84.970686));
            pointsToProject.Add(new Point(477.832501, 144.086974, 10));


            var projectionVectors = new List<Vector>();
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0.337817, 0, 0.941212));
            projectionVectors.Add(new Vector(-0.493732, 0, 0.869614));
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0, 0, 0.941212));
            projectionVectors.Add(new Vector(0.337817, 0, 0.941212));
            projectionVectors.Add(new Vector(-0.493732,  0, 0.869614));
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0.337817, -0, 0.941212));
            projectionVectors.Add(new Vector(-0.493732, -0, 0.869614));
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0.337817, 0, 0.941212));
            projectionVectors.Add(new Vector(-0.493732, -0, 0.869614));
            projectionVectors.Add(new Vector(0, 0, 1));



            var projectedPoints = model.ProjectPoints(pointsToProject, projectionVectors);

            Assert.AreEqual(new Point(-456.318173, 415.756336, 0), projectedPoints[0]);
            Assert.AreEqual(new Point(-81.6446586716978, 408.421105, 0), projectedPoints[1]);
            Assert.AreEqual(new Point(348.38657232155, 396.455541, 0), projectedPoints[2]);
            Assert.AreEqual(new Point(477.832501, 344.086974, 0), projectedPoints[3]);
            Assert.AreEqual(new Point(-456.318173, 349.08967, 0), projectedPoints[4]);
            Assert.AreEqual(new Point(-81.6446586716978, 341.754438, 0), projectedPoints[5]);
            Assert.AreEqual(new Point(348.38657232155, 329.788874, 0), projectedPoints[6]);
            Assert.AreEqual(new Point(477.832501, 277.420307, 0), projectedPoints[7]);
            Assert.AreEqual(new Point(-456.318173, 282.423003, 0), projectedPoints[8]);
            Assert.AreEqual(new Point(-81.6446586716978, 275.087772, 0), projectedPoints[9]);
            Assert.AreEqual(new Point(348.38657232155, 263.122207, 0), projectedPoints[10]);
            Assert.AreEqual(new Point(477.832501, 210.75364, 0), projectedPoints[11]);
            Assert.AreEqual(new Point(-456.318173, 215.756336, 0), projectedPoints[12]);
            Assert.AreEqual(new Point(-81.6446586716978, 208.421105, 0), projectedPoints[13]);
            Assert.AreEqual(new Point(348.38657232155, 196.455541, 0), projectedPoints[14]);
            Assert.AreEqual(new Point(477.832501, 144.086974, 0), projectedPoints[15]);


            var parallelProjectedPoints = model.ProjectPointsParallel(pointsToProject, projectionVectors);
            //Ensure that parallel processing gives the same result
            for (int i = 0; i < pointsToProject.Count; i++)
            {
                Assert.AreEqual(parallelProjectedPoints[i].X, projectedPoints[i].X);
                Assert.AreEqual(parallelProjectedPoints[i].Y, projectedPoints[i].Y);
                Assert.AreEqual(parallelProjectedPoints[i].Z, projectedPoints[i].Z);
            }
        }

        [Test]
        public void WhenProjectPointsAlongProjectionVectors_GivenMeasuredAboveNominalPoints_ThenCheckExpectedProjections()
        {
            DMTModel model = DMTModelReader.ReadFile(new File(@"E:\PowerShapeAndPowerMillAPI\Delcam.Geometry.Test\TestFiles\Plane_1m_150mmZ.dmt"));

            // NOTE: These points and vectors correspond to CurvedSurface when extracted 4 points for each lateral and longoitudinal
            var pointsToProject = new List<Point>();
            pointsToProject.Add(new Point(-456.318173, 415.756336, 10));
            pointsToProject.Add(new Point(-56.69534, 408.421105, 69.512778));
            pointsToProject.Add(new Point(300.14362, 396.455541, 84.970686));
            pointsToProject.Add(new Point(477.832501, 344.086974, 10));
            pointsToProject.Add(new Point(-456.318173, 349.08967, 10));
            pointsToProject.Add(new Point(-56.69534, 341.754438, 69.512778));
            pointsToProject.Add(new Point(300.14362, 329.788874, 84.970686));
            pointsToProject.Add(new Point(477.832501, 277.420307, 10));
            pointsToProject.Add(new Point(-456.318173, 282.423003, 10));
            pointsToProject.Add(new Point(-56.69534, 275.087772, 69.512778));
            pointsToProject.Add(new Point(300.14362, 263.122207, 84.970686));
            pointsToProject.Add(new Point(477.832501, 210.75364, 10));
            pointsToProject.Add(new Point(-456.318173, 215.756336, 10));
            pointsToProject.Add(new Point(-56.69534, 208.421105, 69.512778));
            pointsToProject.Add(new Point(300.14362, 196.455541, 84.970686));
            pointsToProject.Add(new Point(477.832501, 144.086974, 10));


            var projectionVectors = new List<Vector>();
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0.337817, 0, 0.941212));
            projectionVectors.Add(new Vector(-0.493732, 0, 0.869614));
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0, 0, 0.941212));
            projectionVectors.Add(new Vector(0.337817, 0, 0.941212));
            projectionVectors.Add(new Vector(-0.493732, 0, 0.869614));
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0.337817, -0, 0.941212));
            projectionVectors.Add(new Vector(-0.493732, -0, 0.869614));
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0.337817, 0, 0.941212));
            projectionVectors.Add(new Vector(-0.493732, -0, 0.869614));
            projectionVectors.Add(new Vector(0, 0, 1));



            var projectedPoints = model.ProjectPoints(pointsToProject, projectionVectors);

            Assert.AreEqual(new Point(-456.318173, 415.756336, 150), projectedPoints[0]);
            Assert.AreEqual(new Point(-27.8071066642861, 408.421105, 150), projectedPoints[1]);
            Assert.AreEqual(new Point(263.222580021518, 396.455541, 150), projectedPoints[2]);
            Assert.AreEqual(new Point(477.832501, 344.086974, 150), projectedPoints[3]);
            Assert.AreEqual(new Point(-456.318173, 349.08967, 150), projectedPoints[4]);
            Assert.AreEqual(new Point(-27.8071066642861, 341.754438, 150), projectedPoints[5]);
            Assert.AreEqual(new Point(263.222580021518, 329.788874, 150), projectedPoints[6]);
            Assert.AreEqual(new Point(477.832501, 277.420307, 150), projectedPoints[7]);
            Assert.AreEqual(new Point(-456.318173, 282.423003 ,150), projectedPoints[8]);
            Assert.AreEqual(new Point(-27.8071066642861, 275.087772, 150), projectedPoints[9]);
            Assert.AreEqual(new Point(263.222580021518, 263.122207, 150), projectedPoints[10]);
            Assert.AreEqual(new Point(477.832501, 210.75364, 150), projectedPoints[11]);
            Assert.AreEqual(new Point(-456.318173, 215.756336, 150), projectedPoints[12]);
            Assert.AreEqual(new Point(-27.8071066642861, 208.421105, 150), projectedPoints[13]);
            Assert.AreEqual(new Point(263.222580021518, 196.455541, 150), projectedPoints[14]);
            Assert.AreEqual(new Point(477.832501, 144.086974, 150), projectedPoints[15]);


            var parallelProjectedPoints = model.ProjectPointsParallel(pointsToProject, projectionVectors);
            //Ensure that parallel processing gives the same result
            for (int i = 0; i < pointsToProject.Count; i++)
            {
                Assert.AreEqual(parallelProjectedPoints[i].X, projectedPoints[i].X);
                Assert.AreEqual(parallelProjectedPoints[i].Y, projectedPoints[i].Y);
                Assert.AreEqual(parallelProjectedPoints[i].Z, projectedPoints[i].Z);
            }
        }

        [Test]
        public void WhenProjectPointsAlongProjectionVectors_GivenMeasuredAboveAndBelowNominalPoints_ThenCheckExpectedProjections()
        {
            DMTModel model = DMTModelReader.ReadFile(new File(@"E:\PowerShapeAndPowerMillAPI\Delcam.Geometry.Test\TestFiles\Plane_1m_50mmZ.dmt"));

            // NOTE: These points and vectors correspond to CurvedSurface when extracted 4 points for each lateral and longoitudinal
            var pointsToProject = new List<Point>();
            pointsToProject.Add(new Point(-456.318173, 415.756336, 10));
            pointsToProject.Add(new Point(-56.69534, 408.421105, 69.512778));
            pointsToProject.Add(new Point(300.14362, 396.455541, 84.970686));
            pointsToProject.Add(new Point(477.832501, 344.086974, 10));
            pointsToProject.Add(new Point(-456.318173, 349.08967, 10));
            pointsToProject.Add(new Point(-56.69534, 341.754438, 69.512778));
            pointsToProject.Add(new Point(300.14362, 329.788874, 84.970686));
            pointsToProject.Add(new Point(477.832501, 277.420307, 10));
            pointsToProject.Add(new Point(-456.318173, 282.423003, 10));
            pointsToProject.Add(new Point(-56.69534, 275.087772, 69.512778));
            pointsToProject.Add(new Point(300.14362, 263.122207, 84.970686));
            pointsToProject.Add(new Point(477.832501, 210.75364, 10));
            pointsToProject.Add(new Point(-456.318173, 215.756336, 10));
            pointsToProject.Add(new Point(-56.69534, 208.421105, 69.512778));
            pointsToProject.Add(new Point(300.14362, 196.455541, 84.970686));
            pointsToProject.Add(new Point(477.832501, 144.086974, 10));


            var projectionVectors = new List<Vector>();
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0.337817, 0, 0.941212));
            projectionVectors.Add(new Vector(-0.493732, 0, 0.869614));
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0, 0, 0.941212));
            projectionVectors.Add(new Vector(0.337817, 0, 0.941212));
            projectionVectors.Add(new Vector(-0.493732, 0, 0.869614));
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0.337817, -0, 0.941212));
            projectionVectors.Add(new Vector(-0.493732, -0, 0.869614));
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0, 0, 1));
            projectionVectors.Add(new Vector(0.337817, 0, 0.941212));
            projectionVectors.Add(new Vector(-0.493732, -0, 0.869614));
            projectionVectors.Add(new Vector(0, 0, 1));



            var projectedPoints = model.ProjectPoints(pointsToProject, projectionVectors);

            Assert.AreEqual(new Point(-456.318173, 415.756336, 50), projectedPoints[0]);
            Assert.AreEqual(new Point(-63.6988080025605, 408.421105, 50), projectedPoints[1]);
            Assert.AreEqual(new Point(319.998574888206, 396.455541, 50), projectedPoints[2]);
            Assert.AreEqual(new Point(477.832501, 344.086974, 50), projectedPoints[3]);
            Assert.AreEqual(new Point(-456.318173, 349.08967, 50), projectedPoints[4]);
            Assert.AreEqual(new Point(-63.6988080025605, 341.754438, 50), projectedPoints[5]);
            Assert.AreEqual(new Point(319.998574888206, 329.788874, 50), projectedPoints[6]);
            Assert.AreEqual(new Point(477.832501, 277.420307, 50), projectedPoints[7]);
            Assert.AreEqual(new Point(-456.318173, 282.423003, 50), projectedPoints[8]);
            Assert.AreEqual(new Point(-63.6988080025605, 275.087772, 50), projectedPoints[9]);
            Assert.AreEqual(new Point(319.998574888206, 263.122207, 50), projectedPoints[10]);
            Assert.AreEqual(new Point(477.832501, 210.75364, 50), projectedPoints[11]);
            Assert.AreEqual(new Point(-456.318173, 215.756336, 50), projectedPoints[12]);
            Assert.AreEqual(new Point(-63.6988080025605, 208.421105, 50), projectedPoints[13]);
            Assert.AreEqual(new Point(319.998574888206, 196.455541, 50), projectedPoints[14]);
            Assert.AreEqual(new Point(477.832501, 144.086974, 50), projectedPoints[15]);


            var parallelProjectedPoints = model.ProjectPointsParallel(pointsToProject, projectionVectors);
            //Ensure that parallel processing gives the same result
            for (int i = 0; i < pointsToProject.Count; i++)
            {
                Assert.AreEqual(parallelProjectedPoints[i].X, projectedPoints[i].X);
                Assert.AreEqual(parallelProjectedPoints[i].Y, projectedPoints[i].Y);
                Assert.AreEqual(parallelProjectedPoints[i].Z, projectedPoints[i].Z);
            }
        }


        [Test]
        [Ignore("Just to run manually and ensure that time keeps the same")]
        public void WhenProjectPointsParallelAlongProjectionVectors_ThenCheckHowLongItTakes()
        {
            // Before it would take arounf 2 hours 20 minutes, now with parallel processing it takes around 33 minutes
            DMTModel model = DMTModelReader.ReadFile(new File(@"E:\PowerShapeAndPowerMillAPI\Delcam.Geometry.Test\TestFiles\Plane_1m_50mmX_32768Triangles.dmt"));
            var pointsToProject = new List<Point>();
            var projectionVectors = new List<Vector>();
            //var pointDistance = 1000 / 256 ~ 4
            //  Mesh 2 (16641 nodes, 32768 triangles, 1 regions)
            int y = -500;
            int z = -500;
            int index = 1;
            pointsToProject.Add(new Point(0, y, z));
            projectionVectors.Add(new Vector(1, 0, 0));
            while (z < 500)
            {
                while (y < 500)
                {
                    y +=  4;
                    index++;
                    pointsToProject.Add(new Point(0, y, z));
                    projectionVectors.Add(new Vector(1, 0, 0));
                }
                z += 4;
                y = -500;
            }


            var indicesAndProjectedPoints = model.ProjectPointsParallel(pointsToProject, projectionVectors);


            for (int i=0; i< pointsToProject.Count; i++)
            {
                Assert.AreEqual(pointsToProject[i].X + 50, indicesAndProjectedPoints[i].X);
                Assert.AreEqual(pointsToProject[i].Y, indicesAndProjectedPoints[i].Y);
                Assert.AreEqual(pointsToProject[i].Z, indicesAndProjectedPoints[i].Z);
            }

            Assert.AreEqual(pointsToProject.Count, indicesAndProjectedPoints.Count);
        }

        [Test]
        public void WhenProjectPointsAlongProjectionVectors_ThenCheckItReturnsNearestProjections()
        {
            DMTModel model = DMTModelReader.ReadFile(new File(@"E:\PowerShapeAndPowerMillAPI\Delcam.Geometry.Test\TestFiles\Block_1m_50mmX.dmt"));
            var pointsToProject = new List<Point>();
            pointsToProject.Add(new Point(0, -500, -500));
            pointsToProject.Add(new Point(0, -500, 0));
            pointsToProject.Add(new Point(0, -500, 500));
            pointsToProject.Add(new Point(0, 0, -500));
            pointsToProject.Add(new Point(0, 0, 0));
            pointsToProject.Add(new Point(0, 0, 500));
            pointsToProject.Add(new Point(0, 500, -500));
            pointsToProject.Add(new Point(0, 500, 0));
            pointsToProject.Add(new Point(0, 500, 500));


            var projectionVectors = new List<Vector>();
            projectionVectors.Add(new Vector(1, 0, 0));
            projectionVectors.Add(new Vector(1, 0, 0));
            projectionVectors.Add(new Vector(1, 0, 0));
            projectionVectors.Add(new Vector(1, 0, 0));
            projectionVectors.Add(new Vector(1, 0, 0));
            projectionVectors.Add(new Vector(1, 0, 0));
            projectionVectors.Add(new Vector(1, 0, 0));
            projectionVectors.Add(new Vector(1, 0, 0));
            projectionVectors.Add(new Vector(1, 0, 0));

            var projectedPoints = model.ProjectPoints(pointsToProject, projectionVectors);
            Assert.AreEqual(new Point(50, -500, -500), projectedPoints[0]);
            Assert.AreEqual(new Point(50, -500, 0), projectedPoints[1]);
            Assert.AreEqual(new Point(50, -500, 500), projectedPoints[2]);
            Assert.AreEqual(new Point(50, 0, -500), projectedPoints[3]);
            Assert.AreEqual(new Point(50, 0, 0), projectedPoints[4]);
            Assert.AreEqual(new Point(50, 0, 500), projectedPoints[5]);
            Assert.AreEqual(new Point(50, 500, -500), projectedPoints[6]);
            Assert.AreEqual(new Point(50, 500, 0), projectedPoints[7]);
            Assert.AreEqual(new Point(50, 500, 500), projectedPoints[8]);
            Assert.AreEqual(9, projectedPoints.Count);
        }

        [Test]
        public void GetBoundariesFromAMeshWithOneBoundaryTest()
        {
            DMTModel modelToZone = DMTModelReader.ReadFile(new File(TestFiles.FetchTestFile("BaseLeftFootTopSurface.dmt")));
            var boundaries = modelToZone.Boundaries();

            Assert.AreEqual(boundaries.Count, 1, "Failed to calculate Boundaries. Mesh should have only one boundary.");
        }

        [Test]
        public void GetBoundariesFromAMeshWithFiveBoundariesTest()
        {
            DMTModel modelToZone = DMTModelReader.ReadFile(new File(TestFiles.FetchTestFile("MeshForBoundaryCount.dmt")));
            var boundaries = modelToZone.Boundaries();

            Assert.AreEqual(boundaries.Count, 5, "Failed to calculate Boundaries. Mesh only has five boundaries.");
        }

        [Test]
        public void GetBoundariesFromAMeshWithTwoBoundariesTest()
        {
            DMTModel modelToZone =
                DMTModelReader.ReadFile(new File(TestFiles.FetchTestFile("MeshForBoundaryCountAfterStitching.dmt")));
            var boundaries = modelToZone.Boundaries();

            Assert.AreEqual(boundaries.Count, 2, "Failed to calculate Boundaries. Mesh only has two boundaries.");
        }

        [Test]
        public void GetBoundaryNodesTest()
        {
            DMTModel modelToZone = DMTModelReader.ReadFile(new File(TestFiles.FetchTestFile("BaseLeftFootTopSurface.dmt")));
            var boundaryNodes = modelToZone.BoundaryNodes();

            Assert.AreEqual(boundaryNodes.Count, 313, "Failed to calculate BoundaryNodes.");
        }

        [Test]
        public void WhenMoveByVector_ThenAllBlocksShouldBeMoved()
        {
            DMTModel modelToMove = DMTModelReader.ReadFile(new File(TestFiles.SmallModel));
            var movement = new Vector(1, 4, 2);
            var originalVertices = modelToMove.TriangleBlocks[0].Vertices.Cast<DMTVertex>().ToList();
            originalVertices.ForEach(x => x.Position = x.Position + movement);
            modelToMove.Move(movement);
            var newVertices = modelToMove.TriangleBlocks[0].Vertices.Cast<DMTVertex>().ToList();

            CollectionAssert.AreEqual(originalVertices, newVertices);
        }

        #region Implementation
            private static T ReadXmlFile<T>(string fileName)
            {
                System.Xml.Serialization.XmlSerializer reader =
                    new System.Xml.Serialization.XmlSerializer(typeof(T));
                System.IO.StreamReader file = new System.IO.StreamReader(fileName);
                T result = (T)reader.Deserialize(file);
                file.Close();

                return result;
            } 
        #endregion
    }
}