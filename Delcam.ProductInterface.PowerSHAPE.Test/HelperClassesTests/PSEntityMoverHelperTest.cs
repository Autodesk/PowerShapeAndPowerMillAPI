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

namespace Autodesk.ProductInterface.PowerSHAPETest.HelperClassesTests
{
    [TestFixture]
    public class PSEntityMoverHelperTest
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
        public void MoveEntityBySmallAmountTest()
        {
            var activeModel = _powerShape.ActiveModel;
            activeModel.Import(new File(TestFiles.SINGLE_WORKPLANE1));
            var wp = activeModel.Workplanes.Last();
            double distance = 0.00007;
            var newWp = wp.MoveByVector(new Vector(distance, distance, distance), 1)[0] as PSWorkplane;

            Assert.That(newWp.Origin.X, Is.EqualTo(wp.Origin.X + distance));
            Assert.That(newWp.Origin.Y, Is.EqualTo(wp.Origin.Y + distance));
            Assert.That(newWp.Origin.Z, Is.EqualTo(wp.Origin.Z + distance));
        }
    }
}