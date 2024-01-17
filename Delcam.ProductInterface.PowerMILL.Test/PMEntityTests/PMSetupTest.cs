﻿// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.ProductInterface.PowerMILL;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.PMEntityTests
{
    [TestFixture]
    public class PMSetupTest
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

        #region Test properties

        [Test]
        public void Property_Toolpaths()
        {
            // Test project only opens in 2023 and greater
            if (_powerMILL.Version.Major < 2023) Assert.Pass();

            _powerMILL.LoadProject(Files.TestFiles.SetupsProject);

            var toolpaths = _powerMILL.ActiveProject.Setups["1"].Toolpaths;

            var actualToolpathNames = toolpaths.Select(x => x.Name);
            var expectedToolpathNames = new[] { "6", "8", "7" };

            Assert.AreEqual(actualToolpathNames,expectedToolpathNames);
        }

        [Test]
        public void Property_Toolpaths_Empty()
        {
            // Test project only opens in 2023 and greater
            if (_powerMILL.Version.Major < 2023) Assert.Pass();

            _powerMILL.LoadProject(Files.TestFiles.SetupsProject);

            var toolpaths = _powerMILL.ActiveProject.Setups["3"].Toolpaths;

            Assert.AreEqual(toolpaths.Count, 0);
        }

        #endregion
    }
}