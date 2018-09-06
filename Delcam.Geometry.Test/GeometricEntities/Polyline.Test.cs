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
using NUnit.Framework;

namespace Autodesk.Geometry.Test.GeometricEntities
{
    [TestFixture]
    public class PolylineTest
    {
        [Test]
        public void ReadDUCTPICFileTest()
        {
            // Read in the polylines
            var lines =
                Polyline.ReadFromDUCTPictureFile(
                    new File(AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\TestFiles\\PicForPolylineTest.pic"));

            // Assert that we read all the lines
            Assert.AreEqual(43, lines.Count);

            // Now check that all the points on each line are on the same Z plane.  The original issue (bepokework#) was that lines start 
            // started with a conic arc picked up points from the wrong place in the file
            foreach (Polyline line in lines)
            {
                var firstPoint = line[0];
                for (int i = 0; i <= line.Count - 1; i++)
                {
                    Assert.AreEqual(firstPoint.Z, line[i].Z);
                }
            }
        }

        [Test]
        public void RepointOpenPolylineTest()
        {
            // Read in the polyline
            Polyline line =
                Polyline.ReadFromDUCTPictureFile(
                            new File(AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\TestFiles\\PicForOpenPolylineTest.pic"))
                        .Single();

            // Assert that we read all the lines
            Assert.AreEqual(8, line.Count);

            line.Repoint(12);
            Assert.AreEqual(line.Count, 12, "Polyline failed to Repoint. It should have 20 points.");
        }

        [Test]
        public void WriteClosedPolylineTest()
        {
            FileSystem.File tempFile = FileSystem.File.CreateTemporaryFile("pic");
            List<Point> points = new List<Point>();
            points.Add(new Point(0, 0, 0));
            points.Add(new Point(10, 5, 2));
            points.Add(new Point(20, 9, 5));

            Polyline line = new Polyline(points);

            line.WriteToDUCTPictureFile(tempFile);

            Polyline newline = Polyline.ReadFromDUCTPictureFile(tempFile).Single();
            // Assert that the polyline read from the file is open
            Assert.IsFalse(newline.IsClosed);

            // Close the polyline and write again
            line.IsClosed = true;

            line.WriteToDUCTPictureFile(tempFile);

            newline = Polyline.ReadFromDUCTPictureFile(tempFile).Single();
            // Assert that the polyline read from the file is closed
            Assert.IsTrue(newline.IsClosed);
            tempFile.Delete();
        }

        [Test]
        public void
            WhenDensifyPolylineWithOneSegment_GivenLengthIsExactMultipleOfMaxInput_ThenEachNewSegmentsShouldBeInThatLength()
        {
            // Given
            Point startPoint = new Point(0, 0, 0);
            Point endPoint = new Point(0, 0, 10);
            double newMax = 2;
            var vector = new Vector(startPoint, endPoint);
            var length = vector.Magnitude;
            Assert.IsTrue(length % newMax == 0);

            // When
            var poliline = new Polyline(new List<Point> {startPoint, endPoint});
            poliline.Densify(newMax);

            // Then
            Assert.That(poliline.Count, Is.EqualTo(6));
            for (int i = 0; i < poliline.Count - 1; i++)
            {
                var p1 = poliline[i];
                var p2 = poliline[i + 1];
                var v = new Vector(p1, p2);
                Assert.That((double) v.Magnitude, Is.EqualTo(newMax));
            }
        }
    }
}