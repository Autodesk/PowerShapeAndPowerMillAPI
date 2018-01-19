// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for LongitudinalTest and is intended
    /// to contain all LongitudinalTest Unit Tests
    /// </summary>
    [TestFixture]
    public class LongitudinalTest : SurfaceCurveTest<PSLongitudinal>
    {
        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();

            // Get list of laterals
            _surfaceCurves = _parentSurface.Longitudinals;
        }

        #region DatabaseEntity Tests

        /// <summary>
        /// A test for Id
        /// </summary>
        [Test]
        public void LongitudinalIdTest()
        {
            IdTest();
        }

        /// <summary>
        /// A test for Identifier
        /// </summary>
        [Test]
        public void LongitudinalIdentifierTest()
        {
            IdentifierTest("SURFACE['" + _parentSurface.Name + "'].LONGITUDINAL");
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void LongitudinalEqualsTest()
        {
            EqualsTest();
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        public void LongitudinalExistsTest()
        {
            ExistsTest();
        }

        #endregion

        #region Entity Tests

        [Test]
        public void BlankLongitudinalTest()
        {
            BlankTest(_parentSurface.Longitudinals[1]);
        }

        [Test]
        public void BlankExceptLongitudinalTest()
        {
            BlankExceptTest(_parentSurface.Longitudinals[1]);
        }

        [Test]
        public void LongitudinalBoundingBoxTest()
        {
            try
            {
                var bb = _parentSurface.Longitudinals[1].BoundingBox;
                Assert.Fail("Exception not thrown");
            }
            catch (Exception)
            {
                Assert.Pass();
            }
        }

        /*[Test]
        public void CopyLongitudinalTest()
        {
        }*/

        /*[Test]
        public void DeleteLongitudinalTest()
        {
            // Do operation
            base.DeleteTest();

            // Check there are correct number of Longitudinals on the surface
            Assert.AreEqual(2, _parentSurface.NumberOfLongitudinals, "Incorrect number of resultant Longitudinals on the surface");
        }*/

        /*[Test]
        public void LongitudinalLevelTest()
        {
        }

        [Test]
        public void LongitudinalLevelNumberTest()
        {
        }

        [Test]
        public void LimitLongitudinalWithEntitiesTest()
        {
        }

        [Test]
        public void MirrorLongitudinalTest()
        {
        }

        [Test]
        public void MoveLongitudinalTest()
        {
        }

        [Test]
        public void MoveLongitudinalCreatingCopiesTest()
        {
        }

        [Test]
        public void MoveLongitudinalDifferentOriginTest()
        {
        }

        [Test]
        public void LongitudinalNameTest()
        {
        }

        [Test]
        public void RemoveLongitudinalFromSelectionTest()
        {
        }

        [Test]
        public void LongitudinalRenameCommandTest()
        {
        }

        [Test]
        public void RotateLongitudinalTest()
        {
        }

        [Test]
        public void RotateLongitudinalCreatingCopiesTest()
        {
        }*/

        #endregion

        #region Wireframe Tests

        #region Properties

        /*/// <summary>
                /// A test for Centre of Gravity
                /// </summary>
                [Test]
                public void LongitudinalCentreOfGravityTest()
                {
                    base.CentreOfGravityTest(_parentSurface.Longitudinals[1], _parentSurface.Longitudinals[0],
                                             new Point((MM)(-69.335), (MM)42.741667, (MM)0.742857), new Point((MM)(-64), (MM)42.741667, (MM)0));
                }*/

        /*/// <summary>
        /// A test for Length
        /// </summary>
        [Test]
        public void LongitudinalLengthTest()
        {
        }

        /// <summary>
        ///A test for Points
        ///</summary>
        [Test]
        public void LongitudinalPointsTest()
        {
        }*/

        #endregion

        #endregion

        #region SurfaceCurve Tests

        /// <summary>
        /// A test for converting the Longitudinal to a composite curve
        /// </summary>
        [Test]
        public void CreateCompositeCurveTest()
        {
            base.CreateCompositeCurveTest();
        }

        #endregion

        #region Longitudinal Tests

        [Test]
        public void MoveSurfacePointsAlongVecTest()
        {
            MoveSurfacePointsAlongVectorTest();
        }

        #endregion
    }
}