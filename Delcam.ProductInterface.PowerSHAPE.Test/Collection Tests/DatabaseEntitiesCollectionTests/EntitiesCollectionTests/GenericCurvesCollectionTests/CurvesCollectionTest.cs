// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Linq;
using Autodesk.FileSystem;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for CurvesCollectionTest and is intended
    /// to contain all CurvesCollectionTest Unit Tests
    /// </summary>
    [TestFixture]
    public class CurvesCollectionTest : GenericCurvesCollectionTest<PSCurve>
    {
        #region Constructors

        public CurvesCollectionTest()
        {
            // Initialise PowerSHAPE
            if (_powerSHAPE == null)
            {
                _powerSHAPE = new PSAutomation(InstanceReuse.UseExistingInstance, Modes.PShapeMode);
                _powerSHAPE.DialogsOff();
            }
        }

        #endregion

        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
            try
            {
                _powerSHAPECollection = _powerSHAPE.ActiveModel.Curves;
                _genericCurvesCollection = (PSGenericCurvesCollection<PSCurve>) _powerSHAPECollection;
            }
            catch (Exception e)
            {
                Assert.Fail("_powerSHAPECollection could not be set to Active Model's CurvesCollection\n\n" + e.Message);
            }
        }

        #region GenericCurvesCollection Tests

        [Test]
        public void CreateCurveFromBreakTest()
        {
            base.CreateCurveFromBreakTest(TestFiles.SINGLE_CURVE);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// A test for CreateCurvesFromWrap
        /// </summary>
        private void CreateCurvesFromWrapToSurface(string wireframeToWrap, string surfaceToWrapTo)
        {
            // Get generic curves
            var curvesToWrap = ImportAndGetEntities(new File(wireframeToWrap)).Cast<PSGenericCurve>();

            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(new File(surfaceToWrapTo));

            // Wrap wireframe round surface
            var wrappedCurves = _powerSHAPE.ActiveModel.Curves.CreateCurvesFromWrap(surface, curvesToWrap, 10);

            // Check that wrap occurred
            Assert.AreNotEqual(0, wrappedCurves.Count, "No curves returned from operation");
            Assert.AreEqual(2, wrappedCurves.Count, "Incorrect number of curves returned from operation");
            Assert.AreEqual(83.599,
                            Math.Round(wrappedCurves[0].Length.Value + wrappedCurves[1].Length.Value, 3),
                            "Returned curves are incorrect geometry");
        }

        #endregion

        #region CurvesCollection Tests

        /// <summary>
        /// A test for CreateCurve
        /// </summary>
        [Test]
        public void CreateCurveFromPointsTest()
        {
            // Create bezier curve from points
            _powerSHAPE.ActiveModel.Curves.CreateCurveThroughPoints(CurveTypes.Bezier,
                                                                    new[]
                                                                    {
                                                                        new Point(10, 20, 0),
                                                                        new Point(20, 20, 0),
                                                                        new Point(30, 10, 0),
                                                                        new Point(30, 20, 0)
                                                                    });
            Assert.AreEqual(_powerSHAPE.ActiveModel.Curves.Count, 1, "Failed to add bezier curve to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.Curves[0].Type, CurveTypes.Bezier, "Created wrong type of curve");

            // Close model and open new model
            _powerSHAPE.Reset();

            // Create b-spline curve from points
            _powerSHAPE.ActiveModel.Curves.CreateCurveThroughPoints(CurveTypes.bSpline,
                                                                    new[]
                                                                    {
                                                                        new Point(10, 20, 0),
                                                                        new Point(20, 20, 0),
                                                                        new Point(30, 10, 0),
                                                                        new Point(30, 20, 0)
                                                                    });
            Assert.AreEqual(_powerSHAPE.ActiveModel.Curves.Count, 1, "Failed to add b-spline curve to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.Curves[0].Type, CurveTypes.bSpline, "Created wrong type of curve");

            // Close model and open new model
            _powerSHAPE.Reset();

            // Create g2 curve from points
            _powerSHAPE.ActiveModel.Curves.CreateCurveThroughPoints(CurveTypes.g2,
                                                                    new[]
                                                                    {
                                                                        new Point(10, 20, 0),
                                                                        new Point(20, 20, 0),
                                                                        new Point(30, 10, 0),
                                                                        new Point(30, 20, 0)
                                                                    });
            Assert.AreEqual(_powerSHAPE.ActiveModel.Curves.Count, 1, "Failed to add g2 curve to collection");
        }

        /// <summary>
        /// A test for CreateCurve
        /// </summary>
        [Test]
        public void CreateCurveFromCompCurveTest()
        {
            // Import CreateFromCompCurve dgk
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_COMPCURVE));

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(false);

            // Get compcurve
            var baseCompCurve = (PSCompCurve) _powerSHAPE.ActiveModel.SelectedItems[0];

            // Create curve from compcurve
            _powerSHAPE.ActiveModel.Curves.CreateCurveFromCompCurve(baseCompCurve);
            Assert.AreEqual(_powerSHAPE.ActiveModel.Curves.Count, 1, "Failed to add curve to collection");
        }

        /// <summary>
        /// A test for CreateCurvesFromWrap
        /// </summary>
        [Test]
        public void CreateCurvesFromWrapToSurfaceTest()
        {
            // Wrap curves onto surface
            CreateCurvesFromWrapToSurface(TestFiles.CURVES_TO_WRAP, TestFiles.SURFACE_TO_WRAP_ONTO);

            // Wrap CompCurves onto surface
            CreateCurvesFromWrapToSurface(TestFiles.COMPCURVES_TO_WRAP, TestFiles.SURFACE_TO_WRAP_ONTO);
        }

        #endregion

        #region EntitiesCollection Tests

        /// <summary>
        /// A test for RemoveAt
        /// </summary>
        [Test]
        public void RemoveCurveAtTest()
        {
            RemoveAtTest(TestFiles.THREE_CURVES, "Curve2");
        }

        /// <summary>
        /// A test for Remove
        /// </summary>
        [Test]
        public void RemoveCurveTest()
        {
            RemoveTest(TestFiles.THREE_CURVES, "Curve2");
        }

        /// <summary>
        /// A test for Clear
        /// </summary>
        [Test]
        public void ClearCurvesTest()
        {
            ClearTest(TestFiles.THREE_CURVES, TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for AddToSelection
        /// </summary>
        [Test]
        public void AddCurvesToSelectionTest()
        {
            AddToSelectionTest(TestFiles.THREE_CURVES, TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for RemoveToSelection
        /// </summary>
        [Test]
        public void RemoveCurvesFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.THREE_CURVES, TestFiles.SINGLE_SURFACE);
        }

        #endregion

        #region DatabaseEntitiesCollection Tests

        #region Properties

        /// <summary>
        /// A test for the Identifier
        /// </summary>
        [Test]
        public void IdentifierCurveTest()
        {
            IdentifierTest(new File(TestFiles.SINGLE_CURVE), "CURVE");
        }

        [Test]
        public void GetCurveByNameTest()
        {
            // Import file with multiple entities
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.THREE_CURVES));

            // Get an entity by its name
            var namedEntity = _powerSHAPECollection.GetByName("Curve2");

            // Check that the correct entity was returned
            Assert.AreEqual(7, namedEntity.NumberPoints, "Incorrect entity returned");
        }

        #endregion

        #region Operations

        [Test]
        public void ContainsCurveTest()
        {
            ContainsTest(new File(TestFiles.SINGLE_CURVE));
        }

        [Test]
        public void CountCurveTest()
        {
            CountTest(new File(TestFiles.SINGLE_CURVE));
        }

        [Test]
        public void EqualsCurveTest()
        {
            EqualsTest();
        }

        [Test]
        public void CurveItemTest()
        {
            ItemTest(new File(TestFiles.SINGLE_CURVE));
        }

        [Test]
        public void CurveLastItemTest()
        {
            LastItemTest(new File(TestFiles.THREE_CURVES));
        }

        #endregion

        #endregion
    }
}