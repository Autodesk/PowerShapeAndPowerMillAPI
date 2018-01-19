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
    public class PMNCProgramsCollectionTest
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

        #region Tests

        [Test]
        public void CreateNCProgram()
        {
            _powerMILL.ActiveProject.NCPrograms.CreateNCProgram("321");
            _powerMILL.Execute("bool exists = entity_exists('ncprogram', '321')");
            Assert.AreEqual("1",
                            _powerMILL.ExecuteEx("PRINT $exists"),
                            "An NC program with name '321' was expected, but does not exist.");
        }

        [Test]
        public void GetNewEntityNameTest()
        {
            //No entity exists, should return "1" and "NewName"
            Assert.That(_powerMILL.ActiveProject.NCPrograms.GetNewEntityName(), Is.EqualTo("1"));
            Assert.That(_powerMILL.ActiveProject.NCPrograms.GetNewEntityName("NewName"), Is.EqualTo("NewName"));

            //Create a new entity
            var ncProgram = _powerMILL.ActiveProject.NCPrograms.CreateNCProgram("1");

            //1 entity exists, should return "2"
            Assert.That(_powerMILL.ActiveProject.NCPrograms.GetNewEntityName(), Is.EqualTo("2"));

            //Rename the entity
            ncProgram.Name = "MyPrefix";

            //1 entity called "MyPrefix" exists, should return "NewName" and "MyPrefix_1"
            Assert.That(_powerMILL.ActiveProject.NCPrograms.GetNewEntityName("NewName"), Is.EqualTo("NewName"));
            Assert.That(_powerMILL.ActiveProject.NCPrograms.GetNewEntityName("MyPrefix"), Is.EqualTo("MyPrefix_1"));

            //Delete the entity
            _powerMILL.ActiveProject.NCPrograms.Remove(ncProgram);
        }

        #endregion
    }
}