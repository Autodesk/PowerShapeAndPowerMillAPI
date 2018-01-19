// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.ProductInterface.PowerMILL;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.CollectionTests
{
    [TestFixture]
    public class PMToolsCollectionTest
    {
        private PMAutomation _powerMILL;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _powerMILL = new PMAutomation(InstanceReuse.UseExistingInstance);
            _powerMILL.DialogsOff();
            _powerMILL.CloseProject();
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
                _powerMILL.CloseProject();
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public void CreateBallNoseToolTest()
        {
            var toolpath = _powerMILL.ActiveProject.Tools.CreateBallNosedTool();
            Assert.AreEqual("1", toolpath.Name);
        }

        [Test]
        public void CreateBarrelToolTest()
        {
            var toolpath = _powerMILL.ActiveProject.Tools.CreateBarrelTool();
            Assert.AreEqual("1", toolpath.Name);
        }

        [Test]
        public void CreateDovetailToolTest()
        {
            var toolpath = _powerMILL.ActiveProject.Tools.CreateDovetailTool();
            Assert.AreEqual("1", toolpath.Name);
        }

        [Test]
        public void CreateDrillToolTest()
        {
            var toolpath = _powerMILL.ActiveProject.Tools.CreateDrillTool();
            Assert.AreEqual("1", toolpath.Name);
        }

        [Test]
        public void CreateEndMillToolTest()
        {
            var toolpath = _powerMILL.ActiveProject.Tools.CreateEndMillTool();
            Assert.AreEqual("1", toolpath.Name);
        }

        [Test]
        public void CreateFormToolTest()
        {
            var toolpath = _powerMILL.ActiveProject.Tools.CreateFormTool();
            Assert.AreEqual("1", toolpath.Name);
        }

        [Test]
        public void CreateOffCentreTipRadiusedToolTest()
        {
            var toolpath = _powerMILL.ActiveProject.Tools.CreateOffCentreTipRadiusedTool();
            Assert.AreEqual("1", toolpath.Name);
        }

        [Test]
        public void CreateRoutingToolTest()
        {
            var toolpath = _powerMILL.ActiveProject.Tools.CreateRoutingTool();
            Assert.AreEqual("1", toolpath.Name);
        }

        [Test]
        public void CreateTapToolTest()
        {
            var toolpath = _powerMILL.ActiveProject.Tools.CreateTapTool();
            Assert.AreEqual("1", toolpath.Name);
        }

        [Test]
        public void CreateTaperedSphericalToolTest()
        {
            var toolpath = _powerMILL.ActiveProject.Tools.CreateTaperedSphericalTool();
            Assert.AreEqual("1", toolpath.Name);
        }

        [Test]
        public void CreateTaperedTippedToolTest()
        {
            var toolpath = _powerMILL.ActiveProject.Tools.CreateTaperedTippedTool();
            Assert.AreEqual("1", toolpath.Name);
        }

        [Test]
        public void CreateThreadMillToolTest()
        {
            var toolpath = _powerMILL.ActiveProject.Tools.CreateThreadMillTool();
            Assert.AreEqual("1", toolpath.Name);
        }

        [Test]
        public void CreateTippedDiscToolTest()
        {
            var toolpath = _powerMILL.ActiveProject.Tools.CreateTippedDiscTool();
            Assert.AreEqual("1", toolpath.Name);
        }

        [Test]
        public void CreateTipRadiusedToolTest()
        {
            var toolpath = _powerMILL.ActiveProject.Tools.CreateTipRadiusedTool();
            Assert.AreEqual("1", toolpath.Name);
        }

        [Test]
        public void GetNewEntityNameTest()
        {
            //No entity exists, should return "1" and "NewName"
            Assert.That(_powerMILL.ActiveProject.Tools.GetNewEntityName(), Is.EqualTo("1"));
            Assert.That(_powerMILL.ActiveProject.Tools.GetNewEntityName("NewName"), Is.EqualTo("NewName"));

            //Create a new entity
            var tool = _powerMILL.ActiveProject.Tools.CreateBallNosedTool();

            //1 entity exists, should return "2"
            Assert.That(_powerMILL.ActiveProject.Tools.GetNewEntityName(), Is.EqualTo("2"));

            //Rename the entity
            tool.Name = "MyPrefix";

            //1 entity called "MyPrefix" exists, should return "NewName" and "MyPrefix_1"
            Assert.That(_powerMILL.ActiveProject.Tools.GetNewEntityName("NewName"), Is.EqualTo("NewName"));
            Assert.That(_powerMILL.ActiveProject.Tools.GetNewEntityName("MyPrefix"), Is.EqualTo("MyPrefix_1"));

            //Delete the entity
            _powerMILL.ActiveProject.Tools.Remove(tool);
        }
    }
}