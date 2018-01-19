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
    /// This is a test class for PlaneTest and is intended
    /// to contain all PlaneTest Unit Tests
    /// </summary>
    [TestFixture]
    public class PlaneTest
    {
        #region Fields

        private PSAutomation _powerSHAPE;

        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _powerSHAPE = new PSAutomation(InstanceReuse.UseExistingInstance);
            _powerSHAPE.DialogsOff();
        }

        [SetUp]
        public void SetUp()
        {
            _powerSHAPE.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _powerSHAPE.Reset();
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public void ConvertTest()
        {
            var plane = _powerSHAPE.ActiveModel.Surfaces.CreatePlane(new Point(), Planes.XY);
            var surface = plane.ConvertSurfaceToPowerSurface();
            Assert.AreEqual(true, surface.Exists);
            Assert.AreEqual(false, plane.Exists);
        }

        [Test]
        public void AddSurfaceCurveByParameterTest()
        {
            var plane = _powerSHAPE.ActiveModel.Surfaces.CreatePlane(new Point(), Planes.XY);
            var ex = Assert.Throws<Exception>(() => plane.AddSurfaceCurveByParameter(SurfaceCurveTypes.Lateral, 1.5f));
            Assert.AreEqual("Surface curves cannot be created for this type of surface primitive", ex.Message);
        }

        /// <summary>
        /// A test for Plane Constructor
        /// </summary>
        [Test]
        [Ignore("")]
        public void PlaneConstructorTest1()
        {
            //Automation powershape = null; // TODO: Initialize to an appropriate value
            //Point origin = null; // TODO: Initialize to an appropriate value
            //Plane target = new Plane(powershape, origin);
            //Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        /// A test for Plane Constructor
        /// </summary>
        [Test]
        [Ignore("")]
        public void PlaneConstructorTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Plane target = new Plane(powerSHAPE);
            //Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        /// A test for Length
        /// </summary>
        [Test]
        [Ignore("")]
        public void LengthTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Plane target = new Plane(powerSHAPE); // TODO: Initialize to an appropriate value
            //float expected = 0F; // TODO: Initialize to an appropriate value
            //float actual;
            //target.Length = expected;
            //actual = target.Length;
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Volume
        /// </summary>
        [Test]
        [Ignore("")]
        public void VolumeTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Plane target = new Plane(powerSHAPE); // TODO: Initialize to an appropriate value
            //float actual;
            //actual = target.Volume;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Width
        /// </summary>
        [Test]
        [Ignore("")]
        public void WidthTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Plane target = new Plane(powerSHAPE); // TODO: Initialize to an appropriate value
            //float expected = 0F; // TODO: Initialize to an appropriate value
            //float actual;
            //target.Width = expected;
            //actual = target.Width;
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}