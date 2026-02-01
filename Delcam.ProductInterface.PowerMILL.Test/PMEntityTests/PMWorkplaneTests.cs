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
using Autodesk.ProductInterface.PowerMILL;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.PMEntityTests
{
    [TestFixture]
    public class PMWorkplaneTests
    {
        private PMAutomation _powerMill;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _powerMill = new PMAutomation(InstanceReuse.UseExistingInstance);
            _powerMill.DialogsOff();
            _powerMill.CloseProject();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _powerMill.CloseProject();
            }
            catch (Exception)
            {
            }
        }


        #region Test properties

        [Test]
        public void TestOriginProperty()
        {
            var workplane = new Geometry.Workplane
            {
                Origin = new Geometry.Point(10, 20, 30)
            };
            var pmWorkplane = _powerMill.ActiveProject.Workplanes.CreateWorkplane(workplane);
            Assert.AreEqual(10, pmWorkplane.Origin.X, 1e-6);
            Assert.AreEqual(20, pmWorkplane.Origin.Y, 1e-6);
            Assert.AreEqual(30, pmWorkplane.Origin.Z, 1e-6);
        }


        #endregion


        #region Test operations

        [Test]
        public void DrawAndUndrawAllWorkplaneTest()
        {
            _powerMill.ActiveProject.Workplanes.CreateWorkplane(new Geometry.Workplane());
            _powerMill.ActiveProject.Workplanes.CreateWorkplane(new Geometry.Workplane());
            _powerMill.ActiveProject.Workplanes.DrawAll();
            _powerMill.ActiveProject.Workplanes.UndrawAll();
            Assert.IsTrue(true);
        }

        #endregion

    }
}
