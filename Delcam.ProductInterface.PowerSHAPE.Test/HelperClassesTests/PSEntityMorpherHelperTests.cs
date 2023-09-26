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

namespace Autodesk.ProductInterface.PowerSHAPETest.HelperClassesTests
{
    [TestFixture]
    public class PSEntityMorpherHelperTests
    {
        private PSAutomation _powerShape;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _powerShape = new PSAutomation(InstanceReuse.UseExistingInstance);
            _powerShape.DialogsOff();
        }

        [SetUp]
        public void SetUp()
        {
            _powerShape.Reset();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            try
            {
                _powerShape.Reset();
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public void TwoSurfaceMorphTest()
        {
            var activeModel = _powerShape.ActiveModel;
            activeModel.Import(new File(TestFiles.TWO_SURFACE_MORPH));

            var surfacesToMorph = new List<PSSurface>();
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("1"));
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("2"));

            var referenceSurface = activeModel.Surfaces.GetByName("3");
            var controlSurface = activeModel.Surfaces.GetByName("4");

            PSEntityMorpher.MorphSurfacesBetweenTwoSurfaces(surfacesToMorph,
                                                            referenceSurface,
                                                            controlSurface,
                                                            DecayDefinitionOptions.None);

            Assert.IsFalse(referenceSurface.Exists);
            Assert.IsFalse(controlSurface.Exists);
            Assert.AreEqual((MM) 14.588907, surfacesToMorph[0].BoundingBox.ZSize);
        }

        [Test]
        public void TwoSurfaceMorphSolidsTest()
        {
            var activeModel = _powerShape.ActiveModel;
            activeModel.Import(new File(TestFiles.TWO_SURFACE_MORPH_SOLIDS));

            var solidToMorph = activeModel.Solids.GetByName("1");

            var referenceSurface = activeModel.Surfaces.GetByName("3");
            var controlSurface = activeModel.Surfaces.GetByName("4");

            PSEntityMorpher.MorphSolidBetweenTwoSurfaces(solidToMorph,
                                                         referenceSurface,
                                                         controlSurface);

            Assert.AreEqual((MM)14.588907, solidToMorph.BoundingBox.ZSize);
        }

