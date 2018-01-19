// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Autodesk.FileSystem;
using NUnit.Framework;

namespace Autodesk.Geometry.Test.GeometricEntities
{
    [TestFixture]
    public class WorkplaneTest
    {
        [Test]
        public void CreateWorkplaneFromMatFileTest()
        {
            File matFile =
                new File(AppDomain.CurrentDomain.BaseDirectory +
                         "\\..\\..\\TestFiles\\WokplaneTestFiles\\CADToMachineAlignment.mat");
            var workplane = new Workplane(matFile);

            Assert.IsTrue(workplane.Origin.Equals(new Point(227.895, 0, 0), 5),
                          "Origin of the workplane is not the expected one.");
            Assert.IsTrue(workplane.XAxis.Equals(new Vector(0, -1, 0), 5), "X-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane.YAxis.Equals(new Vector(0, 0, 1), 5), "Y-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane.ZAxis.Equals(new Vector(-1, 0, 0), 5), "Z-axis of the workplane is not the expected one.");
        }

        [Test]
        public void CreateWorkplaneFromMatFileTest2()
        {
            File matFile =
                new File(AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\TestFiles\\WokplaneTestFiles\\BigAlignment.mat");
            var workplane = new Workplane(matFile);

            Assert.IsTrue(workplane.Origin.Equals(new Point(299.4922, -0.381127, 0.652323), 5),
                          "Origin of the workplane is not the expected one.");
            Assert.IsTrue(workplane.XAxis.Equals(new Vector(1.3E-05, 0.946991, -0.321261), 5),
                          "X-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane.YAxis.Equals(new Vector(-4.5E-05, -0.321261, -0.946991), 4),
                          "Y-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane.ZAxis.Equals(new Vector(-1, 2.6E-05, 3.9E-05), 5),
                          "Z-axis of the workplane is not the expected one.");
        }

        [Test]
        public void CreateWorkplaneFromANonMatFile_EnsureSensibleErrorTest()
        {
            Assert.Throws<ArgumentException>(CreateWorkplaneFromANoMatFile);
        }

        private void CreateWorkplaneFromANoMatFile()
        {
            File matFile =
                new File(AppDomain.CurrentDomain.BaseDirectory +
                         "\\..\\..\\TestFiles\\WokplaneTestFiles\\WorkplaneFromCADToMachineAlignment.dgk");

            var workplane = new Workplane(matFile);
        }

        [Test]
        public void CreateWorkplaneFromInvertedMatFileTest()
        {
            File matFile =
                new File(AppDomain.CurrentDomain.BaseDirectory +
                         "\\..\\..\\TestFiles\\WokplaneTestFiles\\CADToMachineAlignmentInverted.mat");
            var workplane = new Workplane(matFile);

            Assert.IsTrue(new Point(0, 0, 227.895).Equals(workplane.Origin, 5),
                          "Origin of the workplane is not the expected one.");
            Assert.IsTrue(new Vector(0, 0, -1).Equals(workplane.XAxis, 5), "X-axis of the workplane is not the expected one.");
            Assert.IsTrue(new Vector(-1, 0, 0).Equals(workplane.YAxis, 5), "Y-axis of the workplane is not the expected one.");
            Assert.IsTrue(new Vector(0, 1, 0).Equals(workplane.ZAxis, 5), "Z-axis of the workplane is not the expected one.");
        }

        [Test]
        public void CreateWorkplaneFromMatFileUsingALocalNumberFormatTest()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");

            File matFile =
                new File(AppDomain.CurrentDomain.BaseDirectory +
                         "\\..\\..\\TestFiles\\WokplaneTestFiles\\CADToMachineAlignment.mat");
            var workplane = new Workplane(matFile);

            Assert.IsTrue((workplane.Origin.X == 227.895) & (workplane.Origin.Y == 0) & (workplane.Origin.Z == 0),
                          "Origin of the workplane is not the expected one.");
            Assert.IsTrue(workplane.XAxis.Equals(new Vector(0, -1, 0), 5), "X-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane.YAxis.Equals(new Vector(0, 0, 1), 5), "Y-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane.ZAxis.Equals(new Vector(-1, 0, 0), 5), "Z-axis of the workplane is not the expected one.");
        }

