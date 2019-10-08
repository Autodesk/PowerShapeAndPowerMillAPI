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
using Autodesk.ProductInterface.PowerMILLTest.Files;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.PMEntityTests
{
    [TestFixture]
    public class PMLevelOrSetTests
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

        #endregion


        #region Test operations

        [Test]
        public void DrawAndUndrawAllLevelsOrSetsTest()
        {
            _powerMill.ActiveProject.LevelsAndSets.CreateLevel("Test level 1");
            _powerMill.ActiveProject.LevelsAndSets.CreateLevel("Test level 2");
            _powerMill.ActiveProject.LevelsAndSets.CreateSet("Test set 1");
            _powerMill.ActiveProject.LevelsAndSets.CreateSet("Test set 2");
            _powerMill.ActiveProject.LevelsAndSets.DrawAll();
            _powerMill.ActiveProject.LevelsAndSets.UndrawAll();
            Assert.IsTrue(true);
        }

        [Test]
        public void SelectAllTest()
        {
            _powerMill.ActiveProject.Models.CreateModel(TestFiles.SingleMesh);
            _powerMill.ActiveProject.LevelsAndSets.GetByName("Triangle Model").SelectAll();
            Assert.IsTrue(true);
        }

        [Test]
        public void SelectWireframeTest()
        {
            _powerMill.ActiveProject.Models.CreateModel(TestFiles.SingleMesh);
            _powerMill.ActiveProject.LevelsAndSets.GetByName("Triangle Model").SelectWireframe();
            Assert.IsTrue(true);
        }

        [Test]
        public void SelectSurfacesTest()
        {
            _powerMill.ActiveProject.Models.CreateModel(TestFiles.SingleMesh);
            _powerMill.ActiveProject.LevelsAndSets.GetByName("Triangle Model").SelectSurfaces();
            Assert.IsTrue(true);
        }

        #endregion

    }
}
