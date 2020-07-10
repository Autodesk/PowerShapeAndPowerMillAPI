// **********************************************************************
// *         © COPYRIGHT 2020 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerMILL;
using Autodesk.ProductInterface.PowerMILLTest.Files;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.PMEntityTests
{
    [TestFixture]
    public class PMToolTipRadiusedTest
    {
        private PMAutomation _powerMill;
        private const double TOLERANCE = 0.00001;

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

        [SetUp]
        public void SetUp()
        {
            _powerMill.CloseProject();
            if (_powerMill.Version.Major >= 2018)
            {
                _powerMill.LoadProject(TestFiles.ToolProperties2018);
            }
            else
            {
                _powerMill.LoadProject(TestFiles.ToolProperties);
            }
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

        #region Properties tests

        [Test]
        public void TipRadiusTest()
        {
            var tool = (PMToolTipRadiused)_powerMill.ActiveProject.Tools[2];
            var current = tool.TipRadius;
            tool.TipRadius = current + 0.2;
            Assert.That(tool.TipRadius, Is.EqualTo(current + 0.2));
        }

        #endregion
        
    }
}