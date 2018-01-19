// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.IO;
using System.Linq;
using Autodesk.Geometry.GeometricEntities;
using NUnit.Framework;

namespace Autodesk.Geometry.Test.GeometricEntities
{
    [TestFixture]
    public class SurfaceTest
    {
        private string _testDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory,
                                                     @"..\..\TestFiles\SurfaceTestFiles\");

        [Test]
        public void WhenCreatingFromASetOfOpenCurves_GivenLaterals_ThenCheckPerpendiculars()
        {
            var curvesFile = Path.Combine(_testDirectory, "OpenCurves.pic");
            var expectedPerpendicularsFile = Path.Combine(_testDirectory, "OpenCurvesPerpendiculars.pic");
            var expectedCurves = Spline.ReadFromDUCTPictureFile(new FileSystem.File(expectedPerpendicularsFile));

            var curves = Spline.ReadFromDUCTPictureFile(new FileSystem.File(curvesFile));
            var surface = new Surface(curves, true);

            var numberOfPoints = surface.Longitudinals.First().Count;

            for (int i = 0; i < surface.Longitudinals.Count; i++)
            {
                for (int j = 0; j < numberOfPoints; j++)
                {
                    Assert.AreEqual(expectedCurves[i][j].X,
                                    surface.Longitudinals[i][j].X,
                                    "Perpendicular curves differ from the expected ones.");
                    Assert.AreEqual(expectedCurves[i][j].Y,
                                    surface.Longitudinals[i][j].Y,
                                    "Perpendicular curves differ from the expected ones.");
                    Assert.AreEqual(expectedCurves[i][j].Z,
                                    surface.Longitudinals[i][j].Z,
                                    "Perpendicular curves differ from the expected ones.");
                }
            }
        }

        [Test]
        public void WhenCreatingFromASetOfOpenCurves_GivenLongitudinals_ThenCheckPerpendiculars()
        {
            var curvesFile = Path.Combine(_testDirectory, "OpenCurves.pic");
            var expectedPerpendicularsFile = Path.Combine(_testDirectory, "OpenCurvesPerpendiculars.pic");
            var expectedCurves = Spline.ReadFromDUCTPictureFile(new FileSystem.File(expectedPerpendicularsFile));

            var curves = Spline.ReadFromDUCTPictureFile(new FileSystem.File(curvesFile));
            var surface = new Surface(curves, false);

            var numberOfPoints = surface.Laterals.First().Count;

            for (int i = 0; i < surface.Laterals.Count; i++)
            {
                for (int j = 0; j < numberOfPoints; j++)
                {
                    Assert.AreEqual(expectedCurves[i][j].X,
                                    surface.Laterals[i][j].X,
                                    "Perpendicular curves differ from the expected ones.");
                    Assert.AreEqual(expectedCurves[i][j].Y,
                                    surface.Laterals[i][j].Y,
                                    "Perpendicular curves differ from the expected ones.");
                    Assert.AreEqual(expectedCurves[i][j].Z,
                                    surface.Laterals[i][j].Z,
                                    "Perpendicular curves differ from the expected ones.");
                }
            }
        }

        [Test]
        public void WhenCreatingFromASetOfClosedCurves_ThenCheckPerpendiculars()
        {
            var curvesFile = Path.Combine(_testDirectory, "ClosedCurves.pic");
            var expectedPerpendicularsFile = Path.Combine(_testDirectory, "ClosedCurvesPerpendiculars.pic");
            var expectedCurves = Spline.ReadFromDUCTPictureFile(new FileSystem.File(expectedPerpendicularsFile));

            var curves = Spline.ReadFromDUCTPictureFile(new FileSystem.File(curvesFile));
            var surface = new Surface(curves, true);

            var numberOfPoints = surface.Longitudinals.First().Count;

            for (int i = 0; i < surface.Longitudinals.Count; i++)
            {
                for (int j = 0; j < numberOfPoints; j++)
                {
                    Assert.AreEqual(expectedCurves[i][j].X,
                                    surface.Longitudinals[i][j].X,
                                    "Perpendicular curves differ from the expected ones.");
                    Assert.AreEqual(expectedCurves[i][j].Y,
                                    surface.Longitudinals[i][j].Y,
                                    "Perpendicular curves differ from the expected ones.");
                    Assert.AreEqual(expectedCurves[i][j].Z,
                                    surface.Longitudinals[i][j].Z,
                                    "Perpendicular curves differ from the expected ones.");
                }
            }
        }

        [Test]
        public void WhenCreatingFromASetOfCurves_GivenLessThanTwoCurves_ThenThrowException()
        {
            var curvesFile = Path.Combine(_testDirectory, "SingleSurfaceCurve.pic");
            var curves = Spline.ReadFromDUCTPictureFile(new FileSystem.File(curvesFile));
            Assert.Throws(typeof(ArgumentException), () => new Surface(curves, true));
        }

