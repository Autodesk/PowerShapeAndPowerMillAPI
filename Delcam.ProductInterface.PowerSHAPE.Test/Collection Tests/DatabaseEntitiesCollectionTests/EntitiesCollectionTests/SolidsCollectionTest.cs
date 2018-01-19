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
using Autodesk.FileSystem;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for SolidsCollectionTest and is intended
    /// to contain all SolidsCollectionTest Unit Tests
    /// </summary>
    [TestFixture]
    public class SolidsCollectionTest : EntitiesCollectionTest<PSSolid>
    {
        #region Constructors

        public SolidsCollectionTest()
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
                _powerSHAPECollection = _powerSHAPE.ActiveModel.Solids;
            }
            catch (Exception e)
            {
                Assert.Fail("_powerSHAPECollection could not be set to Active Model's SolidsCollection\n\n" + e.Message);
            }
        }

        #region SolidsCollection Tests

        /// <summary>
        /// A test for AddToSelection
        /// </summary>
        [Test]
        public void AddSolidsToSelectionTest()
        {
            AddToSelectionTest(TestFiles.THREE_SOLIDS, TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for CreateSolid
        /// </summary>
        [Test]
        public void CreateSolidFromSurfaceTestSingleSolid()
        {
            // Import surface
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_SURFACE));

            // Get surface
            var surfaceToSolid = _powerSHAPE.ActiveModel.SelectedItems[0] as PSSurface;
            var surfaceToConvert = new List<PSSurface>();
            surfaceToConvert.Add(surfaceToSolid);

            // Create solid
            var createdSolid = _powerSHAPE.ActiveModel.Solids.CreateSolidsFromSurfaces(surfaceToConvert)[0];

            // Check that solid has been created
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.Solids.Count, "Solid has not been added to collection");
        }

        [Test]
        public void CreateSolidsFromSurfaceTestMultipleSolids()
        {
            var activeModel = _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.CREATE_SOLIDS_FROM_SURFACES_MODEL));
            activeModel.GeneralTolerance = 0.001;

            PSSolid body = activeModel.Solids.GetByName("1");

            // Convert to collection of surfaces  
            List<PSSurface> surfaces = activeModel.Surfaces.CreateSurfacesFromSolid(body);

            // Copy End Caps to use in the solid construction ...
            PSSurface top = activeModel.Surfaces.GetByName("2");
            PSSurface bottom = activeModel.Surfaces.GetByName("3");

            // Add copies to collection
            surfaces.Add(top);
            surfaces.Add(bottom);

            var stockModel = activeModel.Solids.CreateSolidsFromSurfaces(surfaces)[0];

            Assert.AreEqual(2, activeModel.Solids.Count, "Wrong number of solids created");
        }

        /// <summary>
        /// A test for CreateSolid if the Solid will be version 8
        /// </summary>
        [Test]
        public void CreateSolid8FromSurfaceTest()
        {
            // Import surface
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_SURFACE));

            _powerSHAPE.Execute("TOOLS PREFERENCES", "SOLID V8 ON", "ACCEPT");

            // Get surface
            var surfaceToSolid = _powerSHAPE.ActiveModel.SelectedItems[0] as PSSurface;
            var surfaceToConvert = new List<PSSurface>();
            surfaceToConvert.Add(surfaceToSolid);

            // Create solid
            var createdSolid = _powerSHAPE.ActiveModel.Solids.CreateSolidsFromSurfaces(surfaceToConvert)[0];

            // Check that solid has been created
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.Solids.Count, "Solid has not been added to collection");

            // Check that surface has been deleted
            Assert.AreEqual(0, _powerSHAPE.ActiveModel.Surfaces.Count, "Surface has not been removed from collection");
        }

        /// <summary>
        /// A test for CreateSolid
        /// </summary>
        [Test]
        public void CreateSolidExtrusionFromArcTest()
        {
            // Import surface
            var circle = _powerSHAPE.ActiveModel.Arcs.CreateArcCircle(new Point(0, 0, 0), new Point(10, 0, 0), 20.0);

            // Create solid
            PSSolid createdSolid = _powerSHAPE.ActiveModel.Solids.CreateSolidExtrusionFromWireframe(circle, 50, 0);

            // Check that solid has been created
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.Solids.Count, "Solid has not been added to collection");
        }

        /// <summary>
        /// A test for CreateSolid
        /// </summary>
        [Test]
        public void CreateSolidExtrusionTest()
        {
            // Import surface
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SURFACE_FILLIN));

            // Get wireframe
            var wireframe = _powerSHAPE.ActiveModel.SelectedItems[0] as PSCurve;

            // Create solid
            PSSolid createdSolid = _powerSHAPE.ActiveModel.Solids.CreateSolidExtrusionFromWireframe(wireframe, 10, 10);

            // Check that solid has been created
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.Solids.Count, "Solid has not been added to collection");
        }

        [Test]
        public void CreateSolidFromDriveAndSectionsTest()
        {
            // Import curves
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.WIREFRAMES_FOR_DRIVE_SURFACE));

            // Get compcurve
            var driveCurve = _powerSHAPE.ActiveModel.CompCurves[0];

            // Get the sections
            var sections = new List<PSGenericCurve>(_powerSHAPE.ActiveModel.Curves);

            // Create solid
            var createdSolid = _powerSHAPE.ActiveModel.Solids.CreateSolidFromDriveAndSections(driveCurve, sections);

            // Check that solid has been created
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.Solids.Count, "Solid has not been added to collection");
        }

        [Test]
        public void CreateSolidCone()
        {
            // Create a cone
            var cone = _powerSHAPE.ActiveModel.Solids.CreateCone(new Point());

            // Check that solid has been created
            Assert.That(_powerSHAPE.ActiveModel.Solids.Count, Is.EqualTo(1));

            // Check that solid has the correct type
            Assert.That(cone.Type, Is.EqualTo(SolidTypes.Cone));
        }

        [Test]
        public void CreateSolidTorus()
        {
            // Create a Torus
            var torus = _powerSHAPE.ActiveModel.Solids.CreateTorus(new Point());

            // Check that solid has been created
            Assert.That(_powerSHAPE.ActiveModel.Solids.Count, Is.EqualTo(1));

            // Check that solid has the correct type
            Assert.That(torus.Type, Is.EqualTo(SolidTypes.Torus));
        }

        [Test]
        public void CreateSolidSpring()
        {
            // Create a spring
            var spring = _powerSHAPE.ActiveModel.Solids.CreateSpring(new Point());

            // Check that solid has been created
            Assert.That(_powerSHAPE.ActiveModel.Solids.Count, Is.EqualTo(1));

            // Check that solid has the correct type
            Assert.That(spring.Type, Is.EqualTo(SolidTypes.Spring));
        }

        #endregion

        #region EntitiesCollection Tests

        /// <summary>
        /// A test for RemoveAt
        /// </summary>
        [Test]
        public void RemoveSolidAtTest()
        {
            RemoveAtTest(TestFiles.THREE_SOLIDS, "Solid2");
        }

        /// <summary>
        /// A test for Remove
        /// </summary>
        [Test]
        public void RemoveSolidTest()
        {
            RemoveTest(TestFiles.THREE_SOLIDS, "Solid2");
        }

        /// <summary>
        /// A test for Clear
        /// </summary>
        [Test]
        public void ClearSolidsTest()
        {
            ClearTest(TestFiles.THREE_SOLIDS, TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for RemoveToSelection
        /// </summary>
        [Test]
        public void RemoveSolidsFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.THREE_SOLIDS, TestFiles.SINGLE_SURFACE);
        }

        #endregion

        #region DatabaseEntitiesCollection Tests

        #region Properties

        /// <summary>
        /// A test for the Identifier
        /// </summary>
        [Test]
        public void IdentifierSolidTest()
        {
            IdentifierTest(new File(TestFiles.SINGLE_SOLID), "SOLID");
        }

        [Test]
        public void GetSolidByNameTest()
        {
            // Import file with multiple entities
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.THREE_SOLIDS));

            // Get an entity by its name
            var namedEntity = _powerSHAPECollection.GetByName("Solid1");

            // Check that the correct entity was returned
            Assert.AreEqual(12.5, namedEntity.Volume, "Incorrect entity returned");
        }

        #endregion

        #region Operations

        [Test]
        public void ContainsSolidTest()
        {
            ContainsTest(new File(TestFiles.SINGLE_SOLID));
        }

        [Test]
        public void CountSolidTest()
        {
            CountTest(new File(TestFiles.SINGLE_SOLID));
        }

        [Test]
        public void EqualsSolidTest()
        {
            EqualsTest();
        }

        [Test]
        public void SolidItemTest()
        {
            ItemTest(new File(TestFiles.SINGLE_SOLID));
        }

        [Test]
        public void SolidLastItemTest()
        {
            LastItemTest(new File(TestFiles.THREE_SOLIDS));
        }

        #endregion

        #endregion
    }
}