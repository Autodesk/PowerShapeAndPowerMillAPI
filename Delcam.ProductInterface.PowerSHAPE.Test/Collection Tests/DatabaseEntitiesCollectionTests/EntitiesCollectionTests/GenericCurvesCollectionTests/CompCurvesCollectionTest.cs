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
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for CompCurvesCollectionTest and is intended
    /// to contain all CompCurvesCollectionTest Unit Tests
    /// </summary>
    [TestFixture]
    public class CompCurvesCollectionTest : GenericCurvesCollectionTest<PSCompCurve>
    {
        #region Constructors

        public CompCurvesCollectionTest()
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
                _powerSHAPECollection = _powerSHAPE.ActiveModel.CompCurves;
                _genericCurvesCollection = (PSGenericCurvesCollection<PSCompCurve>) _powerSHAPECollection;
            }
            catch (Exception e)
            {
                Assert.Fail("_powerSHAPECollection could not be set to Active Model's CompCurvesCollection\n\n" +
                            e.Message);
            }
        }

        #region GenericCurvesCollection Tests

        [Test]
        public void CreateCompCurveFromBreakTest()
        {
            CreateCurveFromBreakTest(TestFiles.SINGLE_COMPCURVE);
        }

        [Test]
        public void CreateCurvesFromPumpkin()
        {
            var activeModel = _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.PUMPKIN_MODEL));
            var surface = activeModel.Surfaces.Last();
            var curves = activeModel.CompCurves.ToList<PSGenericCurve>();
            var result = activeModel.Curves.CreateCurvesFromWrap(surface, curves, 90.0);
            Assert.That(result.Count, Is.EqualTo(20));
            Assert.That(result[0].BoundingBox.MinZ, Is.EqualTo((MM) (-11.850173)));
        }

        #endregion

        #region CompCurvesCollection Tests

        [Test]
        public void CreateObliqueCurveTest()
        {
            try
            {
                var foot = _powerSHAPE.ActiveModel.Meshes.CreateMeshFromFile(new File(TestFiles.SINGLE_OBLIQUE_MESH1));
                _powerSHAPE.SetActivePlane(Planes.XY);

                var listCompCurves = _powerSHAPE.ActiveModel.CompCurves.CreateObliqueCurve(foot, Planes.XY, 45.0);

                if (listCompCurves.Count != 1)
                {
                    Assert.Fail("Should be one comp curve");
                }
                else
                {
                    if (_powerSHAPE.Version.Major < 16)

                        // TODO: update the old version numbers of points. 
                    {
                        Assert.AreEqual(44, listCompCurves[0].NumberPoints);
                    }
                    else
                    {
                        Assert.AreEqual(32, listCompCurves[0].NumberPoints);
                    }
                    var bx = listCompCurves[0].BoundingBox;
                    if (bx.MinX != 1025.006342)
                    {
                        Assert.Fail("Min X not equal: MinX={0}", bx.MinX);
                    }
                    if (bx.MaxX != 1824.995724)
                    {
                        Assert.Fail("Max X not equal: MaxX={0}", bx.MaxX);
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("CreateObliqueCurveTest\n\n" + ex.Message);
            }
        }

        /// <summary>
        /// A test for CreateCompCurve from points
        /// </summary>
        [Test]
        public void CreateCompCurveFromPointsTest()
        {
            // Create CompCurve from points
            _powerSHAPE.ActiveModel.CompCurves.CreateCompCurveThroughPoints(CurveTypes.Bezier,
                                                                            new[]
                                                                            {
                                                                                new Point(10, 20, 0),
                                                                                new Point(20, 20, 0),
                                                                                new Point(30, 10, 0),
                                                                                new Point(30, 20, 0)
                                                                            });
            Assert.AreEqual(_powerSHAPE.ActiveModel.CompCurves.Count, 1, "Failed to add compcurve to collection");
        }

        /// <summary>
        /// A test for CreateCompCurve from compcurve
        /// </summary>
        [Test]
        public void CreateCompCurveFromArcTest()
        {
            // Import arc from dgk
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_ARC));

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(false);

            // Get arc
            var baseArc = (PSArc) _powerSHAPE.ActiveModel.SelectedItems[0];

            // Create compcurve from arc
            _powerSHAPE.ActiveModel.CompCurves.CreateCompCurveFromWireframe(new PSWireframe[] {baseArc});
            Assert.AreEqual(_powerSHAPE.ActiveModel.CompCurves.Count, 1, "Failed to add compcurve to collection");

            //Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create compcurve in PowerSHAPE");
        }

        /// <summary>
        /// A test for CreateCompCurve from curve
        /// </summary>
        [Test]
        public void CreateCompCurveFromCurveTest()
        {
            // Import curve from dgk
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_CURVE));

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(false);

            // Get curve
            var baseCurve = (PSCurve) _powerSHAPE.ActiveModel.SelectedItems[0];

            // Create compcurve from curve
            _powerSHAPE.ActiveModel.CompCurves.CreateCompCurveFromWireframe(new PSWireframe[] {baseCurve});
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.CompCurves.Count, "Failed to add compcurve to collection");

            //Assert.AreEqual(1, (int)_powerSHAPE.DoCommandEx("MODEL.COMPCURVES"), "Failed to create compcurve in PowerSHAPE");
        }

        /// <summary>
        /// A test for CreateCompCurve from line
        /// </summary>
        [Test]
        public void CreateCompCurveFromLineTest()
        {
            // Import line from dgk
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_LINE));

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(false);

            // Get line
            var baseLine = (PSLine) _powerSHAPE.ActiveModel.SelectedItems[0];

            // Create compcurve from line
            _powerSHAPE.ActiveModel.CompCurves.CreateCompCurveFromWireframe(new PSWireframe[] {baseLine});
            Assert.AreEqual(_powerSHAPE.ActiveModel.CompCurves.Count, 1, "Failed to add compcurve to collection");

            //Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create compcurve in PowerSHAPE");
        }

        /// <summary>
        /// A test for CreateIntersectionCurves
        /// </summary>
        [Test]
        public void CreateIntersectionCurvesTest()
        {
            #region Create From Two Surfaces

            // Import two surfaces
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.TWO_SURFACES_FOR_INTERSECTION));

            // Get surfaces
            var firstSurfaces = new List<PSSurface>();
            var secondSurfaces = new List<PSSurface>();
            firstSurfaces.Add(_powerSHAPE.ActiveModel.Surfaces.GetByName("FirstSurface"));
            secondSurfaces.Add(_powerSHAPE.ActiveModel.Surfaces.GetByName("SecondSurface"));

            // Create compcurve from intersection
            _powerSHAPE.ActiveModel.CompCurves.CreateCompCurvesFromIntersectionOfTwoSetsOfSurfaces(firstSurfaces,
                                                                                                   secondSurfaces);
            Assert.AreEqual(_powerSHAPE.ActiveModel.CompCurves.Count, 1, "Failed to add compcurve to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create compcurve in PowerSHAPE");

            #endregion

            // Close PowerSHAPE model and create new
            _powerSHAPE.Reset();

            #region Create From Multiple Surfaces

            // Import multiple surfaces
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.MULT_SURFACES_FOR_INTERSECTION));

            // Get surfaces (which were added to the Model's 'Surfaces' collection in the Import method)
            firstSurfaces.Clear();
            secondSurfaces.Clear();
            for (var i = 1; i <= 3; i++)
            {
                firstSurfaces.Add(_powerSHAPE.ActiveModel.Surfaces.GetByName("FirstSurfaces" + i));
                secondSurfaces.Add(_powerSHAPE.ActiveModel.Surfaces.GetByName("SecondSurfaces" + i));
            }

            // Create compcurve from intersection
            _powerSHAPE.ActiveModel.CompCurves.CreateCompCurvesFromIntersectionOfTwoSetsOfSurfaces(firstSurfaces,
                                                                                                   secondSurfaces);
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.CompCurves.Count, "Failed to add compcurve to collection");

            //Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create compcurve in PowerSHAPE");

            #endregion
        }

        [Test]
        public void CreateIntersectionFailureTest()
        {
            // Import two surfaces
            var activeModel = _powerSHAPE.ActiveModel;
            activeModel.Import(new File(TestFiles.THREE_SURFACES));

            var surface1 = activeModel.Surfaces[0];
            var surface2 = activeModel.Surfaces[1];

            var intersections = activeModel.CompCurves.CreateCompCurvesFromIntersectionOfTwoSurfaces(surface1, surface2);

            Assert.IsEmpty(intersections);
        }

        /// <summary>
        /// Test the CreateCompCurvesFromAnnotationTest
        /// </summary>
        [Test]
        public void CreateCompCurvesFromAnnotationTest()
        {
            // Create annotation
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_ANNOTATION));

            var compCurves =
                _powerSHAPE.ActiveModel.CompCurves.CreateCompCurvesFromAnnotation(_powerSHAPE.ActiveModel.Annotations[0]);

            Assert.AreEqual(21, compCurves.Count);
        }

        /// <summary>
        /// Test the CreateCompCurvesFromAnnotationTest
        /// </summary>
        [Test]
        public void CreateCompCurvesFromAnnotationsTest()
        {
            // Create annotation
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.TWO_ANNOTATIONS));

            var compCurves =
                _powerSHAPE.ActiveModel.CompCurves.CreateCompCurvesFromAnnotations(
                    _powerSHAPE.ActiveModel.Annotations.ToList());

            Assert.AreEqual(42, compCurves.Count);
        }

        #endregion

        #region EntitiesCollection Tests

        /// <summary>
        /// A test for RemoveAt
        /// </summary>
        [Test]
        public void RemoveCompCurveAtTest()
        {
            RemoveAtTest(TestFiles.THREE_COMPCURVES, "CompCurve2");
        }

        /// <summary>
        /// A test for Remove
        /// </summary>
        [Test]
        public void RemoveCompCurveTest()
        {
            RemoveTest(TestFiles.THREE_COMPCURVES, "CompCurve2");
        }

        /// <summary>
        /// A test for Clear
        /// </summary>
        [Test]
        public void ClearCompCurvesTest()
        {
            ClearTest(TestFiles.THREE_COMPCURVES, TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for AddToSelection
        /// </summary>
        [Test]
        public void AddCompCurvesToSelectionTest()
        {
            AddToSelectionTest(TestFiles.THREE_COMPCURVES, TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for RemoveToSelection
        /// </summary>
        [Test]
        public void RemoveCompCurvesFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.THREE_COMPCURVES, TestFiles.SINGLE_SURFACE);
        }

        #endregion

        #region DatabaseEntitiesCollection Tests

        #region Properties

        /// <summary>
        /// A test for the Identifier
        /// </summary>
        [Test]
        public void IdentifierCompCurveTest()
        {
            IdentifierTest(new File(TestFiles.SINGLE_COMPCURVE), "COMPCURVE");
        }

        [Test]
        public void GetCompCurveByNameTest()
        {
            // Import file with multiple entities
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.THREE_COMPCURVES));

            // Get an entity by its name
            var namedEntity = _powerSHAPECollection.GetByName("CompCurve2");

            // Check that the correct entity was returned
            Assert.AreEqual(7, namedEntity.NumberPoints, "Incorrect entity returned");
        }

        #endregion

        #region Operations

        [Test]
        public void ContainsCompCurveTest()
        {
            ContainsTest(new File(TestFiles.SINGLE_COMPCURVE));
        }

        [Test]
        public void CountCompCurveTest()
        {
            CountTest(new File(TestFiles.SINGLE_COMPCURVE));
        }

        [Test]
        public void EqualsCompCurveTest()
        {
            EqualsTest();
        }

        [Test]
        public void CompCurveItemTest()
        {
            ItemTest(new File(TestFiles.SINGLE_COMPCURVE));
        }

        [Test]
        public void CompCurveLastItemTest()
        {
            LastItemTest(new File(TestFiles.THREE_COMPCURVES));
        }

        #endregion

        #endregion
    }
}