        [Test]
        public void TwoSurfaceMorphTestKeepOn()
        {
            var activeModel = _powerShape.ActiveModel;
            activeModel.Import(new File(TestFiles.TWO_SURFACE_MORPH));

            var surfacesToMorph = new List<PSSurface>();
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("1"));
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("2"));

            var referenceSurface = activeModel.Surfaces.GetByName("3");
            var controlSurface = activeModel.Surfaces.GetByName("4");

            PSEntityMorpher.MorphSurfacesBetweenTwoSurfaces(surfacesToMorph,
                                                            referenceSurface,
                                                            controlSurface,
                                                            DecayDefinitionOptions.None,
                                                            DecayDefinitionBlend.Quartic,
                                                            true);

            Assert.IsTrue(referenceSurface.Exists);
            Assert.IsTrue(controlSurface.Exists);
            Assert.AreEqual((MM) 14.588907, surfacesToMorph[0].BoundingBox.ZSize);
        }

        [Test]
        public void TwoSurfaceMorphTestSurfaceQuartic()
        {
            var activeModel = _powerShape.ActiveModel;
            activeModel.Import(new File(TestFiles.TWO_SURFACE_MORPH));

            var surfacesToMorph = new List<PSSurface>();
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("1"));
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("2"));

            var referenceSurface = activeModel.Surfaces.GetByName("3");
            var controlSurface = activeModel.Surfaces.GetByName("4");
            var decaySurface = activeModel.Surfaces.GetByName("5");

            PSEntityMorpher.MorphSurfacesBetweenTwoSurfaces(surfacesToMorph,
                                                            referenceSurface,
                                                            controlSurface,
                                                            DecayDefinitionOptions.Surface,
                                                            DecayDefinitionBlend.Quartic,
                                                            false,
                                                            false,
                                                            decaySurface);

            Assert.IsFalse(referenceSurface.Exists);
            Assert.IsFalse(controlSurface.Exists);
            Assert.AreEqual((MM) 14.588906999999999, surfacesToMorph[0].BoundingBox.ZSize);
        }

        [Test]
        public void TwoSurfaceMorphTestSurfaceLinear()
        {
            var activeModel = _powerShape.ActiveModel;
            activeModel.Import(new File(TestFiles.TWO_SURFACE_MORPH));

            var surfacesToMorph = new List<PSSurface>();
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("1"));
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("2"));

            var referenceSurface = activeModel.Surfaces.GetByName("3");
            var controlSurface = activeModel.Surfaces.GetByName("4");
            var decaySurface = activeModel.Surfaces.GetByName("5");

            PSEntityMorpher.MorphSurfacesBetweenTwoSurfaces(surfacesToMorph,
                                                            referenceSurface,
                                                            controlSurface,
                                                            DecayDefinitionOptions.Surface,
                                                            DecayDefinitionBlend.Linear,
                                                            false,
                                                            false,
                                                            decaySurface);

            Assert.IsFalse(referenceSurface.Exists);
            Assert.IsFalse(controlSurface.Exists);
            Assert.AreEqual((MM) 14.588906999999999, surfacesToMorph[0].BoundingBox.ZSize);
        }

        [Test]
        public void TwoSurfaceMorphTestSurfaceParabolic()
        {
            var activeModel = _powerShape.ActiveModel;
            activeModel.Import(new File(TestFiles.TWO_SURFACE_MORPH));

            var surfacesToMorph = new List<PSSurface>();
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("1"));
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("2"));

            var referenceSurface = activeModel.Surfaces.GetByName("3");
            var controlSurface = activeModel.Surfaces.GetByName("4");
            var decaySurface = activeModel.Surfaces.GetByName("5");

            PSEntityMorpher.MorphSurfacesBetweenTwoSurfaces(surfacesToMorph,
                                                            referenceSurface,
                                                            controlSurface,
                                                            DecayDefinitionOptions.Surface,
                                                            DecayDefinitionBlend.Parabolic,
                                                            false,
                                                            false,
                                                            decaySurface);

            Assert.IsFalse(referenceSurface.Exists);
            Assert.IsFalse(controlSurface.Exists);
            Assert.AreEqual((MM) 14.588906999999999, surfacesToMorph[0].BoundingBox.ZSize);
        }

        [Test]
        public void TwoSurfaceMorphTestSurfaceCubic()
        {
            var activeModel = _powerShape.ActiveModel;
            activeModel.Import(new File(TestFiles.TWO_SURFACE_MORPH));

            var surfacesToMorph = new List<PSSurface>();
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("1"));
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("2"));

            var referenceSurface = activeModel.Surfaces.GetByName("3");
            var controlSurface = activeModel.Surfaces.GetByName("4");
            var decaySurface = activeModel.Surfaces.GetByName("5");

            PSEntityMorpher.MorphSurfacesBetweenTwoSurfaces(surfacesToMorph,
                                                            referenceSurface,
                                                            controlSurface,
                                                            DecayDefinitionOptions.Surface,
                                                            DecayDefinitionBlend.Cubic,
                                                            false,
                                                            false,
                                                            decaySurface);

            Assert.IsFalse(referenceSurface.Exists);
            Assert.IsFalse(controlSurface.Exists);
            Assert.AreEqual((MM) 14.588906999999999, surfacesToMorph[0].BoundingBox.ZSize);
        }

        [Test]
        public void TwoSurfaceMorphTestCurve()
        {
            var activeModel = _powerShape.ActiveModel;
            activeModel.Import(new File(TestFiles.TWO_SURFACE_MORPH));

            var surfacesToMorph = new List<PSSurface>();
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("1"));
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("2"));

            var referenceSurface = activeModel.Surfaces.GetByName("3");
            var controlSurface = activeModel.Surfaces.GetByName("4");
            var decayCurve = activeModel.CompCurves.GetByName("1");

            PSEntityMorpher.MorphSurfacesBetweenTwoSurfaces(surfacesToMorph,
                                                            referenceSurface,
                                                            controlSurface,
                                                            DecayDefinitionOptions.Curve,
                                                            DecayDefinitionBlend.Cubic,
                                                            false,
                                                            false,
                                                            null,
                                                            decayCurve);

            Assert.IsFalse(referenceSurface.Exists);
            Assert.IsFalse(controlSurface.Exists);
            Assert.AreEqual((MM) 14.588753, surfacesToMorph[0].BoundingBox.ZSize);
        }

        [Test]
        public void TwoSurfaceMorphTestDistance()
        {
            var activeModel = _powerShape.ActiveModel;
            activeModel.Import(new File(TestFiles.TWO_SURFACE_MORPH));

            var surfacesToMorph = new List<PSSurface>();
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("1"));
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("2"));

            var referenceSurface = activeModel.Surfaces.GetByName("3");
            var controlSurface = activeModel.Surfaces.GetByName("4");

            PSEntityMorpher.MorphSurfacesBetweenTwoSurfaces(surfacesToMorph,
                                                            referenceSurface,
                                                            controlSurface,
                                                            DecayDefinitionOptions.Distance);

            Assert.IsFalse(referenceSurface.Exists);
            Assert.IsFalse(controlSurface.Exists);
            Assert.AreEqual((MM) 8.731446, surfacesToMorph[0].BoundingBox.ZSize);
        }

        [Test]
        public void TwoSurfaceMorphTestDistance2()
        {
            var activeModel = _powerShape.ActiveModel;
            activeModel.Import(new File(TestFiles.TWO_SURFACE_MORPH));

            var surfacesToMorph = new List<PSSurface>();
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("1"));
            surfacesToMorph.Add(activeModel.Surfaces.GetByName("2"));

            var referenceSurface = activeModel.Surfaces.GetByName("3");
            var controlSurface = activeModel.Surfaces.GetByName("4");

            PSEntityMorpher.MorphSurfacesBetweenTwoSurfaces(surfacesToMorph,
                                                            referenceSurface,
                                                            controlSurface,
                                                            DecayDefinitionOptions.Distance,
                                                            DecayDefinitionBlend.Quartic,
                                                            false,
                                                            false,
                                                            null,
                                                            null,
                                                            50,
                                                            0.2);

            Assert.IsFalse(referenceSurface.Exists);
            Assert.IsFalse(controlSurface.Exists);
            Assert.AreEqual((MM) 0.079231, surfacesToMorph[0].BoundingBox.ZSize);
        }
    }
}