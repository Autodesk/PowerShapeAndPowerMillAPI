// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Reflection;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for SurfaceCurveTest and is intended
    /// to contain all SurfaceCurveTest Unit Tests
    /// </summary>
    [TestFixture]
    public abstract class SurfaceCurveTest<T> : WireframeTest<T> where T : PSSurfaceCurve
    {
        #region Fields

        protected PSSurface _parentSurface;
        protected PSSurfaceCurvesCollection<T> _surfaceCurves;

        #endregion

        #region Additional test attributes

        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();

            // Import parent surface
            _parentSurface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE_WITH_BULGE);
        }

        #endregion

        #region DatabaseEntity Tests

        /// <summary>
        /// A test for Id
        /// </summary>
        public void IdTest()
        {
            // Get first surface curve
            PSSurfaceCurve surfaceCurve = _surfaceCurves[0];

            // Check that the id is correct
            Assert.AreEqual(_powerSHAPE.ExecuteEx(IdentifierAccessor(surfaceCurve) + "[" + surfaceCurve.Name + "].ID"),
                            surfaceCurve.Id,
                            "Returned incorrect id");
        }

        /// <summary>
        /// A test for Identifier
        /// </summary>
        public void IdentifierTest(string expectedIdentifier)
        {
            // Get first surface curve
            PSSurfaceCurve surfaceCurve = _surfaceCurves[0];

            // Check that the collection Identifier matches the entity type
            string actualIdentifier = (string) surfaceCurve
                .GetType().GetProperty("Identifier", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(surfaceCurve, null);
            Assert.AreEqual(expectedIdentifier, actualIdentifier);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        public void EqualsTest()
        {
            // Get first surface curve
            PSSurfaceCurve surfaceCurve = _surfaceCurves[0];

            // Compare collections
            Assert.IsTrue(_surfaceCurves[0].Equals(surfaceCurve), "Incorrectly returned that entities were not equal");
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        public void ExistsTest()
        {
            // Get first surface curve
            PSSurfaceCurve surfaceCurve = _surfaceCurves[0];

            // Check that correct value is returned
            Assert.IsTrue(surfaceCurve.Exists, "Incorrectly stated that entity did not exist");

            // Delete entity and check again
            surfaceCurve.Delete();
            Assert.IsFalse(surfaceCurve.Exists, "Incorrectly stated that entity existed");
        }

        #endregion

        #region Entity Tests

        public void BlankTest(PSSurfaceCurve curveToBlank)
        {
            // Import other entities
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(TestFiles.THREE_COMPCURVES));

            // Blank entity
            curveToBlank.Blank();

            // Check that entity has been blanked
            _powerSHAPE.ActiveModel.SelectAll(false);
            Assert.AreEqual(3, _powerSHAPE.ActiveModel.SelectedItems.Count, "Entity was not blanked");
        }

        public void BlankExceptTest(PSSurfaceCurve curveToBlankExcept)
        {
            // Import other entities
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(TestFiles.THREE_COMPCURVES));

            // Blank all except the surface curve
            curveToBlankExcept.BlankExcept();

            // Check that other entities have been blanked
            _powerSHAPE.ActiveModel.SelectAll(false);
            Assert.AreNotEqual(4, _powerSHAPE.ActiveModel.SelectedItems.Count, "Nothing was blanked");
            Assert.AreNotEqual(0, _powerSHAPE.ActiveModel.SelectedItems.Count, "Everything was blanked");
        }

        public void BoundingBoxTest(PSSurfaceCurve curveToCheck, Point minimumBounds, Point maximumBounds)
        {
            // Check bounding box
            Assert.AreEqual(minimumBounds,
                            curveToCheck.BoundingBox.MinimumBounds,
                            "Bounding box did not return the correct minimum bounds");
            Assert.AreEqual(maximumBounds,
                            curveToCheck.BoundingBox.MaximumBounds,
                            "Bounding box did not return the correct maximum bounds");
        }

        public void CopyTest()
        {
        }

        public void DeleteTest()
        {
            // Get second surface curve and its guid
            PSSurfaceCurve surfaceCurve = _surfaceCurves[1];
            Guid surfaceCurveGuid = InternalIdAccessor(surfaceCurve);

            // Delete surface curve
            surfaceCurve.Delete();

            // Check that the surface curve is no longer in the collection
            foreach (T collectionSurfaceCurve in _surfaceCurves)
            {
                Assert.IsFalse(InternalIdAccessor(collectionSurfaceCurve) == surfaceCurveGuid, "Lateral still exists");
            }

            // Check that the id of higher laterals has been decreased
            Assert.AreEqual("2", _surfaceCurves[1].Id, "Other IDs not changed correctly");

            // Check that the name of higher laterals has been decreased
            Assert.AreEqual("2", _surfaceCurves[1].Name, "Other names not changed correctly");
        }

        protected Guid InternalIdAccessor(PSSurfaceCurve surfaceCurve)
        {
            PropertyInfo propertyInfo =
                surfaceCurve.GetType().GetProperty("InternalId", BindingFlags.NonPublic | BindingFlags.Instance);
            return (Guid) propertyInfo.GetValue(surfaceCurve, null);
        }

        public void LevelTest()
        {
        }

        public void LevelNumberTest()
        {
        }

        public void LimitWithEntitiesTest()
        {
        }

        public void MirrorTest()
        {
        }

        public void MoveTest()
        {
        }

        public void MoveCreatingCopiesTest()
        {
        }

        public void MoveDifferentOriginTest()
        {
        }

        public void NameTest()
        {
        }

        public void RemoveFromSelectionTest()
        {
        }

        public void RenameCommandTest()
        {
        }

        public void RotateTest()
        {
        }

        public void RotateCreatingCopiesTest()
        {
        }

        #endregion

        #region Wireframe Tests

        #region Properties

        public void CentreOfGravityTest(
            PSSurfaceCurve curvedCurveToTest,
            PSSurfaceCurve straightCurveToTest,
            Point expectedCurvedCOG,
            Point expectedStraightCOG)
        {
            // Check centre of gravity on a curved curve
            Assert.AreEqual(expectedCurvedCOG,
                            curvedCurveToTest.CentreOfGravity,
                            "Centre of Gravity on curved curve is incorrect");

            // Check centre of gravity on a straight curve to see if PowerSHAPE bug has been fixed
            Assert.AreEqual(expectedStraightCOG,
                            straightCurveToTest.CentreOfGravity,
                            "Centre of Gravity on straight curve is incorrect");
        }

        #endregion

        #endregion

        #region SurfaceCurveTests

        #region Operations

        /// <summary>
        /// A test for converting the surface curve to a composite curve
        /// </summary>
        public void CreateCompositeCurveTest()
        {
            // Get first surface curve
            PSSurfaceCurve surfaceCurve = _surfaceCurves[0];

            // Convert it to a composite curve
            PSCompCurve compCurve = surfaceCurve.CreateCompositeCurve();

            // Check the entity exists and has the same length as the surface curve
            Assert.AreEqual(surfaceCurve.Length, compCurve.Length);
        }

        /// <summary>
        /// A test for moving surface points along a vector
        /// </summary>
        public void MoveSurfacePointsAlongVectorTest()
        {
            // Get first surface curve
            PSSurfaceCurve surfaceCurve = _surfaceCurves[0];

            // Create the vector for moving points along
            var vec = new Vector(10, 10, 10);

            // Create the array of points numbers
            var pointsOnSurface = new[] {0, 1, 2};

            // Select the points to move
            surfaceCurve.SelectCurvePoints(pointsOnSurface);

            var x = surfaceCurve.Points[0].X;
            var y = surfaceCurve.Points[0].Y;
            var z = surfaceCurve.Points[0].Z;

            // Move the points along a vector
            surfaceCurve.MoveSurfacePointsAlongVector(vec, pointsOnSurface);

            // Check the points have been moved correctly
            Assert.AreEqual(x + vec.I, surfaceCurve.Points[0].X);
            Assert.AreEqual(y + vec.J, surfaceCurve.Points[0].Y);
            Assert.AreEqual(z + vec.K, surfaceCurve.Points[0].Z);
        }

        #endregion

        #endregion
    }
}