        [Test]
        public void WriteToMatFileTest()
        {
            File matFile =
                new File(AppDomain.CurrentDomain.BaseDirectory +
                         "\\..\\..\\TestFiles\\WokplaneTestFiles\\CADToMachineAlignment.mat");
            var workplane = new Workplane(matFile);

            var output = File.CreateTemporaryFile("mat");

            workplane.WriteToMat(output);

            // Comparing the content of the files is not enough, it may be more than one sequence of rotations that results in the same orientation of the workplane
            // http://www.staff.city.ac.uk/~sbbh653/publications/euler.pdf
            var workplane2 = new Workplane(output);
            Assert.IsTrue(workplane2.Origin.Equals(workplane.Origin, 5), "Origin of the workplane is not the expected one.");
            Assert.IsTrue(workplane2.XAxis.Equals(workplane.XAxis, 5), "X-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane2.YAxis.Equals(workplane.YAxis, 5), "Y-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane2.ZAxis.Equals(workplane.ZAxis, 5), "Z-axis of the workplane is not the expected one.");

            // Delete temp file
            output.Delete();
        }

        [Test]
        public void GetTransformationMatrixTest()
        {
            File matFile =
                new File(AppDomain.CurrentDomain.BaseDirectory +
                         "\\..\\..\\TestFiles\\WokplaneTestFiles\\CADToMachineAlignment.mat");
            var workplane = new Workplane(matFile);

            var output = workplane.GetTransformationMatrix();

            Assert.IsTrue(workplane.Origin == new Point(output[0, 3], output[1, 3], output[2, 3]),
                          "Origin of the workplane is not the expected one.");
            Assert.IsTrue(workplane.XAxis == new Vector(output[0, 0], output[1, 0], output[2, 0]),
                          "X-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane.YAxis == new Vector(output[0, 1], output[1, 1], output[2, 1]),
                          "Y-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane.ZAxis == new Vector(output[0, 2], output[1, 2], output[2, 2]),
                          "Z-axis of the workplane is not the expected one.");
        }

        [Test]
        public void WriteToMatFileUsingALocalNumberFormatTest()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            File matFile =
                new File(AppDomain.CurrentDomain.BaseDirectory +
                         "\\..\\..\\TestFiles\\WokplaneTestFiles\\CADToMachineAlignment.mat");
            var workplane = new Workplane(matFile);

            var output = File.CreateTemporaryFile("mat");
            workplane.WriteToMat(output);

            // Comparing the content of the files is not enough, it may be more than one sequence of rotations that results in the same orientation of the workplane
            // http://www.staff.city.ac.uk/~sbbh653/publications/euler.pdf
            var workplane2 = new Workplane(output);
            Assert.IsTrue(workplane2.Origin.Equals(workplane.Origin, 5), "Origin of the workplane is not the expected one.");
            Assert.IsTrue(workplane2.XAxis.Equals(workplane.XAxis, 5), "X-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane2.YAxis.Equals(workplane.YAxis, 5), "Y-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane2.ZAxis.Equals(workplane.ZAxis, 5), "Z-axis of the workplane is not the expected one.");

            // Delete temp file
            output.Delete();
        }

        [Test]
        public void CreateWorkplaneFromTrxFileTest()
        {
            var trxFile = new File(AppDomain.CurrentDomain.BaseDirectory +
                                   @"\..\..\TestFiles\WokplaneTestFiles\HERMLE_Nominal_OWP.trx");

            var workplane = new Workplane(trxFile);
            Assert.IsTrue(workplane.Origin.Equals(new Point(0.0, 0.0, 0.0)),
                          "Origin of the workplane is not the expected one.");
            Assert.IsTrue(workplane.XAxis.Equals(new Vector(0.0, -1.0, 0.0)),
                          "X-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane.YAxis.Equals(new Vector(0.0, 0.0, 1.0)),
                          "Y-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane.ZAxis.Equals(new Vector(-1.0, 0.0, 0.0)),
                          "Z-axis of the workplane is not the expected one.");
        }

