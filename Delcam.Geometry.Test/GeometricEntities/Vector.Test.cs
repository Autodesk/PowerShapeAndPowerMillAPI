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
    /// <summary>
    /// This is the test class for Vector and contains all unit tests for that class
    /// </summary>
    [TestFixture]
    public class VectorTest
    {
        #region "Test Context"

        private TestContext _testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get { return _testContextInstance; }
            set { _testContextInstance = value; }
        }

        #endregion

        #region "Tests"

        /// <summary>
        /// A test for Magnitude
        /// </summary>
        [Test]
        public void MagnitudeTest()
        {
            Vector target = null;
            MM expected = default(MM);
            MM actual = default(MM);

            target = new Vector(2.0, 3.0, 4.0);
            expected = Math.Sqrt(29);

            actual = target.Magnitude;
            Assert.AreEqual(expected, actual);

            target = new Vector(-2.0, -3.0, -4.0);
            expected = Math.Sqrt(29);

            actual = target.Magnitude;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for I
        /// </summary>
        [Test]
        public void ITest()
        {
            Vector target = new Vector();
            MM expected = 2.0;
            MM actual = default(MM);
            target.I = expected;
            actual = target.I;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for J
        /// </summary>
        [Test]
        public void JTest()
        {
            Vector target = new Vector();
            MM expected = 2.0;
            MM actual = default(MM);
            target.J = expected;
            actual = target.J;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for K
        /// </summary>
        [Test]
        public void KTest()
        {
            Vector target = new Vector();
            MM expected = 2.0;
            MM actual = default(MM);
            target.K = expected;
            actual = target.K;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for RotateAboutZAxis
        /// </summary>
        [Test]
        public void RotateAboutZAxisTest()
        {
            Vector target = null;
            Radian angleRadian = default(Radian);
            Degree angleDegree = default(Degree);
            Vector expected = null;

            // Rotate by PI/2
            target = new Vector(2.0, 3.0, 1.0);
            angleRadian = Math.PI / 2.0;
            expected = new Vector(-3.0, 2.0, 1.0);

            target.RotateAboutZAxis(angleRadian);
            Assert.AreEqual(expected, target);

            // Rotate by 90degrees
            target = new Vector(2.0, 3.0, 1.0);
            angleDegree = 90.0;
            expected = new Vector(-3.0, 2.0, 1.0);

            target.RotateAboutZAxis(angleDegree);
            Assert.AreEqual(expected, target);

            // Rotate by PI
            target = new Vector(2.0, 3.0, 1.0);
            angleRadian = Math.PI;
            expected = new Vector(-2.0, -3.0, 1.0);

            target.RotateAboutZAxis(angleRadian);
            Assert.AreEqual(expected, target);

            // Rotate by 180degrees
            target = new Vector(2.0, 3.0, 1.0);
            angleDegree = 180.0;
            expected = new Vector(-2.0, -3.0, 1.0);

            target.RotateAboutZAxis(angleDegree);
            Assert.AreEqual(expected, target);

            // Rotate by -PI/2
            target = new Vector(2.0, 3.0, 1.0);
            angleRadian = -Math.PI / 2.0;
            expected = new Vector(3.0, -2.0, 1.0);

            target.RotateAboutZAxis(angleRadian);
            Assert.AreEqual(expected, target);

            // Rotate by -90degrees
            target = new Vector(2.0, 3.0, 1.0);
            angleDegree = -90.0;
            expected = new Vector(3.0, -2.0, 1.0);

            target.RotateAboutZAxis(angleDegree);
            Assert.AreEqual(expected, target);

            // Rotate by -PI
            target = new Vector(2.0, 3.0, 1.0);
            angleRadian = Math.PI;
            expected = new Vector(-2.0, -3.0, 1.0);

            target.RotateAboutZAxis(angleRadian);
            Assert.AreEqual(expected, target);

            // Rotate by -180degrees
            target = new Vector(2.0, 3.0, 1.0);
            angleDegree = 180.0;
            expected = new Vector(-2.0, -3.0, 1.0);

            target.RotateAboutZAxis(angleDegree);
            Assert.AreEqual(expected, target);
        }

        /// <summary>
        /// A test for RotateAboutYAxis
        /// </summary>
        [Test]
        public void RotateAboutYAxisTest()
        {
            Vector target = null;
            Radian angleRadian = default(Radian);
            Degree angleDegree = default(Degree);
            Vector expected = null;

            // Rotate by PI/2
            target = new Vector(2.0, 1.0, 3.0);
            angleRadian = Math.PI / 2.0;
            expected = new Vector(3.0, 1.0, -2.0);

            target.RotateAboutYAxis(angleRadian);
            Assert.AreEqual(expected, target);

            // Rotate by 90degrees
            target = new Vector(2.0, 1.0, 3.0);
            angleDegree = 90.0;
            expected = new Vector(3.0, 1.0, -2.0);

            target.RotateAboutYAxis(angleDegree);
            Assert.AreEqual(expected, target);

            // Rotate by PI
            target = new Vector(2.0, 1.0, 3.0);
            angleRadian = Math.PI;
            expected = new Vector(-2.0, 1.0, -3.0);

            target.RotateAboutYAxis(angleRadian);
            Assert.AreEqual(expected, target);

            // Rotate by 180degrees
            target = new Vector(2.0, 1.0, 3.0);
            angleDegree = 180.0;
            expected = new Vector(-2.0, 1.0, -3.0);

            target.RotateAboutYAxis(angleDegree);
            Assert.AreEqual(expected, target);

            // Rotate by -PI/2
            target = new Vector(2.0, 1.0, 3.0);
            angleRadian = -Math.PI / 2.0;
            expected = new Vector(-3.0, 1.0, 2.0);

            target.RotateAboutYAxis(angleRadian);
            Assert.AreEqual(expected, target);

            // Rotate by -90degrees
            target = new Vector(2.0, 1.0, 3.0);
            angleDegree = -90.0;
            expected = new Vector(-3.0, 1.0, 2.0);

            target.RotateAboutYAxis(angleDegree);
            Assert.AreEqual(expected, target);

            // Rotate by -PI
            target = new Vector(2.0, 1.0, 3.0);
            angleRadian = Math.PI;
            expected = new Vector(-2.0, 1.0, -3.0);

            target.RotateAboutYAxis(angleRadian);
            Assert.AreEqual(expected, target);

            // Rotate by -180degrees
            target = new Vector(2.0, 1.0, 3.0);
            angleDegree = 180.0;
            expected = new Vector(-2.0, 1.0, -3.0);

            target.RotateAboutYAxis(angleDegree);
            Assert.AreEqual(expected, target);
        }

        /// <summary>
        /// A test for RotateAboutXAxis
        /// </summary>
        [Test]
        public void RotateAboutXAxisTest()
        {
            Vector target = null;
            Radian angleRadian = default(Radian);
            Degree angleDegree = default(Degree);
            Vector expected = null;

            // Rotate by PI/2
            target = new Vector(1.0, 2.0, 3.0);
            angleRadian = Math.PI / 2.0;
            expected = new Vector(1.0, -3.0, 2.0);

            target.RotateAboutXAxis(angleRadian);
            Assert.AreEqual(expected, target);

            // Rotate by 90degrees
            target = new Vector(1.0, 2.0, 3.0);
            angleDegree = 90.0;
            expected = new Vector(1.0, -3.0, 2.0);

            target.RotateAboutXAxis(angleDegree);
            Assert.AreEqual(expected, target);

            // Rotate by PI
            target = new Vector(1.0, 2.0, 3.0);
            angleRadian = Math.PI;
            expected = new Vector(1.0, -2.0, -3.0);

            target.RotateAboutXAxis(angleRadian);
            Assert.AreEqual(expected, target);

            // Rotate by 180degrees
            target = new Vector(1.0, 2.0, 3.0);
            angleDegree = 180.0;
            expected = new Vector(1.0, -2.0, -3.0);

            target.RotateAboutXAxis(angleDegree);
            Assert.AreEqual(expected, target);

            // Rotate by -PI/2
            target = new Vector(1.0, 2.0, 3.0);
            angleRadian = -Math.PI / 2.0;
            expected = new Vector(1.0, 3.0, -2.0);

            target.RotateAboutXAxis(angleRadian);
            Assert.AreEqual(expected, target);

            // Rotate by -90degrees
            target = new Vector(1.0, 2.0, 3.0);
            angleDegree = -90.0;
            expected = new Vector(1.0, 3.0, -2.0);

            target.RotateAboutXAxis(angleDegree);
            Assert.AreEqual(expected, target);

            // Rotate by -PI
            target = new Vector(1.0, 2.0, 3.0);
            angleRadian = Math.PI;
            expected = new Vector(1.0, -2.0, -3.0);

            target.RotateAboutXAxis(angleRadian);
            Assert.AreEqual(expected, target);

            // Rotate by -180degrees
            target = new Vector(1.0, 2.0, 3.0);
            angleDegree = 180.0;
            expected = new Vector(1.0, -2.0, -3.0);

            target.RotateAboutXAxis(angleDegree);
            Assert.AreEqual(expected, target);
        }

        /// <summary>
        /// A test for RotateAboutVector
        /// </summary>
        [Test]
        public void RotateAboutVectorTest()
        {
            Vector target = null;
            Vector vectorToRotateAbout = null;
            Radian angleRadian = default(Radian);
            Degree angleDegree = default(Degree);
            Vector expected = null;

            // Rotate by PI
            target = new Vector(1.0, 2.0, 3.0);
            vectorToRotateAbout = new Vector(1.0, 1.0, 1.0);
            angleRadian = Math.PI;
            expected = new Vector(3.0, 2.0, 1.0);

            target.RotateAboutVector(vectorToRotateAbout, angleRadian);
            Assert.AreEqual(expected, target);

            // Rotate by 180degrees
            target = new Vector(1.0, 2.0, 3.0);
            vectorToRotateAbout = new Vector(1.0, 1.0, 1.0);
            angleDegree = 180.0;
            expected = new Vector(3.0, 2.0, 1.0);

            target.RotateAboutVector(vectorToRotateAbout, angleDegree);
            Assert.AreEqual(expected, target);
        }

        /// <summary>
        /// A test for RebaseFromWorkplane
        /// </summary>
        [Test]
        public void RebaseFromWorkplaneTest()
        {
            Vector target = new Vector(-1.0, -2.0, -3.0);
            Workplane workplane = new Workplane(new Point(10, 30, 40),
                                                new Vector(-1.0, 0.0, 0.0),
                                                new Vector(0.0, -1.0, 0.0),
                                                new Vector(0.0, 0.0, -1.0));
            Vector expected = new Vector(1.0, 2.0, 3.0);

            target.RebaseFromWorkplane(workplane);
            Assert.AreEqual(expected, target);
        }

        /// <summary>
        /// A test for RebaseToWorkplane
        /// </summary>
        [Test]
        public void RebaseToWorkplaneTest()
        {
            Vector target = new Vector(1.0, 2.0, 3.0);
            Workplane workplane = new Workplane(new Point(10, 30, 40),
                                                new Vector(-1.0, 0.0, 0.0),
                                                new Vector(0.0, -1.0, 0.0),
                                                new Vector(0.0, 0.0, -1.0));
            Vector expected = new Vector(-1.0, -2.0, -3.0);

            target.RebaseToWorkplane(workplane);
            Assert.AreEqual(expected, target);
        }

        /// <summary>
        /// A test for op_Subtraction
        /// </summary>
        [Test]
        public void SubtractionTest()
        {
            Vector left = new Vector(2.0, 3.0, 4.0);
            Vector right = new Vector(5.0, 7.0, 9.0);
            Vector expected = new Vector(-3.0, -4.0, -5.0);

            Vector actual = null;
            actual = left - right;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Dot Product
        /// </summary>
        [Test]
        public void DotProductTest()
        {
            Vector left = new Vector(2.0, 3.0, 4.0);
            Vector right = new Vector(5.0, 7.0, 9.0);
            MM expected = 67.0;

            MM actual = default(MM);
            actual = Vector.DotProduct(left, right);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Scalar Multiply
        /// </summary>
        [Test]
        public void ScalarMultiplyTest()
        {
            Vector vectorToScale = new Vector(2.0, 3.0, 4.0);
            double scalar = 2.5;
            Vector expected = new Vector(5.0, 7.5, 10.0);
            Vector actual = null;

            actual = vectorToScale * scalar;
            Assert.AreEqual(expected, actual);

            actual = scalar * vectorToScale;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for op_Inequality
        /// </summary>
        [Test]
        public void InequalityTest()
        {
            Vector left = null;
            Vector right = null;
            bool expected = false;
            bool actual = false;

            left = new Vector(1.0, 2.0, 3.0);
            right = new Vector(2.0, 1.0, 3.0);
            expected = true;

            actual = left != right;
            Assert.AreEqual(expected, actual);

            actual = left != left;
            expected = false;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for CrossProduct
        /// </summary>
        [Test]
        public void CrossProductTest()
        {
            Vector left = new Vector(1.0, 0.0, 0.0);
            Vector right = new Vector(0.0, 1.0, 0.0);
            Vector expected = new Vector(0.0, 0.0, 1.0);
            Vector actual = null;

            actual = Vector.CrossProduct(left, right);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Equality
        /// </summary>
        [Test]
        public void EqualityTest()
        {
            Vector left = null;
            Vector right = null;
            bool expected = false;
            bool actual = false;

            left = new Vector(1.0, 2.0, 3.0);
            right = new Vector(2.0, 1.0, 3.0);
            expected = false;

            actual = left == right;
            Assert.AreEqual(expected, actual);

            actual = left == left;
            expected = true;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for op_Division
        /// </summary>
        [Test]
        public void op_DivisionTest()
        {
            Vector left = new Vector(7.0, 6.0, 5.0);
            double scalar = 2.0;
            Vector expected = new Vector(3.5, 3.0, 2.5);
            Vector actual = null;

            actual = left / scalar;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for op_Addition
        /// </summary>
        [Test]
        public void op_AdditionTest()
        {
            Vector left = new Vector(2.0, 3.0, 4.0);
            Vector right = new Vector(5.0, 7.0, 9.0);
            Vector expected = new Vector(7.0, 10.0, 13.0);

            Vector actual = null;
            actual = left + right;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Normalize
        /// </summary>
        [Test]
        public void NormalizeTest()
        {
            Vector target = new Vector(2.0, 7.0, 3.0);
            MM expected = 1.0;

            target.Normalize();
            MM actual = target.Magnitude;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GetXYVectors
        /// </summary>
        [Test]
        public void GetXYVectorsTest()
        {
            Assert.Inconclusive("Not sure what this code should actually do.");

            Vector target = new Vector(1.0, 2.0, 3.0);
            Vector xVector = null;
            Vector xVectorExpected = new Vector(1.0, 0.0, 0.0);
            Vector yVector = null;
            Vector yVectorExpected = new Vector(0.0, 1.0, 0.0);
            target.GetXYVectors(ref xVector, ref yVector);
            Assert.AreEqual(xVectorExpected, xVector);
            Assert.AreEqual(yVectorExpected, yVector);
        }

        /// <summary>
        /// A test for EulerRotation
        /// </summary>
        [Test]
        public void EulerRotationTest()
        {
            Vector target = new Vector(2, 3, 4);
            Euler.Angles angle = new Euler.Angles(0.5, 0.6, 0.7, Conventions.XYX);
            target.EulerRotation(angle);
            Assert.IsTrue(new Vector(4.44486484901456, -1.35675116542822, 2.72073569997234).Equals(target, 12));
        }

        /// <summary>
        /// A test for Clone
        /// </summary>
        [Test]
        public void CloneTest()
        {
            Vector target = new Vector(1.0, 2.0, 3.0);
            Vector expected = new Vector(1.0, 2.0, 3.0);
            Vector actual = null;
            actual = target.Clone();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for AngleTo
        /// </summary>
        [Test]
        public void AngleToTest()
        {
            Vector target = new Vector(1.0, 1.0, 1.0);
            Vector otherVector = new Vector(-1.0, -1.0, -1.0);
            Radian expected = Math.PI;
            Radian actual = default(Radian);
            actual = target.AngleTo(otherVector);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for AngleBetween
        /// </summary>
        [Test]
        public void AngleBetweenTest()
        {
            Vector target = new Vector(1.0, 1.0, 1.0);
            Vector otherVector = new Vector(-1.0, -1.0, -1.0);
            Radian expected = Math.PI;
            Radian actual = default(Radian);
            actual = target.AngleBetween(otherVector);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Vector Constructor
        /// </summary>
        [Test]
        public void VectorConstructorTest2()
        {
            MM i = 0.0;
            MM j = 0.0;
            MM k = 0.0;
            Vector target = new Vector();
            Assert.AreEqual(i, target.I);
            Assert.AreEqual(j, target.J);
            Assert.AreEqual(k, target.K);
        }

        /// <summary>
        /// A test for Vector Constructor
        /// </summary>
        [Test]
        public void VectorConstructorTest1()
        {
            MM i = 2.0;
            MM j = 3.0;
            MM k = 4.0;
            Vector target = new Vector(i, j, k);

            Assert.AreEqual(i, target.I);
            Assert.AreEqual(j, target.J);
            Assert.AreEqual(k, target.K);
        }

        /// <summary>
        /// A test for Vector Constructor
        /// </summary>
        [Test]
        public void VectorConstructorTest()
        {
            MM i = 2.0;
            MM j = 3.0;
            MM k = 4.0;
            MM[] componentArray =
            {
                i,
                j,
                k
            };
            Vector target = new Vector(componentArray);

            Assert.AreEqual(i, target.I);
            Assert.AreEqual(j, target.J);
            Assert.AreEqual(k, target.K);
        }

        #endregion
    }
}