        [Test]
        public void WhenCreatingFromTwoSetsOfCurves_GivenLessThanTwoCurves_ThenThrowException()
        {
            var curvesFile = Path.Combine(_testDirectory, "SingleSurfaceCurve.pic");
            var curves = Spline.ReadFromDUCTPictureFile(new FileSystem.File(curvesFile));
            Assert.Throws(typeof(ArgumentException), () => new Surface(curves, curves));
        }

        [Test]
        public void WhenCreatingFromASetOfCurves_GivenDifferentNumberOfPoints_ThenThrowException()
        {
            var curvesFile = Path.Combine(_testDirectory, "DifferentNumberOfPoints.pic");
            var curves = Spline.ReadFromDUCTPictureFile(new FileSystem.File(curvesFile));
            Assert.Throws(typeof(ArgumentException), () => new Surface(curves, true));
        }

        [Test]
        public void WhenCreatingFromTwoSetsOfCurves_GivenNonCoincidentPoints_ThenThrowException()
        {
            var lateralsFile = Path.Combine(_testDirectory, "NonCoincidentPoints.pic");
            var laterals = Spline.ReadFromDUCTPictureFile(new FileSystem.File(lateralsFile));
            var longitudinalsFile = Path.Combine(_testDirectory, "OpenCurvesPerpendiculars.pic");
            var longitudinals = Spline.ReadFromDUCTPictureFile(new FileSystem.File(longitudinalsFile));
            Assert.Throws(typeof(ArgumentException), () => new Surface(laterals, longitudinals));
        }

        [Test]
        public void WhenCreatingFromTwoSetsOfCurves_GivenDifferentNumberOfPoints_ThenThrowException()
        {
            var lateralsFile = Path.Combine(_testDirectory, "DifferentNumberOfPoints.pic");
            var laterals = Spline.ReadFromDUCTPictureFile(new FileSystem.File(lateralsFile));
            var longitudinalsFile = Path.Combine(_testDirectory, "OpenCurvesPerpendiculars.pic");
            var longitudinals = Spline.ReadFromDUCTPictureFile(new FileSystem.File(longitudinalsFile));
            Assert.Throws(typeof(ArgumentException), () => new Surface(laterals, longitudinals));
        }

        [Test]
        [TestCase("OpenCurves.pic", "OpenCurvesPerpendiculars.pic")]
        [TestCase("Laterals_1.pic", "Longitudinals_2.pic")]
        public void WhenCreatingFromTwoSetsOfCurves_GivenTheSameNumberOfPointsAndCoincident_ThenCreateSurface(
            string lateralsFileName,
            string longitudinalsFileName)
        {
            var lateralsFile = Path.Combine(_testDirectory, lateralsFileName);
            var laterals = Spline.ReadFromDUCTPictureFile(new FileSystem.File(lateralsFile));
            var longitudinalsFile = Path.Combine(_testDirectory, longitudinalsFileName);
            var longitudinals = Spline.ReadFromDUCTPictureFile(new FileSystem.File(longitudinalsFile));
            var surface = new Surface(laterals, longitudinals);

            Assert.Pass("Able to create a surface from two sets of curves.");
        }

        [Test]
        public void WhenGettingSurfaceNormal_ThenCheckItMatchesNormalInPowerShape()
        {
            var lateralsFile = Path.Combine(_testDirectory, "Laterals.pic");
            var laterals = Spline.ReadFromDUCTPictureFile(new FileSystem.File(lateralsFile));
            var longitudinalsFile = Path.Combine(_testDirectory, "Longitudinals.pic");
            var longitudinals = Spline.ReadFromDUCTPictureFile(new FileSystem.File(longitudinalsFile));
            var surface = new Surface(laterals, longitudinals);
            var normal = surface.GetNormal(1, 1);

            // It is enough to compare till 1 micro, usually the accuracies that we work are 10 micros
            // The normal also matches surface at 2 and 2
            var normalInPowerShape = new Vector(0.0, -0.138196, 0.990405);
            Assert.That(normal.I.Value, Is.EqualTo(normalInPowerShape.I.Value).Within(0.001));
            Assert.That(normal.J.Value, Is.EqualTo(normalInPowerShape.J.Value).Within(0.001));
            Assert.That(normal.K.Value, Is.EqualTo(normalInPowerShape.K.Value).Within(0.001));
        }

        [Test]
        public void WhenGettingSurfaceNormal_GivenDifferentNumberOfLateralsAndLongitudinals_ThenCheckExpectionIsNotThrown()
        {
            var lateralsFile = Path.Combine(_testDirectory, "Offset_Laterals_2.pic");
            var laterals = Spline.ReadFromDUCTPictureFile(new FileSystem.File(lateralsFile));
            var longitudinalsFile = Path.Combine(_testDirectory, "Offset_Longitudinals_2.pic");
            var longitudinals = Spline.ReadFromDUCTPictureFile(new FileSystem.File(longitudinalsFile));
            var surface = new Surface(laterals, longitudinals);

            var normal = surface.GetNormal(14, 15);
            Assert.DoesNotThrow(() => surface.GetNormal(14, 15));
        }
    }
}