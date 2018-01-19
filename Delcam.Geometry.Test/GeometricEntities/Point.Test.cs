// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.Geometry.Euler;
using NUnit.Framework;

namespace Autodesk.Geometry.Test.GeometricEntities
{
    //<TestFixture("Point Test", "Test Points Class")> _
    /// <summary>
    /// This is a test class for PointTest and is intended
    /// to contain all PointTest Unit Tests
    /// </summary>
    [TestFixture]
    public class PointTest
    {
        #region "Test Context"

        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        #endregion

        #region "Unit Tests"

        /// <summary>
        /// A test for X
        /// </summary>
        [Test]
        public void XTest()
        {
            Point target = new Point(1.0, 2.0, 3.0);
            MM expected = 1.0;
            MM actual = default(MM);
            target.X = expected;
            actual = target.X;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Y
        /// </summary>
        [Test]
        public void YTest()
        {
            Point target = new Point(1.0, 2.0, 3.0);
            MM expected = 2.0;
            MM actual = default(MM);
            target.Y = expected;
            actual = target.Y;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Z
        /// </summary>
        [Test]
        public void ZTest()
        {
            Point target = new Point(1.0, 2.0, 3.0);
            MM expected = 3.0;
            MM actual = default(MM);
            target.Z = expected;
            actual = target.Z;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for ToString
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            Point target = new Point(1.1234, 2.2345, 3.3456);
            string expected = "1.1234 2.2345 3.3456";
            string actual = null;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for RotateAboutXAxis
        /// </summary>
        [Test]
        public void RotateAboutXAxisTest()
        {
            Point target = null;
            Point pointToRotateAbout = new Point(-1.0, 1.0, 2.0);
            Radian angleRadian = Math.PI / 2.0;
            Degree angleDegree = 90.0;
            Point expected = new Point(1.0, -1.0, 3.0);

            target = new Point(1.0, 2.0, 4.0);
            target.RotateAboutXAxis(pointToRotateAbout, angleRadian);
            Assert.AreEqual(expected, target);

            target = new Point(1.0, 2.0, 4.0);
            target.RotateAboutXAxis(pointToRotateAbout, angleDegree);
            Assert.AreEqual(expected, target);
        }

        /// <summary>
        /// A test for RotateAboutYAxis
        /// </summary>
        [Test]
        public void RotateAboutYAxisTest()
        {
            Point target = null;
            Point pointToRotateAbout = new Point(-1.0, 1.0, 2.0);
            Radian angleRadian = Math.PI / 2.0;
            Degree angleDegree = 90.0;
            Point expected = new Point(1, 2, 2.22044604925031E-16);

            target = new Point(1.0, 2.0, 4.0);
            target.RotateAboutYAxis(pointToRotateAbout, angleRadian);
            Assert.IsTrue(expected.Equals(target, 12));

            target = new Point(1.0, 2.0, 4.0);
            target.RotateAboutYAxis(pointToRotateAbout, angleDegree);
            Assert.IsTrue(expected.Equals(target, 12));
        }

        /// <summary>
        /// A test for RotateAboutZAxis
        /// </summary>
        [Test]
        public void RotateAboutZAxisTest()
        {
            Point target = null;
            Point pointToRotateAbout = new Point(-1.0, 1.0, 2.0);
            Radian angleRadian = Math.PI / 2.0;
            Degree angleDegree = 90.0;
            Point expected = new Point(-2.0, 3.0, 4.0);

            target = new Point(1.0, 2.0, 4.0);
            target.RotateAboutZAxis(pointToRotateAbout, angleRadian);
            Assert.AreEqual(expected, target);

            target = new Point(1.0, 2.0, 4.0);
            target.RotateAboutZAxis(pointToRotateAbout, angleDegree);
            Assert.AreEqual(expected, target);
        }

        /// <summary>
        /// A test for DistanceToPoint
        /// </summary>
        [Test]
        public void DistanceToPointTest()
        {
            Point thisPoint = new Point(0, 1, 0);
            Point otherPoint = new Point(4, 4, 0);
            Assert.IsTrue(thisPoint.DistanceToPoint(otherPoint) == 5);
        }

        /// <summary>
        /// A test for RebaseToWorkplane
        /// </summary>
        [Test]
        public void RebaseToWorkplaneTest()
        {
            Point target = new Point(1.0, 2.0, 3.0);
            Workplane workplane = new Workplane(new Point(10, 30, 40),
                                                new Vector(-1.0, 0.0, 0.0),
                                                new Vector(0.0, -1.0, 0.0),
                                                new Vector(0.0, 0.0, -1.0));
            Point expected = new Point(9.0, 28.0, 37.0);

            target.RebaseToWorkplane(workplane);
            Assert.AreEqual(expected, target);
        }

        /// <summary>
        /// A test for RebaseFromWorkplane
        /// </summary>
        [Test]
        public void RebaseFromWorkplaneTest()
        {
            Point target = new Point(9.0, 28.0, 37.0);
            Workplane workplane = new Workplane(new Point(10, 30, 40),
                                                new Vector(-1.0, 0.0, 0.0),
                                                new Vector(0.0, -1.0, 0.0),
                                                new Vector(0.0, 0.0, -1.0));
            Point expected = new Point(1.0, 2.0, 3.0);

            target.RebaseFromWorkplane(workplane);
            Assert.AreEqual(expected, target);
        }

        /// <summary>
        /// A test for Subtraction of a Vector from a Point to give a Point
        /// </summary>
        [Test]
        public void PointMinusVectorTest()
        {
            Point left = new Point(1.0, 2.0, 3.0);
            Vector right = new Vector(3.0, 6.0, 9.0);
            Point expected = new Point(-2.0, -4.0, -6.0);
            Point actual = null;
            actual = left - right;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Subtraction of a Point from a Point to give a Vector
        /// </summary>
        [Test]
        public void SubtractionTest()
        {
            Point left = new Point(2.0, -1.0, 7.0);
            Point right = new Point(3.0, -4.0, -10.0);
            Vector expected = new Vector(-1.0, 3.0, 17.0);
            Vector actual = null;
            actual = left - right;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for op_Multiply
        /// </summary>
        [Test]
        public void MultiplyTest()
        {
            double scalar = 2.5;
            Point pointValue = null;
            Point expected = new Point(5.0, 2.5, -10.0);
            Point actual = null;

            pointValue = new Point(2.0, 1.0, -4.0);
            actual = scalar * pointValue;
            Assert.AreEqual(expected, actual);

            pointValue = new Point(2.0, 1.0, -4.0);
            actual = pointValue * scalar;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Inequality
        /// </summary>
        [Test]
        public void InequalityTest()
        {
            Point left = null;
            Point right = null;
            bool expected = false;
            bool actual = false;

            left = new Point(1.0, 2.0, 3.0);
            right = new Point(1.0, 2.0, 3.0);
            expected = false;
            actual = left != right;
            Assert.AreEqual(expected, actual);

            left = new Point(1.0, 2.0, 4.0);
            right = new Point(1.0, 2.0, 3.0);
            expected = true;
            actual = left != right;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Inequality of MM array and a Point
        /// </summary>
        [Test]
        public void InequalityOfMMArrayAndPointTest()
        {
            MM[] left = null;
            Point right = null;
            bool expected = false;
            bool actual = false;

            left = new MM[]
            {
                1.0,
                2.0,
                3.0
            };
            right = new Point(1.0, 2.0, 3.0);
            expected = false;
            actual = left != right;
            Assert.AreEqual(expected, actual);
            actual = right != left;
            Assert.AreEqual(expected, actual);

            left = new MM[]
            {
                1.0,
                2.0,
                4.0
            };
            right = new Point(1.0, 2.0, 3.0);
            expected = true;
            actual = left != right;
            Assert.AreEqual(expected, actual);
            actual = right != left;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Equality
        /// </summary>
        [Test]
        public void EqualityTest()
        {
            Point left = null;
            Point right = null;
            bool expected = false;
            bool actual = false;

            left = new Point(1.0, 2.0, 3.0);
            right = new Point(1.0, 2.0, 3.0);
            expected = true;
            actual = left == right;
            Assert.AreEqual(expected, actual);

            left = new Point(1.0, 2.0, 4.0);
            right = new Point(1.0, 2.0, 3.0);
            expected = false;
            actual = left == right;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Equality of MM array and a Point
        /// </summary>
        [Test]
        public void EqualityOfMmArrayAndPointTest()
        {
            MM[] left = null;
            Point right = null;
            bool expected = false;
            bool actual = false;

            left = new MM[]
            {
                1.0,
                2.0,
                3.0
            };
            right = new Point(1.0, 2.0, 3.0);
            expected = true;
            actual = left == right;
            Assert.AreEqual(expected, actual);
            actual = right == left;
            Assert.AreEqual(expected, actual);

            left = new MM[]
            {
                1.0,
                2.0,
                4.0
            };
            right = new Point(1.0, 2.0, 3.0);
            expected = false;
            actual = left == right;
            Assert.AreEqual(expected, actual);
            actual = right == left;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Division
        /// </summary>
        [Test]
        public void DivisionTest()
        {
            Point left = new Point(2.0, 6.0, 9.0);
            double scalar = 1.5;
            Point expected = new Point(2.0 / 1.5, 4.0, 6.0);
            Point actual = null;
            actual = left / scalar;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Addition
        /// </summary>
        [Test]
        public void AdditionTest()
        {
            Point left = new Point(2.0, -1.0, -4.0);
            Vector right = new Vector(3.5, -2.5, 4.5);
            Point expected = new Point(5.5, -3.5, 0.5);
            Point actual = null;
            actual = left + right;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for EulerRotation
        /// </summary>
        [Test]
        public void EulerRotationTest()
        {
            Point target = new Point(2, 3, 4);
            Euler.Angles angle = new Euler.Angles(0.5, 0.6, 0.7, Conventions.XYX);
            target.EulerRotation(angle);
            Assert.IsTrue(new Point(4.44486484901456, -1.35675116542822, 2.72073569997234).Equals(target, 12));
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void EqualsTest()
        {
            Point target = null;
            object obj = null;
            bool expected = false;
            bool actual = false;

            target = new Point(1.0, 2.0, 3.0);
            obj = 2.0;
            expected = false;
            actual = target.Equals(obj);
            Assert.AreEqual(expected, actual);

            obj = new Point(1.0, 2.0, 3.0);
            expected = true;
            actual = target.Equals(obj);
            Assert.AreEqual(expected, actual);

            obj = new Point(1.0, 32.0, 3.0);
            expected = false;
            actual = target.Equals(obj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Clone
        /// </summary>
        [Test]
        public void CloneTest()
        {
            Point target = new Point(1.0, 2.0, 3.0);
            Point expected = new Point(1.0, 2.0, 3.0);
            Point actual = null;
            actual = target.Clone();
            Assert.AreEqual(expected, actual);
            Assert.AreNotSame(expected, actual);
        }

        /// <summary>
        /// A test for Point Constructor
        /// </summary>
        [Test]
        public void PointConstructorTest3()
        {
            MM x = 1.0;
            MM y = 2.0;
            MM z = 3.0;
            Point target = new Point(x, y, z);

            Assert.AreEqual(x, target.X);
            Assert.AreEqual(y, target.Y);
            Assert.AreEqual(z, target.Z);
        }

        /// <summary>
        /// A test for Point Constructor
        /// </summary>
        [Test]
        public void PointConstructorTest2()
        {
            MM x = 0.0;
            MM y = 0.0;
            MM z = 0.0;
            Point target = new Point();

            Assert.AreEqual(x, target.X);
            Assert.AreEqual(y, target.Y);
            Assert.AreEqual(z, target.Z);
        }

        /// <summary>
        /// A test for Point Constructor
        /// </summary>
        [Test]
        public void PointConstructorTest1()
        {
            string textLine = "1.234 2.345 3.456";
            char delimiter = ' ';
            Point target = new Point(textLine, delimiter);

            MM x = 1.234;
            MM y = 2.345;
            MM z = 3.456;

            Assert.AreEqual(x, target.X);
            Assert.AreEqual(y, target.Y);
            Assert.AreEqual(z, target.Z);
        }

        /// <summary>
        /// A test for Point Constructor
        /// </summary>
        [Test]
        public void PointConstructorTest()
        {
            MM x = 1.123;
            MM y = 2.345;
            MM z = 3.456;

            MM[] pointArray =
            {
                x,
                y,
                z
            };
            Point target = new Point(pointArray);

            Assert.AreEqual(x, target.X);
            Assert.AreEqual(y, target.Y);
            Assert.AreEqual(z, target.Z);
        }

        [Test]
        public void NullPointTest()
        {
            Point point = new Point();
            Assert.DoesNotThrow(() =>
            {
                var a = point == null;
            });
        }

        [Test]
        public void NotNullPointTest()
        {
            Point point = new Point();
            Assert.DoesNotThrow(() =>
            {
                var a = point == null;
            });
        }

        [Test]
        public void PointIsOutside_IsInsideTriangleTest()
        {
            var vertex1 = new Point(237.546414, -266.545037, 662.823076);
            var vertex2 = new Point(242.604951, -269.07227, 662.823076);
            var vertex3 = new Point(240.043176, -271.357651, 662.823076);
            var pointOutsideTriangle = new Point(242.500137, -270.985504, 662.823076);

            var result = pointOutsideTriangle.IsInsideTriangle(vertex1, vertex2, vertex3);

            Assert.False(result, "Point is outside triangle, not inside.");
        }

        [Test]
        public void PointIsInside_IsInsideTriangleTest()
        {
            var vertex1 = new Point(237.546414, -266.545037, 662.823076);
            var vertex2 = new Point(242.604951, -269.07227, 662.823076);
            var vertex3 = new Point(240.043176, -271.357651, 662.823076);
            var pointInsideTriangle = new Point(239.710074, -268.528324, 662.823059);

            var result = pointInsideTriangle.IsInsideTriangle(vertex1, vertex2, vertex3);

            Assert.True(result, "Point is inside triangle, not outside.");
        }

        [Test]
        public void WhenTestRayInterceptTriangle_GivenProjectedPointIsInside_ThenItShouldReturnTrue()
        {
            // Given
            var vertex1 = new Point(-1, 0, 0);
            var vertex2 = new Point(1, 0, 0);
            var vertex3 = new Point(0, 1, 0);

            var point = new Point(0, 0.5, 1);
            var rayDirection = new Vector(0, 0, -1);

            var projectedPoint = new Point(0, 0.5, 0);

            Assert.IsTrue(projectedPoint.IsInsideTriangle(vertex1, vertex2, vertex3));

            // When
            var result = point.IsRayInterceptTriangleTest(rayDirection, vertex1, vertex2, vertex3);

            // Then
            Assert.IsTrue(result);
        }

        [Test]
        public void WhenTestRayInterceptTriangle_GivenProjectedPointIsOutside_ThenItShouldReturnTrue()
        {
            // Given
            var vertex1 = new Point(-1, 0, 0);
            var vertex2 = new Point(1, 0, 0);
            var vertex3 = new Point(0, 1, 0);

            var point = new Point(0, 3, 1);
            var rayDirection = new Vector(0, 0, -1);

            var projectedPoint = new Point(0, 3, 0);

            Assert.IsFalse(projectedPoint.IsInsideTriangle(vertex1, vertex2, vertex3));

            // When
            var result = point.IsRayInterceptTriangleTest(rayDirection, vertex1, vertex2, vertex3);

            // Then
            Assert.IsFalse(result);
        }

        [Test]
        public void WhenTestRayInterceptTriangle_GivenProjectedPointLiesOnSide_ThenItShouldReturnTrue()
        {
            // Given project point lies between v1 & v2
            var vertex1 = new Point(-1, 0, 0);
            var vertex2 = new Point(1, 0, 0);
            var vertex3 = new Point(0, 1, 0);

            var point = new Point(0.5, 0, 1);
            var rayDirection = new Vector(0, 0, -1);

            var projectedPoint = new Point(0.5, 0, 0);

            var pv1 = projectedPoint - vertex1;
            var v2v1 = vertex2 - vertex1;

            // Align
            Assert.That(Vector.CrossProduct(pv1, v2v1).Magnitude, Is.EqualTo((MM) 0));

            // P between v1 v2
            var x = Vector.DotProduct(pv1, v2v1);
            Assert.That(x, Is.GreaterThan((MM) 0));
            Assert.That(x, Is.LessThan((MM) (v2v1.Magnitude * v2v1.Magnitude)));

            // When
            var result = point.IsRayInterceptTriangleTest(rayDirection, vertex1, vertex2, vertex3);

            // Then
            Assert.IsTrue(result);
        }

        #endregion
    }
}