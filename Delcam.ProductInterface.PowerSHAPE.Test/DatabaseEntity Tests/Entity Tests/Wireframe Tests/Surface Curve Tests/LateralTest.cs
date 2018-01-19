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
    /// This is a test class for LateralTest and is intended
    /// to contain all LateralTest Unit Tests
    /// </summary>
    [TestFixture]
    public class LateralTest : SurfaceCurveTest<PSLateral>
    {
        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();

            // Get list of laterals
            _surfaceCurves = _parentSurface.Laterals;
        }

        #region DatabaseEntity Tests

        /// <summary>
        /// A test for Id
        /// </summary>
        [Test]
        public void LateralIdTest()
        {
            IdTest();
        }

        /// <summary>
        /// A test for Identifier
        /// </summary>
        [Test]
        public void LateralIdentifierTest()
        {
            IdentifierTest("SURFACE['" + _parentSurface.Name + "'].LATERAL");
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void LateralEqualsTest()
        {
            EqualsTest();
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        public void LateralExistsTest()
        {
            ExistsTest();
        }

        #endregion

        #region Entity Tests

        [Test]
        public void BlankLateralTest()
        {
            BlankTest(_parentSurface.Laterals[1]);
        }

        [Test]
        public void BlankExceptLateralTest()
        {
            BlankExceptTest(_parentSurface.Laterals[1]);
        }

        [Test]
        public void LateralBoundingBoxTest()
        {
            try
            {
                var bb = _parentSurface.Laterals[1].BoundingBox;
                Assert.Fail("Exception not thrown");
            }
            catch (Exception)
            {
                Assert.Pass();
            }
        }

        /*[Test]
        public void CopyLateralTest()
        {
        }*/

        /*[Test]
        public void DeleteLateralTest()
        {
            // Do operation
            base.DeleteTest();

            // Check there are correct number of laterals on the surface
            Assert.AreEqual(2, _parentSurface.NumberOfLaterals, "Incorrect number of resultant laterals on the surface");
        }*/

        /*[Test]
        public void LateralLevelTest()
        {
        }

        [Test]
        public void LateralLevelNumberTest()
        {
        }

        [Test]
        public void LimitLateralWithEntitiesTest()
        {
        }

        [Test]
        public void MirrorLateralTest()
        {
        }

        [Test]
        public void MoveLateralTest()
        {
        }

        [Test]
        public void MoveLateralCreatingCopiesTest()
        {
        }

        [Test]
        public void MoveLateralDifferentOriginTest()
        {
        }

        [Test]
        public void LateralNameTest()
        {
        }

        [Test]
        public void RemoveLateralFromSelectionTest()
        {
        }

        [Test]
        public void LateralRenameCommandTest()
        {
        }

        [Test]
        public void RotateLateralTest()
        {
        }

        [Test]
        public void RotateLateralCreatingCopiesTest()
        {
        }*/

        #endregion

        #region Wireframe Tests

        #region Properties

        /*/// <summary>
        /// A test for Centre of Gravity
        /// </summary>
        [Test]
        public void LateralCentreOfGravityTest()
        {
            base.CentreOfGravityTest(_parentSurface.Laterals[1], _parentSurface.Laterals[0],
                                     new Point((MM)(-69.335), (MM)42.741667, (MM)0.742857), new Point((MM)(-64), (MM)42.741667, (MM)0));
        }*/

        /*/// <summary>
        /// A test for Length
        /// </summary>
        [Test]
        public void LateralLengthTest()
        {
        }

        /// <summary>
        ///A test for Points
        ///</summary>
        [Test]
        public void LateralPointsTest()
        {
        }*/

        #endregion

        #endregion

        #region SurfaceCurve Tests

        /// <summary>
        /// A test for converting the lateral to a composite curve
        /// </summary>
        [Test]
        public void CreateCompositeCurveTest()
        {
            base.CreateCompositeCurveTest();
        }

        #endregion

        #region Lateral Tests

        [Test]
        public void MoveSurfacePointsAlongVecTest()
        {
            MoveSurfacePointsAlongVectorTest();
        }

        #endregion
    }
}