        [Test]
        public void WriteToTrxFileTest()
        {
            var trxFile = new File(AppDomain.CurrentDomain.BaseDirectory +
                                   @"\..\..\TestFiles\WokplaneTestFiles\HERMLE_Nominal_OWP.trx");

            var workplane = new Workplane(trxFile);
            var output = File.CreateTemporaryFile("trx", false);
            workplane.WriteToTrx(output);
            var workplane2 = new Workplane(output);
            Assert.IsTrue(workplane2.Origin.Equals(workplane.Origin), "Origin of the workplane is not the expected one.");
            Assert.IsTrue(workplane2.XAxis.Equals(workplane.XAxis), "X-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane2.YAxis.Equals(workplane.YAxis), "Y-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane2.ZAxis.Equals(workplane.ZAxis), "Z-axis of the workplane is not the expected one.");
            output.Delete();
        }

        [Test]
        public void WhenCreatingWorkplaneFromMatrix_ThenCheckOutput()
        {
            var matrixFile =
                new File(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\TestFiles\WokplaneTestFiles\x_30_ty10.matrix");
            var expected = new File(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\TestFiles\WokplaneTestFiles\x_30_ty10.mat");

            var workplane = new Workplane(matrixFile);
            var workplane2 = new Workplane(expected);
            Assert.IsTrue(workplane2.Origin.Equals(workplane.Origin, 5), "Origin of the workplane is not the expected one.");
            Assert.IsTrue(workplane2.XAxis.Equals(workplane.XAxis, 5), "X-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane2.YAxis.Equals(workplane.YAxis, 5), "Y-axis of the workplane is not the expected one.");
            Assert.IsTrue(workplane2.ZAxis.Equals(workplane.ZAxis, 5), "Z-axis of the workplane is not the expected one.");
        }

        [Test]
        public void WhenWritingWorkplaneToMatrix_ThenCheckOutput()
        {
            var matrixFile =
                new File(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\TestFiles\WokplaneTestFiles\x_30_ty10.matrix");
            var output = File.CreateTemporaryFile("matrix");

            var workplane = new Workplane(matrixFile);
            workplane.WriteToMatrix(output);

            bool filesAreTheSame = System.IO.File.ReadLines(output.Path).SequenceEqual(System.IO.File.ReadLines(matrixFile.Path));

            Assert.True(filesAreTheSame, "Failed to read matrix file.");
            output.Delete();
        }

        [Test]
        public void CreateWorkplaneFromVectors()
        {
            // Create single workplane
            var xAxis = new Vector(1, 0, 0);
            var yAxis = new Vector(0.00000000000000000000000001, 1, 0);
            var Wp = new Workplane(new Point(0, 10, 20), xAxis, yAxis);

            Assert.That(Wp, Is.Not.Null);
            Assert.That(Wp.XAxis, Is.EqualTo(xAxis));
            Assert.That(Wp.YAxis, Is.EqualTo(yAxis));
        }

        [Test]
        public void WhenVectorsAreNotPerpendicular_ShouldThrowException()
        {
            // Create single workplane
            Assert.Throws<ArgumentException>(() => new Workplane(new Point(0, 10, 20),
                                                                 new Vector(1, 0, 0),
                                                                 new Vector(0.1, 1, 0)));
        }

        [Test]
        public void WhenVectorsAreNotPerfectlyPerpendicular_ShouldNotThrowException()
        {
            Assert.DoesNotThrow(() =>
            {
                var workplane = new Workplane(new Point(368.8622, 74.3197, 550.1203),
                                              new Vector(0.1618, -0.9838, -0.0771),
                                              new Vector(0.4301, 0, 0.9028),
                                              new Vector(-0.8882, -0.1792, 0.4231));
            });
        }
    }
}