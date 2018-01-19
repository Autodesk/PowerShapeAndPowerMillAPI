// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using NUnit.Framework;

namespace Autodesk.Geometry.Test.GeometricEntities
{
    [TestFixture]
    public class LineTest
    {
        [Test]
        public void WhenTestIntersection_GivenTwoLinesAreNotCoplaner_ThenItShouldReturnFalse()
        {
            // Given
            var line1 = new Line(new Point(-5, 0, 0), new Point(5, 0, 0));
            var line2 = new Line(new Point(0, -5, 2), new Point(0, 5, 2));

            // When
            var result = line1.Intersects(line2);

            // Then
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void WhenTestIntersection_GivenTwoLinesAreParallel_ThenItShouldReturnFalse()
        {
            // Given
            var line1 = new Line(new Point(0, 0, 0), new Point(0, 5, 0));
            var line2 = new Line(new Point(2, 0, 0), new Point(2, 5, 0));

            // When
            var result = line1.Intersects(line2);

            // Then
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void WhenTestIntersection_GivenTwoLinesInterceptOnOneEnd_ThenItShouldReturnTrue()
        {
            // Given
            var line1 = new Line(new Point(0, 0, 0), new Point(0, 5, 0));
            var line2 = new Line(new Point(0, 0, 0), new Point(2, 5, 0));

            // When
            var result = line1.Intersects(line2);

            // Then
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void WhenTestIntersection_GivenTwoLinesInterceptInTheMiddle_ThenItShouldReturnTrue()
        {
            // Given
            var line1 = new Line(new Point(0, -5, 0), new Point(0, 5, 0));
            var line2 = new Line(new Point(-5, 0, 0), new Point(5, 0, 0));

            // When
            var result = line1.Intersects(line2);

            // Then
            Assert.That(result, Is.EqualTo(true));
        }
    }
}