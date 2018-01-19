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
    /// This is a test class for SurfacesCollectionTest and is intended
    /// to contain all SurfacesCollectionTest Unit Tests
    /// </summary>
    [TestFixture]
    public class SurfacesCollectionTest : EntitiesCollectionTest<PSSurface>
    {
        #region Constructors

        public SurfacesCollectionTest()
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
                _powerSHAPECollection = _powerSHAPE.ActiveModel.Surfaces;
            }
            catch (Exception e)
            {
                Assert.Fail("_powerSHAPECollection could not be set to Active Model's SurfacesCollection\n\n" +
                            e.Message);
            }
        }

        #region SurfacesCollection Tests

        /// <summary>
        /// A test for AddToSelection
        /// </summary>
        [Test]
        public void AddSurfacesToSelectionTest()
        {
            AddToSelectionTest(TestFiles.THREE_SURFACES, TestFiles.SINGLE_ARC);
        }

        /// <summary>
        /// A test for AppendTwoSurfaces
        /// </summary>
        [Test]
        [Ignore("Code has been moved to Surface - reimplement test there")]
        public void AppendTwoSurfacesTest()
        {
            //// Import two touching surfaces
            //_powerSHAPE.ActiveModel.Import(new Autodesk.FileSystem.File(TestFiles.TOUCHING_SURFACES));

            //// Append two surfaces
            //_powerSHAPE.ActiveModel.Surfaces.AppendTwoSurfaces(_powerSHAPE.ActiveModel.Surfaces[0], _powerSHAPE.ActiveModel.Surfaces[0].Longitudinals[0], _powerSHAPE.ActiveModel.Surfaces[1]);
            //Assert.AreEqual(_powerSHAPE.ActiveModel.Surfaces.Count, 1, "Failed to append surfaces");
        }

        /// <summary>
        /// A test for CreateFillet
        /// </summary>
        [Test]
        public void CreateSurfaceFilletTest()
        {
            // Import two touching surfaces
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.TOUCHING_SURFACES));
            var firstSurface = new List<PSSurface>();
            var secondSurface = new List<PSSurface>();
            firstSurface.Add(_powerSHAPE.ActiveModel.Surfaces[0]);
            secondSurface.Add(_powerSHAPE.ActiveModel.Surfaces[1]);

            // Fillet two surfaces
            _powerSHAPE.ActiveModel.Surfaces.CreateFilletSurfaces(firstSurface,
                                                                  secondSurface,
                                                                  3,
                                                                  FilletTypes.CONVEX,
                                                                  FilletTrimOptions.TRIMON,
                                                                  FilletRouteOptions.ALLON,
                                                                  FilletCreaseOptions.CREASEON,
                                                                  FilletCornerTypes.ROLL);
            Assert.AreEqual(3, _powerSHAPE.ActiveModel.Surfaces.Count, "Failed to fillet surfaces");
        }

        /// <summary>
        /// A test for CreateFillet based on feedback from Ash @ PCC
        /// </summary>
        [Test]
        public void CreateSurfaceFilletTestAsh()
        {
            // Import two touching surfaces
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.FILLET_SURFACES));
            var firstSurface = _powerSHAPE.ActiveModel.Surfaces[0];
            var secondSurface = _powerSHAPE.ActiveModel.Surfaces[1];

            // Fillet two surfaces
            _powerSHAPE.ActiveModel.Surfaces.CreateFilletSurfaces(firstSurface,
                                                                  secondSurface,
                                                                  63.12,
                                                                  FilletTypes.CONVEX,
                                                                  FilletTrimOptions.TRIMON,
                                                                  FilletRouteOptions.ALLON,
                                                                  FilletCreaseOptions.CREASEOFF,
                                                                  FilletCornerTypes.ROUND);
            Assert.AreEqual(3, _powerSHAPE.ActiveModel.Surfaces.Count, "Failed to fillet surfaces");
        }

        /// <summary>
        /// A test for CreateFillinSurface
        /// </summary>
        [Test]
        public void CreateFillInSurfaceTest()
        {
            // Import CreateFillIn dgk
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SURFACE_FILLIN));

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(false);

            // Get curve from selection
            var fillInCurve = _powerSHAPE.ActiveModel.SelectedItems[0] as PSCurve;

            // Create surface fill in
            _powerSHAPE.ActiveModel.Surfaces.CreateFillInSurface(fillInCurve);
            Assert.AreEqual(_powerSHAPE.ActiveModel.Surfaces.Count, 1, "Failed to add surface to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create surface in PowerSHAPE");
        }

        /// <summary>
        /// A test for CreateSurfacesFromSolid
        /// </summary>
        [Test]
        public void CreateSurfaceFromSolidTest()
        {
            // Import CreateFillIn dgk
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_SOLID));

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(false);

            // Get solid from selection
            var solidToConvert = _powerSHAPE.ActiveModel.SelectedItems[0] as PSSolid;

            // Create surfaces from solid
            _powerSHAPE.ActiveModel.Surfaces.CreateSurfacesFromSolid(solidToConvert);
            Assert.AreEqual(6,
                            _powerSHAPE.ActiveModel.Surfaces.Count,
                            "Failed to add " + (6 - _powerSHAPE.ActiveModel.Surfaces.Count) + " surfaces to collection");
        }

        /// <summary>
        /// A test for CreateSurface
        /// </summary>
        [Test]
        public void CreateSurfaceExtrusionTest()
        {
            // Import CreateFillIn dgk
            var firstFillinCurve = (PSCurve) ImportAndGetEntity(new File(TestFiles.SURFACE_FILLIN));

            // Create surface extrusion
            var firstExtrudedSurface =
                (PSSurfaceExtrusion) _powerSHAPE.ActiveModel.Surfaces.CreateExtrudedSurface(firstFillinCurve, 10, 20);
            Assert.AreEqual(_powerSHAPE.ActiveModel.Surfaces.Count, 1, "Failed to add surface to collection");
            Assert.IsTrue(firstExtrudedSurface.Length == 10, "Surface has incorrect length");
            Assert.IsTrue(firstExtrudedSurface.NegativeLength == 20, "Surface has incorrect negative length");

            // Import multiple curves
            var secondFillinCurves =
                ImportAndGetEntities(new File(TestFiles.SURFACE_FILLIN_MULTIPLE)).Cast<PSWireframe>();

            // Create surface extrusion
            var secondExtrudedSurfaces =
                _powerSHAPE.ActiveModel.Surfaces.CreateExtrudedSurfaces(secondFillinCurves, 20, 25)
                           .Cast<PSSurfaceExtrusion>()
                           .ToList();
            Assert.AreEqual(_powerSHAPE.ActiveModel.Surfaces.Count, 3, "Failed to add surface to collection");
            Assert.IsTrue(secondExtrudedSurfaces[0].Length == 20, "Surface has incorrect length");
            Assert.IsTrue(secondExtrudedSurfaces[1].NegativeLength == 25, "Surface has incorrect negative length");
        }

        /// <summary>
        /// A test for CreateSurface (from Network)
        /// </summary>
        [Test]
        public void CreateSurfaceFromNetworkTest()
        {
            // Import CreateFromNetwork dgk
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SURFACE_FROM_NETWORK));

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(false);

            // Create surface from network
            _powerSHAPE.ActiveModel.Surfaces.CreateSurface(AutomaticSurfacingMethods.FromNetwork,
                                                           AutomaticSurfaceTypes.PowerSurface);
            Assert.AreEqual(_powerSHAPE.ActiveModel.Surfaces.Count, 1, "Failed to add surface to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create surface in PowerSHAPE");

            _powerSHAPE.ActiveModel.Surfaces[0].Delete();

            _powerSHAPE.ActiveModel.SelectAll(false);

            _powerSHAPE.ActiveModel.Surfaces.CreateSurface(AutomaticSurfacingMethods.FromNetwork,
                                                           AutomaticSurfaceTypes.PowerSurface,
                                                           new SurfaceAdvancedOptions(
                                                               aInteriorInterPolation:
                                                               AdvancedInteriorInterpolationMethods.Linear)
            );

            Assert.AreEqual(_powerSHAPE.ActiveModel.Surfaces.Count, 1, "Failed to add surface to collection");

            //Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create surface in PowerSHAPE");
        }

        /// <summary>
        /// A test for CreateSurface (from Separate)s
        /// </summary>
        [Test]
        public void CreateSurfaceFromSeparateTest()
        {
            // Import CreateFromSeparate dgk
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SURFACE_FROM_SEPARATE));

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(false);

            // Create surface from separate
            _powerSHAPE.ActiveModel.Surfaces.CreateSurface(AutomaticSurfacingMethods.FromSeparate,
                                                           AutomaticSurfaceTypes.PowerSurface);
            Assert.AreEqual(_powerSHAPE.ActiveModel.Surfaces.Count, 1, "Failed to add surface to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create surface in PowerSHAPE");
        }

        /// <summary>
        /// A test for CreateSurface (Drive Curve)
        /// </summary>
        [Test]
        public void CreateSurfaceDriveCurveTest()
        {
            // Import CreateDriveCurve dgk
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SURFACE_DRIVECURVE));

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(false);

            // Create surface from drive curve
            _powerSHAPE.ActiveModel.Surfaces.CreateSurface(AutomaticSurfacingMethods.DriveCurve,
                                                           AutomaticSurfaceTypes.PowerSurface);
            Assert.AreEqual(_powerSHAPE.ActiveModel.Surfaces.Count, 1, "Failed to add surface to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create surface in PowerSHAPE");
        }

        /// <summary>
        /// A test for CreateSurface (Two Rails)
        /// </summary>
        [Test]
        public void CreateSurfaceTwoRailsTest()
        {
            // Import CreateTwoRails dgk
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SURFACE_TWO_RAILS));

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(false);

            // Create surface from two rails
            _powerSHAPE.ActiveModel.Surfaces.CreateSurface(AutomaticSurfacingMethods.TwoRails,
                                                           AutomaticSurfaceTypes.PowerSurface);
            Assert.AreEqual(_powerSHAPE.ActiveModel.Surfaces.Count, 1, "Failed to add surface to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create surface in PowerSHAPE");
        }

        /// <summary>
        /// A test for CreateSurface (Plane of Best-Fit)
        /// </summary>
        [Test]
        public void CreateSurfacePlaneOfBestFitTest()
        {
            // Import CreateFromPlaneOfBestFit dgk
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SURFACE_PLANE_OF_BEST_FIT));

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(false);

            // Create surface from plane of best fit
            _powerSHAPE.ActiveModel.Surfaces.CreateSurface(AutomaticSurfacingMethods.PlaneOfBestFit,
                                                           AutomaticSurfaceTypes.PowerSurface);
            Assert.AreEqual(_powerSHAPE.ActiveModel.Surfaces.Count, 1, "Failed to add surface to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create surface in PowerSHAPE");
        }

        /// <summary>
        /// A test for CreateSurface (From Triangles)
        /// </summary>
        [Test]
        public void CreateSurfaceFromTrianglesTest()
        {
            // Import CreateFromPlaneOfBestFit dgk
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SURFACE_FROM_TRIANGLES));

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(false);

            // Create surface from plane of best fit
            _powerSHAPE.ActiveModel.Surfaces.CreateSurface(AutomaticSurfacingMethods.FromTriangles,
                                                           AutomaticSurfaceTypes.PowerSurface);
            Assert.AreEqual(_powerSHAPE.ActiveModel.Surfaces.Count, 1, "Failed to add surface to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create surface in PowerSHAPE");
        }

        [Test]
        public void CreateSurfaceFromTrianglesAdvancedTest()
        {
            // Import CreateFromPlaneOfBestFit dgk
            var activeModel = _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SURFACE_FROM_TRIANGLES));

            // Get the mesh
            var mesh = activeModel.Meshes[0];

            // Create surface
            var surface = activeModel.Surfaces.CreateSurfaceFromTriangles(mesh);
            Assert.AreEqual((MM) 0.0, surface.BoundingBox.ZSize);
            Assert.AreEqual((MM) 86.741516, surface.BoundingBox.XSize);
            Assert.AreEqual(2797.3579490000002, surface.Area);
        }

        [Test]
        public void CreateSurfaceFromTrianglesAdvanced2Test()
        {
            // Import CreateFromPlaneOfBestFit dgk
            var activeModel = _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SURFACE_FROM_TRIANGLES));

            // Get the mesh
            var mesh = activeModel.Meshes[0];

            // Create surface
            var surface = activeModel.Surfaces.CreateSurfaceFromTriangles(mesh, null, 25.0, 44.1);
            Assert.AreEqual((MM) 0.0, surface.BoundingBox.ZSize);
            Assert.AreEqual((MM) 88.512372, surface.BoundingBox.XSize);
            Assert.AreEqual(3921.9330570000002, surface.Area);
        }

        [Test]
        public void CreateSurfaceFromTrianglesAdvancedIslandsTest()
        {
            // Import CreateFromPlaneOfBestFit dgk
            var activeModel = _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SURFACE_FROM_TRIANGLES));

            // Get the mesh
            var mesh = activeModel.Meshes[0];
            var curves = activeModel.CompCurves.Cast<PSGenericCurve>().ToList();

            // Create surface
            var surface = activeModel.Surfaces.CreateSurfaceFromTriangles(mesh, curves, 25.0, 44.1);
            Assert.AreEqual((MM) 0.0, surface.BoundingBox.ZSize);
            Assert.AreEqual((MM) 70.004663, surface.BoundingBox.XSize);
            Assert.AreEqual(3487.9011620000001, surface.Area);
        }

        [Test]
        public void CreateSurfaceCone()
        {
            // Create a cone
            var cone = _powerSHAPE.ActiveModel.Surfaces.CreateCone(new Point());

            // Check that surface has been created
            Assert.That(_powerSHAPE.ActiveModel.Surfaces.Count, Is.EqualTo(1));

            // Check that surface has the correct type
            Assert.That(cone.Type, Is.EqualTo(SurfaceTypes.Cone));
        }

        [Test]
        public void CreateSurfaceTorus()
        {
            // Create a Torus
            var torus = _powerSHAPE.ActiveModel.Surfaces.CreateTorus(new Point());

            // Check that surface has been created
            Assert.That(_powerSHAPE.ActiveModel.Surfaces.Count, Is.EqualTo(1));

            // Check that surface has the correct type
            Assert.That(torus.Type, Is.EqualTo(SurfaceTypes.Torus));
        }

        [Test]
        public void CreateSurfaceSpring()
        {
            // Create a spring
            var spring = _powerSHAPE.ActiveModel.Surfaces.CreateSpring(new Point());

            // Check that surface has been created
            Assert.That(_powerSHAPE.ActiveModel.Surfaces.Count, Is.EqualTo(1));

            // Check that surface has the correct type
            Assert.That(spring.Type, Is.EqualTo(SurfaceTypes.Spring));
        }

        #endregion

        #region EntitiesCollection Tests

        /// <summary>
        /// A test for RemoveAt
        /// </summary>
        [Test]
        public void RemoveSurfaceAtTest()
        {
            RemoveAtTest(TestFiles.THREE_SURFACES, "Surface2");
        }

        /// <summary>
        /// A test for Remove
        /// </summary>
        [Test]
        public void RemoveSurfaceTest()
        {
            RemoveTest(TestFiles.THREE_SURFACES, "Surface2");
        }

        /// <summary>
        /// A test for Clear
        /// </summary>
        [Test]
        public void ClearSurfacesTest()
        {
            ClearTest(TestFiles.THREE_SURFACES, TestFiles.SINGLE_ARC);
        }

        /// <summary>
        /// A test for RemoveToSelection
        /// </summary>
        [Test]
        public void RemoveSurfacesFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.THREE_SURFACES, TestFiles.SINGLE_ARC);
        }

        #endregion

        #region DatabaseEntitiesCollection Tests

        #region Properties

        /// <summary>
        /// A test for the Identifier
        /// </summary>
        [Test]
        public void IdentifierSurfaceTest()
        {
            IdentifierTest(new File(TestFiles.SINGLE_SURFACE), "SURFACE");
        }

        [Test]
        public void GetSurfaceByNameTest()
        {
            // Import file with multiple entities
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.THREE_SURFACES));

            // Get an entity by its name
            var namedEntity = _powerSHAPECollection.GetByName("Surface2");

            // Check that the correct entity was returned
            Assert.AreEqual(SurfaceTypes.Plane, namedEntity.Type, "Incorrect entity returned");
        }

        #endregion

        #region Operations

        [Test]
        public void ContainsSurfaceTest()
        {
            ContainsTest(new File(TestFiles.SINGLE_SURFACE));
        }

        [Test]
        public void CountSurfaceTest()
        {
            CountTest(new File(TestFiles.SINGLE_SURFACE));
        }

        [Test]
        public void EqualsSurfaceTest()
        {
            EqualsTest();
        }

        [Test]
        public void SurfaceItemTest()
        {
            ItemTest(new File(TestFiles.SINGLE_SURFACE));
        }

        [Test]
        public void SurfaceLastItemTest()
        {
            LastItemTest(new File(TestFiles.THREE_SURFACES));
        }

        #endregion

        #endregion
    }
}