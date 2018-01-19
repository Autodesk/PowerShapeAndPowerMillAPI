// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for ArcTest and is intended
    /// to contain all ArcTest Unit Tests
    /// </summary>
    [TestFixture]
    public abstract class WireframeTest<T> : EntityTest<T> where T : PSWireframe
    {
        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
        }

        #region Wireframe Tests

        #region Properties

        /// <summary>
        /// A test for Centre of Gravity
        /// </summary>
        public void CentreOfGravityTest(string wireframeFile, Point expectedCOG)
        {
            // Get entity
            var wireframe = (PSWireframe) ImportAndGetEntity(wireframeFile);

            // Check centre of gravity
            Assert.AreEqual(expectedCOG, wireframe.CentreOfGravity, "Centre of Gravity is incorrect");
        }

        /// <summary>
        /// A test for Centre of Gravity
        /// </summary>
        public void LengthTest(string wireframeFile, double expectedLength)
        {
            // Get entity
            var wireframe = (PSWireframe) ImportAndGetEntity(wireframeFile);

            // Check property returns correct value
            Assert.AreEqual(expectedLength, Math.Round(wireframe.Length.Value, 6), "Length is not as expected");
        }

        /// <summary>
        /// A test for Points
        /// </summary>
        public void PointsTest(string wireframeFile, int expectedNumberOfPoints)
        {
            // Get entity
            var wireframe = (PSWireframe) ImportAndGetEntity(wireframeFile);

            // Check number of points of entity
            Assert.AreEqual(expectedNumberOfPoints, wireframe.Points.Count, "Incorrect number of points in Points list");

            // Check property returns correct value
            Assert.AreEqual(wireframe.StartPoint, wireframe.Points[0], "Start Point is not in Points list");
            Assert.AreEqual(wireframe.EndPoint,
                            wireframe.Points[expectedNumberOfPoints - 1],
                            "End Point is not in Points list");
        }

        #endregion

        #region Operations

        #endregion

        #endregion
    }
}