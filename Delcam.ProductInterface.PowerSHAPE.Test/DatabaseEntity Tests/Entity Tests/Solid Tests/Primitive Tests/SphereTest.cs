// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest.DatabaseEntity_Tests.Entity_Tests.Solid_Tests.Primitive_Tests
{
    [TestFixture]
    public class SphereTest : EntityTest<PSSolidSphere>
    {
        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
        }

        [Test]
        public void RadiusGetTest()
        {
            // Create a SolidSphere
            var sphere = _powerSHAPE.ActiveModel.Solids.CreateSphere(new Point(), 5.0);

            // Test that the radius is 5.0

            Assert.IsTrue(sphere.Radius == 5);
        }

        [Test]
        public void RadiusSetTest()
        {
            // Create a SolidSphere
            var sphere = _powerSHAPE.ActiveModel.Solids.CreateSphere(new Point(), 5.0);

            // Set the radius to 10.3
            sphere.Radius = 10.3f;

            // Test the radius is 10.3
            Assert.IsTrue(sphere.Radius == 10.3);
        }
    }
}